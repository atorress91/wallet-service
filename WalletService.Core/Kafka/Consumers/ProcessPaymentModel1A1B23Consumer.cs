using System.Security.Cryptography.X509Certificates;
using Confluent.Kafka;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using WalletService.Core.Kafka.Messages;
using WalletService.Core.Kafka.Producer;
using WalletService.Core.Kafka.Topics;
using WalletService.Data.Adapters.IAdapters;
using WalletService.Data.Database.Models;
using WalletService.Data.Repositories.IRepositories;
using WalletService.Models.Configuration;
using WalletService.Models.Constants;
using WalletService.Models.DTO.GradingDto;
using WalletService.Models.Enums;
using WalletService.Models.Requests.UserGradingRequest;
using WalletService.Models.Requests.WalletRequest;
using WalletService.Models.Responses;
using WalletService.Utility.Extensions;

namespace WalletService.Core.Kafka.Consumers;

public class ProcessPaymentModel1A1B23Consumer : BaseKafkaConsumer
{

    public ProcessPaymentModel1A1B23Consumer(
        ConsumerSettings         consumerSettings,
        ApplicationConfiguration configuration,
        ILogger                  logger,
        IServiceScopeFactory     serviceScopeFactory)
        : base(consumerSettings, configuration, logger, serviceScopeFactory)
    {
    }


    protected override Task<bool> OnMessage(ConsumeResult<Ignore, string> e)
    {
        try
        {
            Logger.LogInformation("[ProcessPaymentEcoPoolConsumer] OnMessage | Init");

            return Process();
        }
        catch (Exception ex)
        {
            Logger.LogError("[ProcessPaymentEcoPoolConsumer] Error | Data: {Exception}", ex);
            return Task.FromResult(false);
        }
        finally
        {
            Logger.LogInformation("[ProcessPaymentEcoPoolConsumer] OnMessage | End");
        }
    }

    private async Task<bool> Process()
    {
        using var                       scope                    = ServiceScopeFactory.CreateScope();
        var                             resultsEcoPoolRepository = scope.ServiceProvider.GetService<IResultsEcoPoolRepository>();
        var                             walletRepository         = scope.ServiceProvider.GetService<IWalletRepository>();
        var                             wallet1ARepository         = scope.ServiceProvider.GetService<IWalletModel1ARepository>();
        var                             wallet1BRepository         = scope.ServiceProvider.GetService<IWalletModel1BRepository>();
        var                             configurationAdapter     = scope.ServiceProvider.GetService<IConfigurationAdapter>();
        var                             kafkaProducer            = scope.ServiceProvider.GetService<KafkaProducer>();
        ICollection<UserGradingRequest> listGrading              = new List<UserGradingRequest>();
        var                             responseGradings         = new GradingResponse();
        
        var listModel1AResults = await resultsEcoPoolRepository!.GetResultsModel1AToPayment();
        var listModel1BResults = await resultsEcoPoolRepository!.GetResultsModel1BToPayment();
        var listModel2Results = await resultsEcoPoolRepository!.GetResultsModel2ToPayment();
        var listModel3Results = await resultsEcoPoolRepository!.GetResultsMode3ToPayment();
        
        var dictionaryPointsModelTotal = new Dictionary<int, double>();
        
        var dictionaryModel1A = listModel1AResults.GroupBy(x
            => x.AffiliateId).ToDictionary(group => group.Key, group => group.ToList());
        
        var dictionaryModel1B = listModel1BResults.GroupBy(x
            => x.AffiliateId).ToDictionary(group => group.Key, group => group.ToList());
        
        var dictionaryModel2 = listModel2Results.GroupBy(x
            => x.AffiliateId).ToDictionary(group => group.Key, group => group.ToList());
        
        var dictionaryModel3 = listModel3Results.GroupBy(x
            => x.AffiliateId).ToDictionary(group => group.Key, group => group.ToList());

        if (listModel2Results is { Count: 0 } && listModel3Results is { Count: 0 })
            return false;

        var gradingResponse = await configurationAdapter!.GetGradings();
        if (gradingResponse.IsSuccessful && !string.IsNullOrEmpty(gradingResponse.Content))
            responseGradings = gradingResponse.Content.ToJsonObject<GradingResponse>();

        ICollection<GradingDto> gradings = responseGradings!.Data;
        
        await CommissionsModel1A(dictionaryModel1A, wallet1ARepository, walletRepository, dictionaryPointsModelTotal);

        await CommissionsModel1B(dictionaryModel1B, wallet1BRepository, walletRepository, dictionaryPointsModelTotal);

        await CommissionsModel3(dictionaryModel3, walletRepository, dictionaryPointsModelTotal);
        
        var model = await CommissionsModel2(dictionaryModel2, walletRepository, dictionaryPointsModelTotal, listGrading, gradings);

        _ = Task.Run(async ()
            => await kafkaProducer!.ProduceAsync(KafkaTopics.ProcessModelFourFiveSixTopic, model.ToJsonString()));

            return true;
    }

