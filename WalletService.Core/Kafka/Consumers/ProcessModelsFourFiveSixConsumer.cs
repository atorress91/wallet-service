using System.Text.Json;
using Confluent.Kafka;
using iText.Layout.Element;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using WalletService.Core.Kafka.Messages;
using WalletService.Data.Adapters.IAdapters;
using WalletService.Data.Repositories.IRepositories;
using WalletService.Models.Configuration;
using WalletService.Models.Constants;
using WalletService.Models.DTO.AffiliateInformation;
using WalletService.Models.DTO.GradingDto;
using WalletService.Models.DTO.LeaderBoardDto;
using WalletService.Models.Enums;
using WalletService.Models.Requests.UserGradingRequest;
using WalletService.Models.Requests.WalletRequest;
using WalletService.Utility.Extensions;

namespace WalletService.Core.Kafka.Consumers;

public class ProcessModelsFourFiveSixConsumer : BaseKafkaConsumer
{
    public ProcessModelsFourFiveSixConsumer(
        ConsumerSettings         consumerSettings,
        ApplicationConfiguration configuration,
        ILogger                  logger,
        IServiceScopeFactory     serviceScopeFactory
    ) : base(consumerSettings, configuration, logger, serviceScopeFactory) { }

    protected override Task<bool> OnMessage(ConsumeResult<Ignore, string> e)
    {
        try
        {
            var message = JsonSerializer.Deserialize<ModelFourFiveSixMessage>(e.Message.Value);
            Logger.LogInformation("[ProcessModelsFourFiveSixConsumer] OnMessage | Init");
            return message is not null ? Process(message) : Task.FromResult(false);
        }
        catch (Exception ex)
        {
            Logger.LogError("[ProcessModelsFourFiveSixConsumer] Error | Data: {Exception}", ex);
            return Task.FromResult(false);
        }
        finally
        {
            Logger.LogInformation("[ProcessModelsFourFiveSixConsumer] OnMessage | End");
        }
    }

    private async Task<bool> Process(ModelFourFiveSixMessage model)
    {
        using var scope                 = ServiceScopeFactory.CreateScope();
        var       walletRepository      = scope.ServiceProvider.GetService<IWalletRepository>();
        var       accountServiceAdapter = scope.ServiceProvider.GetService<IAccountServiceAdapter>();

        var listUsersGraded = model.UserGradings.Where(x => x.Grading!.ScopeLevel >= Constants.CustomerModel4Scope[0]).ToList();

        if (listUsersGraded is { Count: 0 })
            return false;

        var dictionary = await DebitModelFourFiveProcess(model, listUsersGraded, walletRepository, accountServiceAdapter);
        
        var treeUsersInformation = await LeaderBoardModelFourProcess(listUsersGraded, accountServiceAdapter);

        var leaderBoardModel5 = new List<LeaderBoardModel5>();
        var leaderBoardModel6 = new List<LeaderBoardModel6>();

        await GradingModel4ToCreateNextModels(
            model,
            listUsersGraded,
            accountServiceAdapter,
            walletRepository!,
            dictionary,
            leaderBoardModel5,
            leaderBoardModel6,
            treeUsersInformation);

        await LeaderBoardModelFiveProcess(leaderBoardModel5, accountServiceAdapter);
        await LeaderBoardModelSixProcess(leaderBoardModel6, accountServiceAdapter);

        return true;
    }

    private static async Task LeaderBoardModelFiveProcess(
        List<LeaderBoardModel5> leaderBoardModel5,
        IAccountServiceAdapter? accountServiceAdapter)
    {

        leaderBoardModel5 = leaderBoardModel5.OrderModel5();
        await accountServiceAdapter!.DeleteTreeModel5();
        await accountServiceAdapter.AddTreeModel5(leaderBoardModel5);
        
    }
    
    private static async Task LeaderBoardModelSixProcess(
        List<LeaderBoardModel6> leaderBoardModel6,
        IAccountServiceAdapter? accountServiceAdapter)
    {
        leaderBoardModel6 = leaderBoardModel6.OrderModel6();
        await accountServiceAdapter!.DeleteTreeModel6();
        await accountServiceAdapter.AddTreeModel6(leaderBoardModel6);
    }

    private static async Task<ICollection<UserBinaryInformation>> LeaderBoardModelFourProcess(IEnumerable<UserGradingRequest> listUsersGraded, IAccountServiceAdapter? accountServiceAdapter)
    {
        var result = new List<UserBinaryInformation>();
        
        var leaderBoardModel4 = listUsersGraded.Select(s => new LeaderBoardModel4()
        {
            AffiliateId   = s.AffiliateId,
            Amount        = (decimal)s.Commissions,
            UserName      = s.UserName,
            CreatedAt     = new DateTime(),
            UserCreatedAt = new DateTime(),
        }).ToList();

        leaderBoardModel4 = leaderBoardModel4.OrderModel4();
        
        await accountServiceAdapter!.DeleteTreeModel4();
        var response = await accountServiceAdapter.AddTreeModel4(leaderBoardModel4);
        
        if (string.IsNullOrEmpty(response.Content))
            return result;

        result = response.Content.ToJsonObject<List<UserBinaryInformation>>();
        
        return result!;
    }
    
