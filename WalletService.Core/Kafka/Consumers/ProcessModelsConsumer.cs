using System.Security.Cryptography.X509Certificates;
using System.Text.Json;
using Confluent.Kafka;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using WalletService.Core.Kafka.Messages;
using WalletService.Data.Adapters.IAdapters;
using WalletService.Data.Database.Models;
using WalletService.Data.Repositories.IRepositories;
using WalletService.Models.Configuration;
using WalletService.Models.Constants;
using WalletService.Models.DTO.AffiliateInformation;
using WalletService.Models.Enums;
using WalletService.Models.Requests.UserGradingRequest;
using WalletService.Models.Requests.WalletRequest;
using WalletService.Utility.Extensions;

namespace WalletService.Core.Kafka.Consumers;

public class ProcessModelsConsumer : BaseKafkaConsumer
{
    public ProcessModelsConsumer(
        ConsumerSettings         consumerSettings,
        ApplicationConfiguration configuration,
        ILogger                  logger,
        IServiceScopeFactory     serviceScopeFactory
    ) : base(consumerSettings, configuration, logger, serviceScopeFactory) { }

    protected override Task<bool> OnMessage(ConsumeResult<Ignore, string> e)
    {
        try
        {
            var message = JsonSerializer.Deserialize<ModelFourMessage>(e.Message.Value);
            Logger.LogInformation("[ProcessFourModel] OnMessage | Init");
            return message is not null ? Process(message) : Task.FromResult(false);
        }
        catch (Exception ex)
        {
            Logger.LogError("[ProcessFourModel] Error | Data: {Exception}", ex);
            return Task.FromResult(false);
        }
        finally
        {
            Logger.LogInformation("[ProcessFourModel] OnMessage | End");
        }
    }

    private async Task<bool> Process(ModelFourMessage model)
    {
        using var scope                 = ServiceScopeFactory.CreateScope();
        var       walletRepository      = scope.ServiceProvider.GetService<IWalletRepository>();
        var       accountServiceAdapter = scope.ServiceProvider.GetService<IAccountServiceAdapter>();
        var       leaderBoardRepository = scope.ServiceProvider.GetService<ILeaderBoardRepository>();

        var listUsersGraded = model.UserGradings.Where(x => x.Grading!.ScopeLevel >= Constants.CustomerModel4Scope[0]).ToList();

        if (listUsersGraded is { Count: 0 })
            return false;

        var dictionary = await ResultGradingProcess(model, listUsersGraded, walletRepository, accountServiceAdapter);
        await BinaryProcess(model, listUsersGraded, accountServiceAdapter, walletRepository, dictionary, leaderBoardRepository);

        return true;
    }

    private static async Task<Dictionary<int, decimal>> ResultGradingProcess(
        ModelFourMessage         model,
        List<UserGradingRequest> listUsersGraded,
        IWalletRepository?       walletRepository,
        IAccountServiceAdapter?  accountServiceAdapter)
    {
        var userDictionary = listUsersGraded.ToDictionary(x => x.AffiliateId, _ => 0m);
        foreach (var item in listUsersGraded)
        {
            if (item.Grading!.ScopeLevel >= Constants.CustomerModel5Scope)
            {
                var firstGrading = model.Gradings.First(x => x.ScopeLevel == Constants.CustomerModel4Scope[1]);
                if (walletRepository is not null)
                    await CreateDebitPayment(
                        walletRepository,
                        item.AffiliateId,
                        item.UserName,
                        firstGrading.PersonalPurchases,
                        string.Format(Constants.ConceptBinaryPayment, firstGrading.PersonalPurchases, firstGrading.Name),
                        WalletConceptType.model_four_payment.ToString());

                userDictionary[item.AffiliateId] = firstGrading.PurchasesNetwork;
                
                if (walletRepository is not null)
                    await CreateDebitPayment(
                        walletRepository,
                        item.AffiliateId,
                        item.UserName,
                        item.Grading.PersonalPurchases,
                        string.Format(Constants.ConceptModelFivePayment, item.Grading.PersonalPurchases, item.Grading.Name),
                        WalletConceptType.model_five_payment.ToString());
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
            }
            await accountServiceAdapter!.UpdateGradingByUser(item.AffiliateId, item.Grading.Id);
        }

        return userDictionary;
    }