    private static async Task<ModelFourFiveSixMessage> CommissionsModel2(
        Dictionary<int, List<ResultsModel2>>  dictionary,
        IWalletRepository?                     walletRepository,
        Dictionary<int, double>                dictionaryPointsModelTwo,
        ICollection<UserGradingRequest>        listGrading,
        ICollection<GradingDto>                gradings)
    {
        foreach (var (key, listPoolsPerUser) in dictionary)
        {

            var globalPayment = (double)listPoolsPerUser.Sum(x => x.PaymentAmount);
            var userName      = listPoolsPerUser.First().AffiliateName;
            if (walletRepository is not null && globalPayment is not 0)
                await CreateCreditPayment(
                    walletRepository,
                    key,
                    userName,
                    globalPayment,
                    Constants.CommissionModelThreeDescriptionNormal,
                    WalletConceptType.purchasing_pool.ToString());

            var listPerLevels = listPoolsPerUser.SelectMany(x => x.ResultsModel2Levels);
            var dictionaryPerLevels = listPerLevels.GroupBy(x
                => x.AffiliateId).ToDictionary(group => group.Key, group => group.ToList());

            foreach (var (userLevel, ecoPoolLevelsList) in dictionaryPerLevels)
            {
                if (ecoPoolLevelsList.Any(x => x.CompletedAt != null))
                    continue;

                var    userNameLevel      = ecoPoolLevelsList.First().AffiliateName;
                var    level              = ecoPoolLevelsList.First().Level;
                var    side               = ecoPoolLevelsList.First().BinarySide;
                var    userCreatedAt      = ecoPoolLevelsList.First().UserCreatedAt ?? DateTime.Now;
                var    globalPaymentLevel = (double)ecoPoolLevelsList.Sum(x => x.PaymentAmount);
                double globalCommissionLevel;

                if (dictionaryPointsModelTwo.Any(x => x.Key == userLevel))
                    globalCommissionLevel = (double)ecoPoolLevelsList.Sum(x => x.CommisionAmount) + dictionaryPointsModelTwo[userLevel];
                else
                    globalCommissionLevel = (double)ecoPoolLevelsList.Sum(x => x.CommisionAmount);

                var globalPointsLevel = (double)ecoPoolLevelsList.Sum(x => x.Points)!;

                if (walletRepository is null || globalPaymentLevel is 0)
                    continue;

                await CreateCreditPayment(
                    walletRepository,
                    userLevel,
                    userNameLevel,
                    globalPaymentLevel,
                    string.Format(Constants.CommissionModelThreeDescription, userName, level),
                    WalletConceptType.pool_commission.ToString());

                AddOrUpdateGrading(ref listGrading, userLevel, key, userNameLevel, globalCommissionLevel, globalPointsLevel, side,
                    userCreatedAt,
                    gradings);
            }
        }

        var model = new ModelFourFiveSixMessage
        {
            Gradings     = gradings.Where(x => x.Id > 1).ToList(),
            UserGradings = listGrading.Where(x => x.Grading is { Id: > 1 }).ToList()
        };
        return model;
    }