    private static async Task<Dictionary<int, decimal>> DebitModelFourFiveProcess(
        ModelFourFiveSixMessage         model,
        List<UserGradingRequest> listUsersGraded,
        IWalletRepository?       walletRepository,
        IAccountServiceAdapter?  accountServiceAdapter)
    {
        var userDictionary = listUsersGraded.ToDictionary(x => x.AffiliateId, _ => 0m);
        foreach (var item in listUsersGraded)
        {
            if (item.Grading!.ScopeLevel >= Constants.CustomerModel5Scope)
            {
                var model4Grading = model.Gradings.First(x => x.ScopeLevel == Constants.CustomerModel4Scope[1]);
                if (walletRepository is not null)
                    await CreateDebitPayment(
                        walletRepository,
                        item.AffiliateId,
                        item.UserName,
                        model4Grading.PersonalPurchases,
                        string.Format(Constants.ConceptBinaryPayment, model4Grading.PersonalPurchases, model4Grading.Name),
                        WalletConceptType.model_four_payment.ToString());

                userDictionary[item.AffiliateId] = model4Grading.PurchasesNetwork;
                
                if (walletRepository is not null)
                    await CreateDebitPayment(
                        walletRepository,
                        item.AffiliateId,
                        item.UserName,
                        item.Grading.PersonalPurchases,
                        string.Format(Constants.ConceptModelFivePayment, item.Grading.PersonalPurchases, item.Grading.Name),
                        WalletConceptType.model_five_payment.ToString());
                
                await accountServiceAdapter!.UpdateGradingByUser(item.AffiliateId, item.Grading.Id);
            }
            else
            {
                if (walletRepository is not null)
                    await CreateDebitPayment(
                        walletRepository,
                        item.AffiliateId,
                        item.UserName,
                        item.Grading.PersonalPurchases,
                        string.Format(Constants.ConceptBinaryPayment, item.Grading.PersonalPurchases, item.Grading.Name),
                        WalletConceptType.model_four_payment.ToString()
                    );
                userDictionary[item.AffiliateId] = item.Grading.PurchasesNetwork;
                
                await accountServiceAdapter!.UpdateGradingByUser(item.AffiliateId, item.Grading.Id);
            }
        }

        return userDictionary;
    }
    

    private static async Task GradingModel4ToCreateNextModels(
        ModelFourFiveSixMessage                   model,
        ICollection<UserGradingRequest>    listUsersGraded,
        IAccountServiceAdapter?            accountServiceAdapter,
        IWalletRepository                  walletRepository,
        Dictionary<int, decimal>           userDictionary,
        ICollection<LeaderBoardModel5>     leaderBoardModel5,
        ICollection<LeaderBoardModel6>     leaderBoardModel6,
        ICollection<UserBinaryInformation> resultPoints)
    {
        var resultOldPoints = await walletRepository!.GetUserModelFour(userDictionary.Select(x => x.Key).ToArray());
        
        foreach (var user in userDictionary)
        {
            var     oldPoints      = resultOldPoints.Where(x => x.AffiliateId == user.Key).ToList();
            var     oldLeftPoints  = oldPoints.Sum(x => x.CreditLeft - x.DebitLeft);
            var     oldRightPoints = oldPoints.Sum(x => x.CreditRight - x.DebitRight);
            decimal payment;
            var     leftPoints            = 0m;
            var     rightPoints           = 0m;
            var     userPointsInformation = resultPoints.Where(x => x.UserId == user.Key)!.FirstOrDefault();
            var     userInformation       = listUsersGraded.First(x => x.AffiliateId == user.Key);

            if (userPointsInformation is not null)
            {
                leftPoints  = oldLeftPoints + userPointsInformation.PointsLeft;
                rightPoints = oldRightPoints + userPointsInformation.PointsRight;
            }

            if (leftPoints == rightPoints && leftPoints > 0)
            {
                payment        = leftPoints;
                var points = payment + (decimal)userInformation.Commissions;
                await CreditModel4(
                    walletRepository,
                    payment,
                    userPointsInformation!.PointsLeft,
                    userPointsInformation.PointsRight,
                    userInformation);
                
                var grading = await GradingModelFiveSix(model, accountServiceAdapter, walletRepository, payment, userInformation, user);

                if (grading is null)
                    continue;
                
                if (grading.ScopeLevel == Constants.CustomerModel5Scope)
                {
                    leaderBoardModel5.Add(new LeaderBoardModel5
                    {
                        AffiliateId = user.Key,
                        Amount      = points,
                        UserName    = userInformation.UserName,
                        CreatedAt   = DateTime.Now
                    });
                }

                if (grading.ScopeLevel >= Constants.CustomerModel6Scope[0])
                {
                    leaderBoardModel6.Add(new LeaderBoardModel6
                    {
                        AffiliateId = user.Key,
                        Amount      = points,
                        UserName    = userInformation.UserName,
                        CreatedAt   = DateTime.Now
                    });
                }
                
            }
            else if (leftPoints > 0 || rightPoints > 0)
            {
                payment                  = leftPoints < rightPoints ? leftPoints : rightPoints;
                var points = payment + (decimal)userInformation.Commissions;
                await CreditModel4(
                    walletRepository,
                    payment,
                    userPointsInformation!.PointsLeft,
                    userPointsInformation.PointsRight,
                    userInformation);
                
                var grading = await GradingModelFiveSix(model, accountServiceAdapter, walletRepository, payment, userInformation, user);
                
                if (grading is null)
                    continue;
                
                if (grading.ScopeLevel == Constants.CustomerModel5Scope)
                {
                    leaderBoardModel5.Add(new LeaderBoardModel5
                    {
                        AffiliateId = user.Key,
                        Amount      = points,
                        UserName    = userInformation.UserName,
                        CreatedAt   = DateTime.Now
                    });
                }

                if (grading.ScopeLevel >= Constants.CustomerModel6Scope[0])
                {
                    leaderBoardModel6.Add(new LeaderBoardModel6
                    {
                        AffiliateId = user.Key,
                        Amount      = points,
                        UserName    = userInformation.UserName,
                        CreatedAt   = DateTime.Now
                    });
                }
            }
            else
                await walletRepository.TransactionPoints(
                    user.Key, 0, 0,
                    userPointsInformation!.PointsLeft,
                    userPointsInformation.PointsRight);
            
        }
        
    }