    private static async Task BinaryProcess(
        ModelFourMessage                        model,
        IReadOnlyCollection<UserGradingRequest> listUsersGraded,
        IAccountServiceAdapter?                 accountServiceAdapter,
        IWalletRepository?                      walletRepository,
        Dictionary<int, decimal>                userDictionary,
        ILeaderBoardRepository?                 leaderBoardRepository)
     {
        try
        {
            var resultPoints      = await accountServiceAdapter!.CalculatePointPerUser(userDictionary);
            var resultOldPoints   = await walletRepository!.GetUserModelFour(userDictionary.Select(x => x.Key).ToArray());
            var leaderBoardModel5 = new List<LeaderBoardModel5>();
            var leaderBoardModel6 = new List<LeaderBoardModel6>();
            var paymentTotal      = 0m;
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
                    payment      =  leftPoints;
                    paymentTotal += payment;
                    await SecondGradingProcess(
                        model,
                        accountServiceAdapter,
                        walletRepository,
                        payment,
                        userPointsInformation!.PointsLeft,
                        userPointsInformation.PointsRight,
                        userInformation,
                        user,
                        leaderBoardModel5,
                        leaderBoardModel6);
                }
                
                else if (leftPoints > 0 || rightPoints > 0)
                { 
                    payment      =  leftPoints < rightPoints ? leftPoints : rightPoints;
                    paymentTotal += payment;
                    await SecondGradingProcess(
                        model,
                        accountServiceAdapter,
                        walletRepository,
                        payment,
                        userPointsInformation!.PointsLeft,
                        userPointsInformation.PointsRight,
                        userInformation,
                        user,
                        leaderBoardModel5,
                        leaderBoardModel6);
                }
                else
                    await walletRepository.TransactionPoints(
                        user.Key, 0, 0,
                        userPointsInformation!.PointsLeft,
                        userPointsInformation.PointsRight);
            }
            

            if (leaderBoardModel5 is { Count: > 0 })
            {
                leaderBoardModel5 = leaderBoardModel5.OrderByDescending(x => x.Amount).ToList();

                for (var i = 0; i < leaderBoardModel5.Count; i++)
                {
                    
                    var row = i / Constants.MatrixModel5[0];
                    var col = i % Constants.MatrixModel5[1];
                    leaderBoardModel5[i].MatrixPosition  = col is 0 ? "{head}" : new[] { row, col }.ToJsonString();
                    leaderBoardModel5[i].GradingPosition = i;

                    if (leaderBoardModel5[i].MatrixPosition is not "{head}")
                        continue;
                    
                    var count = leaderBoardModel5.Count >= Constants.MatrixModel5[0] 
                        ? Constants.MatrixModel5[0] - 1 : leaderBoardModel5.Count - 1;
                    
                    var grading = model.Gradings.First(x => x.Id == leaderBoardModel5[i].GradingId);
                    var payment = grading.PurchasesNetwork * count;
                    await CreateCreditPayment(
                        walletRepository,
                        leaderBoardModel5[i].AffiliateId,
                        leaderBoardModel5[i].UserName,
                        (double)payment,
                        string.Format(Constants.ConceptCommissionModelFivePayment, payment, grading.Name), 
                        WalletConceptType.model_six_payment.ToString());
                }
                
                await leaderBoardRepository!.CleanLeaderBoardModel5();
                if(leaderBoardModel5 is {Count:>0})
                    await leaderBoardRepository.AddCustomersToModel5(leaderBoardModel5);
            }

