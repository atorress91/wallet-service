using Confluent.Kafka;
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
            var message = e.Message.Value.ToJsonObject<ModelFourFiveSixMessage>();
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
        try
        {
            using var scope                 = ServiceScopeFactory.CreateScope();
            var       walletRepository      = scope.ServiceProvider.GetService<IWalletRepository>();
            var       accountServiceAdapter = scope.ServiceProvider.GetService<IAccountServiceAdapter>();

            var listUsersGraded = model.UserGradings.Where(x => x.Grading!.ScopeLevel >= Constants.CustomerModel4Scope[0]).ToList();

            if (listUsersGraded is { Count: 0 })
                return false;

            listUsersGraded = listUsersGraded.OrderByDescending(x => x.Commissions).ToList();
            var dictionary      = await DebitModelFourFiveProcess(model, listUsersGraded, walletRepository, accountServiceAdapter);
        
            
            var treeUsersInformation = await LeaderBoardModelFourProcess(accountServiceAdapter, dictionary);

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

            
            
            await LeaderBoardModelFiveProcess(leaderBoardModel5, accountServiceAdapter, walletRepository!, model.Gradings);
            await LeaderBoardModelSixProcess(leaderBoardModel6, accountServiceAdapter, walletRepository!, model.Gradings);

            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }

    }

    private static async Task LeaderBoardModelFiveProcess(
        List<LeaderBoardModel5>         leaderBoardModel5,
        IAccountServiceAdapter?         accountServiceAdapter,
        IWalletRepository               walletRepository,
        ICollection<GradingDto> gradings)
    {

        leaderBoardModel5 = leaderBoardModel5.OrderModel5();
        await accountServiceAdapter!.DeleteTreeModel5();
        await accountServiceAdapter.AddTreeModel5(leaderBoardModel5);

        foreach (var item in leaderBoardModel5)
        {
            var paymentGroup = leaderBoardModel5.Where(x => x.FatherModel5 == item.AffiliateId).ToList();
            if(paymentGroup is {Count:0})
                continue;
            
            var grading     = gradings.First(x => x.Id == item.GradingId);
            var totalAmount = paymentGroup.Count * grading.PurchasesNetwork;
            await CreateCreditPayment(
                walletRepository,
                item.AffiliateId,
                item.UserName,
                (double)totalAmount,
                string.Format(Constants.ConceptCommissionModelFivePayment, totalAmount, grading.Name),
                WalletConceptType.model_four_payment.ToString()
            );
        }
    }
    
    private static async Task LeaderBoardModelSixProcess(
        List<LeaderBoardModel6> leaderBoardModel6,
        IAccountServiceAdapter? accountServiceAdapter,
        IWalletRepository       walletRepository,
        ICollection<GradingDto> gradings)
    {
        leaderBoardModel6 = leaderBoardModel6.OrderModel6();
        await accountServiceAdapter!.DeleteTreeModel6();
        await accountServiceAdapter.AddTreeModel6(leaderBoardModel6);
        
        foreach (var item in leaderBoardModel6)
        {
            var paymentGroup = leaderBoardModel6.Where(x => x.FatherModel6 == item.AffiliateId).ToList();
            if(paymentGroup is {Count:0})
                continue;
            
            var grading     = gradings.First(x => x.Id == item.GradingId);
            var totalAmount = paymentGroup.Count * grading.PurchasesNetwork;
            await CreateCreditPayment(
                walletRepository,
                item.AffiliateId,
                item.UserName,
                (double)totalAmount,
                string.Format(Constants.ConceptCommissionModelSixPayment, totalAmount, grading.Name),
                WalletConceptType.model_four_payment.ToString()
            );
        }
    }

    private static async Task<ICollection<UserBinaryInformation>> LeaderBoardModelFourProcess(
        IAccountServiceAdapter? accountServiceAdapter, 
        Dictionary<int, decimal> dictionary)
    {
        var result = new List<UserBinaryInformation>();

        var response = await accountServiceAdapter!.GetTreeModel4(dictionary);
        
        if (string.IsNullOrEmpty(response.Content))
            return result;

        var userBinaryResponse = response.Content.ToJsonObject<UserBinaryResponse>();
        return userBinaryResponse!.Data;
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
            if (item.Grading!.ScopeLevel >= 1)
            {
                if (walletRepository is not null)
                    await CreateDebitPayment(
                        walletRepository,
                        item.AffiliateId,
                        item.UserName,
                        item.Grading.PersonalPurchases,
                        string.Format(Constants.ConceptBinaryPayment, item.Grading.PersonalPurchases, item.Grading.Name),
                        WalletConceptType.model_four_payment.ToString());

                userDictionary[item.AffiliateId] = item.Grading.PurchasesNetwork;
                
                var description = item.Grading!.ScopeLevel > Constants.CustomerModel5Scope ? Constants.ConceptModelSixPayment : Constants.ConceptModelFivePayment;
                if (walletRepository is not null && item.Grading!.ScopeLevel >= Constants.CustomerModel5Scope)
                    await CreateDebitPayment(
                        walletRepository,
                        item.AffiliateId,
                        item.UserName,
                        item.Grading.PersonalPurchases,
                        string.Format(description, item.Grading.PersonalPurchases, item.Grading.Name),
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
        var resultOldPoints   = await walletRepository!.GetUserModelFour(userDictionary.Select(x => x.Key).ToArray());
        
        var userIds           = listUsersGraded!.Select(x => x.AffiliateId).ToArray();
        var responseCondition = await accountServiceAdapter!.GetHave2Children(userIds);
        
        if (string.IsNullOrEmpty(responseCondition.Content))
            return;
        
        var resultUserIds = responseCondition.Content.ToJsonObject<int[]>();
        
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
                var points         = payment + (decimal)userInformation.Commissions;
                var hasTwoChildren = resultUserIds!.Contains(userInformation.AffiliateId);
                await CreditModel4(
                    walletRepository,
                    payment,
                    userPointsInformation!.PointsLeft,
                    userPointsInformation.PointsRight,
                    userInformation,
                    hasTwoChildren);
                
                var grading = await GradingModelFiveSix(model, accountServiceAdapter, walletRepository, payment, userInformation, user);

                if (grading is null)
                    continue;
                
                if (grading.ScopeLevel >= Constants.CustomerModel5Scope)
                {
                    leaderBoardModel5.Add(new LeaderBoardModel5
                    {
                        AffiliateId = user.Key,
                        Amount      = points,
                        UserName    = userInformation.UserName,
                        CreatedAt   = DateTime.Now,
                        GradingId   = grading.Id,
                        UserCreatedAt = userInformation.UserCreatedAt
                    });
                }

                if (grading.ScopeLevel >= Constants.CustomerModel6Scope[0])
                {
                    leaderBoardModel6.Add(new LeaderBoardModel6
                    {
                        AffiliateId   = user.Key,
                        Amount        = points,
                        UserName      = userInformation.UserName,
                        CreatedAt     = DateTime.Now,
                        GradingId     = grading.Id,
                        UserCreatedAt = userInformation.UserCreatedAt,
                    });
                }
                
            }
            else if (leftPoints > 0 || rightPoints > 0)
            {
                payment                  = leftPoints < rightPoints ? leftPoints : rightPoints;
                var points         = payment + (decimal)userInformation.Commissions;
                var hasTwoChildren = resultUserIds!.Contains(userInformation.AffiliateId);
                await CreditModel4(
                    walletRepository,
                    payment,
                    userPointsInformation!.PointsLeft,
                    userPointsInformation.PointsRight,
                    userInformation,
                    hasTwoChildren);
                
                var grading = await GradingModelFiveSix(model, accountServiceAdapter, walletRepository, payment, userInformation, user);
                
                if (grading is null)
                    continue;
                
                if (grading.ScopeLevel >= Constants.CustomerModel5Scope)
                {
                    leaderBoardModel5.Add(new LeaderBoardModel5
                    {
                        AffiliateId = user.Key,
                        Amount      = points,
                        UserName    = userInformation.UserName,
                        CreatedAt   = DateTime.Now,
                        GradingId     = grading.Id,
                        UserCreatedAt = userInformation.UserCreatedAt
                    });
                }

                if (grading.ScopeLevel >= Constants.CustomerModel6Scope[0])
                {
                    leaderBoardModel6.Add(new LeaderBoardModel6
                    {
                        AffiliateId   = user.Key,
                        Amount        = points,
                        UserName      = userInformation.UserName,
                        CreatedAt     = DateTime.Now,
                        GradingId     = grading.Id,
                        UserCreatedAt = userInformation.UserCreatedAt,
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
                => x.VolumePoints < pointsModel && x.ScopeLevel > Constants.CustomerModel4Scope[2])
            .MaxBy(o => o.ScopeLevel);
        
        if (grading is null)
            return null;
        
        if (grading.ScopeLevel <= Constants.CustomerModel5Scope)
            return grading;
        
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
        UserGradingRequest             userInformation,
        bool                           hasTwoChildren)
    {
        hasTwoChildren = true;
        if (hasTwoChildren)
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
        return walletRepository.DebitEcoPoolTransactionSp(creditTransaction);
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
}