    private static async Task CommissionsModel3(
        Dictionary<int, List<ResultsModel3>> dictionaryModelTwo,
        IWalletRepository?                     walletRepository,
        Dictionary<int, double>                dictionaryPointsModelTwo)
    {
        foreach (var (key, listPoolsPerUser) in dictionaryModelTwo)
        {

            var globalPayment = (double)listPoolsPerUser.Sum(x => x.PaymentAmount);
            var userName      = listPoolsPerUser.First().AffiliateName;
            if (walletRepository is not null && globalPayment is not 0)
                await CreateCreditPayment(
                    walletRepository,
                    key,
                    userName,
                    globalPayment,
                    Constants.CommissionModelTwoDescriptionNormal,
                    WalletConceptType.purchasing_pool.ToString());

            var listPerLevels = listPoolsPerUser.SelectMany(x => x.ResultsModel3Levels);
            var dictionaryPerLevels = listPerLevels.GroupBy(x
                => x.AffiliateId).ToDictionary(group => group.Key, group => group.ToList());

            foreach (var (userLevel, ecoPoolLevelsList) in dictionaryPerLevels)
            {
                if (ecoPoolLevelsList.Any(x => x.CompletedAt != null))
                    continue;

                var userNameLevel      = ecoPoolLevelsList.First().AffiliateName;
                var level              = ecoPoolLevelsList.First().Level;
                var globalPaymentLevel = (double)ecoPoolLevelsList.Sum(x => x.PaymentAmount);

                if (walletRepository is null || globalPaymentLevel is 0)
                    continue;

                if (dictionaryPointsModelTwo.ContainsKey(userLevel))
                    dictionaryPointsModelTwo[userLevel] += globalPaymentLevel;
                else
                    dictionaryPointsModelTwo.Add(userLevel, globalPaymentLevel);

                await CreateCreditPayment(
                    walletRepository,
                    userLevel,
                    userNameLevel,
                    globalPaymentLevel,
                    string.Format(Constants.CommissionModelTwoDescription, userName, level),
                    WalletConceptType.pool_commission.ToString());
            }
        }
    }   
    private static async Task CommissionsModel1A(
        Dictionary<int, List<ResultsModel1A>> dictionaryModel1A,
        IWalletModel1ARepository?             walletRepository1A,
        IWalletRepository?                    walletRepository,
        Dictionary<int, double>               dictionaryPointsModel1A)
    {
        foreach (var (key, listPoolsPerUser) in dictionaryModel1A)
        {

            var globalPayment = (double)listPoolsPerUser.Sum(x => x.PaymentAmount);
            var userName      = listPoolsPerUser.First().AffiliateName;
            if (walletRepository is not null && globalPayment is not 0)
                await CreateCredit1APaymentServices(
                    walletRepository1A!,
                    key,
                    userName,
                    globalPayment,
                    Constants.CommissionModelTwoDescriptionNormal,
                    WalletConceptType.purchasing_pool.ToString());

            var listPerLevels = listPoolsPerUser.SelectMany(x => x.ResultsModel1ALevels);
            var dictionaryPerLevels = listPerLevels.GroupBy(x
                => x.AffiliateId).ToDictionary(group => group.Key, group => group.ToList());

            foreach (var (userLevel, ecoPoolLevelsList) in dictionaryPerLevels)
            {
                if (ecoPoolLevelsList.Any(x => x.CompletedAt != null))
                    continue;

                var userNameLevel      = ecoPoolLevelsList.First().AffiliateName;
                var level              = ecoPoolLevelsList.First().Level;
                var globalPaymentLevel = (double)ecoPoolLevelsList.Sum(x => x.PaymentAmount);

                if (walletRepository is null || globalPaymentLevel is 0)
                    continue;

                if (dictionaryPointsModel1A.ContainsKey(userLevel))
                    dictionaryPointsModel1A[userLevel] += globalPaymentLevel;
                else
                    dictionaryPointsModel1A.Add(userLevel, globalPaymentLevel);

                await CreateCreditPayment(
                    walletRepository,
                    userLevel,
                    userNameLevel,
                    globalPaymentLevel,
                    string.Format(Constants.CommissionModelTwoDescription, userName, level),
                    WalletConceptType.pool_commission.ToString());
            }
        }
    }
    private static async Task CommissionsModel1B(
        Dictionary<int, List<ResultsModel1B>> dictionaryModel1B,
        IWalletModel1BRepository?             walletRepository1B,
        IWalletRepository?                    walletRepository,
        Dictionary<int, double>               dictionaryPointsModel1B)
    {
        foreach (var (key, listPoolsPerUser) in dictionaryModel1B)
        {

            var globalPayment = (double)listPoolsPerUser.Sum(x => x.PaymentAmount);
            var userName      = listPoolsPerUser.First().AffiliateName;
            if (walletRepository is not null && globalPayment is not 0)
                await CreateCredit1BPaymentServices(
                    walletRepository1B!,
                    key,
                    userName,
                    globalPayment,
                    Constants.CommissionModelTwoDescriptionNormal,
                    WalletConceptType.purchasing_pool.ToString());

            var listPerLevels = listPoolsPerUser.SelectMany(x => x.ResultsModel1BLevels);
            var dictionaryPerLevels = listPerLevels.GroupBy(x
                => x.AffiliateId).ToDictionary(group => group.Key, group => group.ToList());

            foreach (var (userLevel, ecoPoolLevelsList) in dictionaryPerLevels)
            {
                if (ecoPoolLevelsList.Any(x => x.CompletedAt != null))
                    continue;

                var userNameLevel      = ecoPoolLevelsList.First().AffiliateName;
                var level              = ecoPoolLevelsList.First().Level;
                var globalPaymentLevel = (double)ecoPoolLevelsList.Sum(x => x.PaymentAmount);

                if (walletRepository is null || globalPaymentLevel is 0)
                    continue;

                if (dictionaryPointsModel1B.ContainsKey(userLevel))
                    dictionaryPointsModel1B[userLevel] += globalPaymentLevel;
                else
                    dictionaryPointsModel1B.Add(userLevel, globalPaymentLevel);

                await CreateCreditPayment(
                    walletRepository,
                    userLevel,
                    userNameLevel,
                    globalPaymentLevel,
                    string.Format(Constants.CommissionModelTwoDescription, userName, level),
                    WalletConceptType.pool_commission.ToString());
            }
        }
    }

