using Confluent.Kafka;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using WalletService.Core.Kafka.Messages;
using WalletService.Core.Kafka.Producer;
using WalletService.Core.Kafka.Topics;
using WalletService.Data.Adapters.IAdapters;
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

public class ProcessPaymentEcoPoolConsumer : BaseKafkaConsumer
{

    public ProcessPaymentEcoPoolConsumer(
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

            return PaymentEcoPoolProcess();
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

    private async Task<bool> PaymentEcoPoolProcess()
    {
        using var                       scope                    = ServiceScopeFactory.CreateScope();
        var                             resultsEcoPoolRepository = scope.ServiceProvider.GetService<IResultsEcoPoolRepository>();
        var                             walletRepository         = scope.ServiceProvider.GetService<IWalletRepository>();
        var                             configurationAdapter     = scope.ServiceProvider.GetService<IConfigurationAdapter>();
        var                             kafkaProducer            = scope.ServiceProvider.GetService<KafkaProducer>();
        ICollection<UserGradingRequest> listGrading              = new List<UserGradingRequest>();
        var                             responseGradings         = new GradingResponse();
        var                             listEcoPools             = await resultsEcoPoolRepository!.GetResultsEcoPoolToPayment();
        var dictionary = listEcoPools.GroupBy(x
            => x.AffiliateId).ToDictionary(group => group.Key, group => group.ToList());

        if (listEcoPools is { Count: 0 })
            return false;

        var gradingResponse = await configurationAdapter!.GetGradings();
        if (gradingResponse.IsSuccessful && !string.IsNullOrEmpty(gradingResponse.Content))
            responseGradings = gradingResponse.Content.ToJsonObject<GradingResponse>();

        ICollection<GradingDto> gradings = responseGradings!.Data;
        foreach (var (key, listPoolsPerUser) in dictionary)
        {
            if (listPoolsPerUser.Any(x => x.CompletedAt != null))
                continue;

            var globalPayment = (double)listPoolsPerUser.Sum(x => x.PaymentAmount);
            var userName      = listPoolsPerUser.First().AffiliateName;
            if (walletRepository is not null && globalPayment is not 0)
                await CreateCreditPayment(
                    walletRepository,
                    key,
                    userName,
                    globalPayment,
                    Constants.CommissionPoolDescriptionNormal,
                    WalletConceptType.purchasing_pool.ToString());

            var listPerLevels = listPoolsPerUser.SelectMany(x => x.ResultEcoPoolLevels);
            var dictionaryPerLevels = listPerLevels.GroupBy(x
                => x.AffiliateId).ToDictionary(group => group.Key, group => group.ToList());

            foreach (var (userLevel, ecoPoolLevelsList) in dictionaryPerLevels)
            {
                if (ecoPoolLevelsList.Any(x => x.CompletedAt != null))
                    continue;

                var userNameLevel         = ecoPoolLevelsList.First().AffiliateName;
                var level                 = ecoPoolLevelsList.First().Level;
                var side                  = ecoPoolLevelsList.First().BinarySide;
                var globalPaymentLevel    = (double)ecoPoolLevelsList.Sum(x => x.PaymentAmount);
                var globalCommissionLevel = (double)ecoPoolLevelsList.Sum(x => x.CommisionAmount);
                var globalPointsLevel     = (double)ecoPoolLevelsList.Sum(x => x.Points)!;

                if (walletRepository is not null && globalPaymentLevel is not 0)
                    await CreateCreditPayment(
                        walletRepository,
                        userLevel,
                        userNameLevel,
                        globalPaymentLevel,
                        string.Format(Constants.CommissionPoolDescription, userNameLevel, level),
                        WalletConceptType.pool_commission.ToString());
                AddOrUpdateGrading(ref listGrading, userLevel, key, userNameLevel, globalCommissionLevel, globalPointsLevel, side,
                    gradings);
            }
        }

        var model = new ModelFourMessage
        {
            Gradings     = gradings.Where(x => x.Id > 1).ToList(),
            UserGradings = listGrading.Where(x => x.Grading is { Id: > 1 }).ToList()
        }.ToJsonString();

        _ = Task.Run(async ()
            => await kafkaProducer!.ProduceAsync(KafkaTopics.ProcessModelFourTopic, model));

        return true;
    }

    private static void AddOrUpdateGrading(
        ref ICollection<UserGradingRequest> listGrading,
        int                                 userLevel,
        int                                 userOwner,
        string                              userName,
        double                              globalPaymentLevel,
        double                              points,
        int                                 side,
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
                Side             = side
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
        // return walletRepository.CreditTransaction(creditTransaction);
        return Task.CompletedTask;
    }

}