            if (leaderBoardModel6 is { Count: > 0 })
            {
                leaderBoardModel6 = leaderBoardModel6.OrderByDescending(x => x.Amount).ToList();
                
                for (var i = 0; i < leaderBoardModel6.Count; i++)
                {
                    var row = i / Constants.MatrixModel6[0];
                    var col = i % Constants.MatrixModel6[1];
                    leaderBoardModel6[i].MatrixPosition  = col is 0 ? "{head}" : new[] { row, col }.ToJsonString();
                    leaderBoardModel6[i].GradingPosition = i;
                
                    var grading = model.Gradings.First(x => x.Id == leaderBoardModel6[i].GradingId);
                
                    if (leaderBoardModel6[i].MatrixPosition is not "{head}")
                        continue;
                    
                    var count = leaderBoardModel6.Count >= Constants.MatrixModel6[0] 
                        ? Constants.MatrixModel6[0] - 1 : leaderBoardModel6.Count - 1;
                
                    var payment = grading.PurchasesNetwork  * count;
                    if(leaderBoardModel6[i].MatrixPosition is "{head}")
                        await CreateCreditPayment(
                            walletRepository,
                            leaderBoardModel6[i].AffiliateId,
                            leaderBoardModel6[i].UserName,
                            (double)payment,
                            string.Format(Constants.ConceptCommissionModelSixPayment, payment, grading.Name), 
                            WalletConceptType.model_six_payment.ToString());
                    leaderBoardModel6.RemoveAt(i);
                }
                
                await leaderBoardRepository!.CleanLeaderBoardModel6();
                if(leaderBoardModel6 is {Count:>0})
                    await leaderBoardRepository.AddCustomersToModel6(leaderBoardModel6);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
     }
    
    private static async Task SecondGradingProcess(
        ModelFourMessage           model,
        IAccountServiceAdapter     accountServiceAdapter,
        IWalletRepository          walletRepository,
        decimal                    payment,
        decimal                    creditPointLeft,
        decimal                    creditPointRight,
        UserGradingRequest         userInformation,
        KeyValuePair<int, decimal> user,
        List<LeaderBoardModel5>    leaderBoardModel5,
        List<LeaderBoardModel6>    leaderBoardModel6)
    {
        var pointsModel = payment + (decimal)userInformation.Commissions;
        var grading     = userInformation.Grading;
        
        
        var gradingTemp = model.Gradings.Where(x
            => x.VolumePoints < pointsModel && userInformation.Grading!.Id < x.Id && x.ScopeLevel > Constants.CustomerModel4Scope[1])
            .MaxBy(o => o.ScopeLevel);

        
        if (gradingTemp is not null)
        {
            grading = gradingTemp;
            await CreateDebitPayment(
                walletRepository,
                user.Key,
                userInformation.UserName,
                grading!.PersonalPurchases,
                string.Format(Constants.ConceptCommissionModelSixPayment, grading.PersonalPurchases, grading.Name),
                WalletConceptType.model_six_payment.ToString());
            await accountServiceAdapter!.UpdateGradingByUser(user.Key, grading.Id);
        }
            
        
        await CreateCreditPayment(
            walletRepository,
            user.Key,
            userInformation.UserName,
            (double)payment,
            string.Format(Constants.ConceptCommissionBinaryPayment, payment, grading!.Name),
            WalletConceptType.model_four_payment.ToString()
        );

        await walletRepository.TransactionPoints(
            user.Key, payment, payment,
            creditPointLeft,
            creditPointRight);


        if (grading.ScopeLevel == Constants.CustomerModel5Scope)
            leaderBoardModel5.Add(new LeaderBoardModel5
            {
                AffiliateId     = userInformation.AffiliateId,
                Amount          = pointsModel,
                GradingPosition = 0,
                MatrixPosition  = string.Empty,
                CreatedAt       = DateTime.Now,
                UserName = userInformation.UserName,
                GradingId = grading.Id
            });
        if (Constants.CustomerModel6Scope.Contains(grading.ScopeLevel))
            leaderBoardModel6.Add(new LeaderBoardModel6
            {
                AffiliateId     = userInformation.AffiliateId,
                Amount          = pointsModel,
                GradingPosition = 0,
                MatrixPosition  = string.Empty,
                CreatedAt       = DateTime.Now,
                UserName        = userInformation.UserName,
                GradingId       = grading.Id
            });
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