    private static void AddOrUpdateGrading(
        ref ICollection<UserGradingRequest> listGrading,
        int                                 userLevel,
        int                                 userOwner,
        string                              userName,
        double                              globalPaymentLevel,
        double                              points,
        int                                 side,
        DateTime                            userCreateAt,
        ICollection<GradingDto>?            gradings)
    {
        var userGrading = listGrading.FirstOrDefault(x => x.AffiliateId == userLevel);
        if (userGrading is not null)
        {
            userGrading.Points      += points;
            userGrading.Commissions += globalPaymentLevel;
        }
        else
        {
            userGrading = new UserGradingRequest
            {
                AffiliateId      = userLevel,
                AffiliateOwnerId = userOwner,
                UserName         = userName,
                Commissions      = globalPaymentLevel,
                Points           = points,
                Side             = side,
                UserCreatedAt    = userCreateAt
            };
            listGrading.Add(userGrading);
        }

        if (gradings is null)
            return;
        gradings = gradings.Where(x => x.Id != 1).ToList();

        var gradingTempPoints = gradings.OrderByDescending(x
            => x.ScopeLevel).FirstOrDefault(x => x.VolumePoints < userGrading.Commissions);

        if (gradingTempPoints is not null)
            userGrading.Grading = gradingTempPoints;
    }