    private static void AddUsersLeaderBoard5(
        ICollection<LeaderBoardModel5> leaderBoardModel5,
        ICollection<LeaderBoardModel6> leaderBoardModel6,
        GradingDto                     grading,
        KeyValuePair<int, decimal>     user,
        decimal                        points,
        UserGradingRequest             userInformation)
    {
        if (grading.ScopeLevel == Constants.CustomerModel5Scope)
        {
            leaderBoardModel5.Add(new LeaderBoardModel5
            {
                AffiliateId = user.Key,
                Amount      = points,
                UserName    = userInformation.UserName,
                CreatedAt   = DateTime.Now
            });
        }

        if (grading.ScopeLevel >= Constants.CustomerModel6Scope[0])
        {
            leaderBoardModel6.Add(new LeaderBoardModel6
            {
                AffiliateId = user.Key,
                Amount      = points,
                UserName    = userInformation.UserName,
                CreatedAt   = DateTime.Now
            });
        }
    }

    

    private static async Task<GradingDto?> GradingModelFiveSix(
        ModelFourFiveSixMessage           model,
        IAccountServiceAdapter?    accountServiceAdapter,
        IWalletRepository          walletRepository,
        decimal                    payment,
        UserGradingRequest         userInformation,
        KeyValuePair<int, decimal> user)
    {
        var pointsModel = payment + (decimal)userInformation.Commissions;


        var grading = model.Gradings.Where(x
                => x.VolumePoints < pointsModel && userInformation.Grading!.Id < x.Id && x.ScopeLevel > Constants.CustomerModel4Scope[1])
            .MaxBy(o => o.ScopeLevel);
        
        if (grading is null)
            return null;
        
        await CreateDebitPayment(
            walletRepository,
            user.Key,
            userInformation.UserName,
            grading!.PersonalPurchases,
            string.Format(Constants.ConceptCommissionModelSixPayment, grading.PersonalPurchases, grading.Name),
            WalletConceptType.model_six_payment.ToString());
        await accountServiceAdapter!.UpdateGradingByUser(user.Key, grading.Id);

        return grading;
    }

    private static async Task CreditModel4(
        IWalletRepository              walletRepository,
        decimal                        payment,
        decimal                        creditPointLeft,
        decimal                        creditPointRight,
        UserGradingRequest             userInformation)
    {
        
        await CreateCreditPayment(
            walletRepository,
            userInformation.AffiliateId,
            userInformation.UserName,
            (double)payment,
            string.Format(Constants.ConceptCommissionBinaryPayment, payment, userInformation.Grading!.Name),
            WalletConceptType.model_four_payment.ToString()
        );

        await walletRepository.TransactionPoints(
            userInformation.AffiliateId, payment, payment,
            creditPointLeft,
            creditPointRight);
    }

    private static Task CreateDebitPayment(
        IWalletRepository walletRepository,
        int               userId,
        string            userName,
        decimal           payment,
        string            concept,
        string            conceptType)
    {
        if (payment <= 0)
            return Task.CompletedTask;
        
        // return Task.CompletedTask;

        var creditTransaction = new DebitTransactionRequest
        {
            AffiliateId       = userId,
            AffiliateUserName = userName,
            Debit             = payment,
            PaymentMethod     = "Billetera",
            UserId            = Constants.AdminUserId,
            Concept           = concept,
            AdminUserName     = Constants.AdminEcosystemUserName,
            ConceptType       = conceptType
        };
        return walletRepository.DebitEcoPoolTransactionSP(creditTransaction);
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
}