    private static Task CreateCreditPayment(
        IWalletRepository walletRepository,
        int               userId,
        string            userName,
        double            globalPayment,
        string            concept,
        string            conceptType)
    {
        if (globalPayment <= 0)
            return Task.CompletedTask;
        
        // return Task.CompletedTask;

        var creditTransaction = new CreditTransactionRequest
        {
            AffiliateId       = userId,
            AffiliateUserName = userName,
            Credit            = globalPayment,
            UserId            = Constants.AdminUserId,
            Concept           = concept,
            AdminUserName     = Constants.AdminEcosystemUserName,
            ConceptType       = conceptType
        };
        return walletRepository.CreditTransaction(creditTransaction);
    }
    
    private static Task CreateCredit1APayment(
        IWalletModel1ARepository walletRepository,
        int               userId,
        string            userName,
        double            globalPayment,
        string            concept,
        string            conceptType)
    {
        if (globalPayment <= 0)
            return Task.CompletedTask;
        
        // return Task.CompletedTask;

        var creditTransaction = new CreditTransactionRequest
        {
            AffiliateId       = userId,
            AffiliateUserName = userName,
            Credit            = globalPayment,
            UserId            = Constants.AdminUserId,
            Concept           = concept,
            AdminUserName     = Constants.AdminEcosystemUserName,
            ConceptType       = conceptType
        };
        return walletRepository.CreditTransaction(creditTransaction);
    }
    
    private static Task CreateCredit1APaymentServices(
        IWalletModel1ARepository walletRepository,
        int                      userId,
        string                   userName,
        double                   globalPayment,
        string                   concept,
        string                   conceptType)
    {
        if (globalPayment <= 0)
            return Task.CompletedTask;
        
        // return Task.CompletedTask;

        var creditTransaction = new CreditTransactionRequest
        {
            AffiliateId       = userId,
            AffiliateUserName = userName,
            Credit            = globalPayment,
            UserId            = Constants.AdminUserId,
            Concept           = concept,
            AdminUserName     = Constants.AdminEcosystemUserName,
            ConceptType       = conceptType
        };
        return walletRepository.CreditServiceBalanceTransaction(creditTransaction);
    }
    
    private static Task CreateCredit1BPayment(
        IWalletModel1BRepository walletRepository,
        int                      userId,
        string                   userName,
        double                   globalPayment,
        string                   concept,
        string                   conceptType)
    {
        if (globalPayment <= 0)
            return Task.CompletedTask;
        
        // return Task.CompletedTask;

        var creditTransaction = new CreditTransactionRequest
        {
            AffiliateId       = userId,
            AffiliateUserName = userName,
            Credit            = globalPayment,
            UserId            = Constants.AdminUserId,
            Concept           = concept,
            AdminUserName     = Constants.AdminEcosystemUserName,
            ConceptType       = conceptType
        };
        return walletRepository.CreditTransaction(creditTransaction);
    }
    
    private static Task CreateCredit1BPaymentServices(
        IWalletModel1BRepository walletRepository,
        int                      userId,
        string                   userName,
        double                   globalPayment,
        string                   concept,
        string                   conceptType)
    {
        if (globalPayment <= 0)
            return Task.CompletedTask;
        
        // return Task.CompletedTask;

        var creditTransaction = new CreditTransactionRequest
        {
            AffiliateId       = userId,
            AffiliateUserName = userName,
            Credit            = globalPayment,
            UserId            = Constants.AdminUserId,
            Concept           = concept,
            AdminUserName     = Constants.AdminEcosystemUserName,
            ConceptType       = conceptType
        };
        return walletRepository.CreditServiceBalanceTransaction(creditTransaction);
    }

}