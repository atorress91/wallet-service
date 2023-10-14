using System.Text.Json;
using Confluent.Kafka;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using WalletService.Core.Kafka.Messages;
using WalletService.Data.Adapters.IAdapters;
using WalletService.Data.Repositories.IRepositories;
using WalletService.Models.Configuration;
using WalletService.Models.Constants;
using WalletService.Models.Enums;
using WalletService.Models.Requests.UserGradingRequest;
using WalletService.Models.Requests.WalletRequest;

namespace WalletService.Core.Kafka.Consumers;

public class ProcessFourModelConsumer : BaseKafkaConsumer
{
    public ProcessFourModelConsumer(
        ConsumerSettings         consumerSettings,
        ApplicationConfiguration configuration,
        ILogger                  logger,
        IServiceScopeFactory     serviceScopeFactory
    ) : base(consumerSettings, configuration, logger, serviceScopeFactory)
    {
    }

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

        var listUsersGraded = model.UserGradings.Where(x => x.Grading is { Id: > 1 }).ToList();

        if (listUsersGraded is { Count: 0 })
            return false;

        var dictionary = await ResultGradingProcess(model, listUsersGraded, walletRepository, accountServiceAdapter);
        await BinaryProcess(model, listUsersGraded, accountServiceAdapter, walletRepository, dictionary);

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
            if (item.Grading!.ScopeLevel > 2)
            {
                var firstGrading = model.Gradings.First(x => x.ScopeLevel == 2);
                if (walletRepository is not null)
                    await CreateDebitPayment(
                        walletRepository,
                        item.AffiliateId,
                        item.UserName,
                        firstGrading.PersonalPurchases,
                        string.Format(Constants.ConceptRecurringPayment, firstGrading.PersonalPurchases, firstGrading.Name),
                        WalletConceptType.recurring_payment.ToString());

                userDictionary[item.AffiliateId] = firstGrading.PurchasesNetwork;

                if (walletRepository is not null)
                    await CreateDebitPayment(
                        walletRepository,
                        item.AffiliateId,
                        item.UserName,
                        item.Grading.PersonalPurchases,
                        string.Format(Constants.ConceptRecurringPayment, item.Grading.PersonalPurchases, item.Grading.Name),
                        WalletConceptType.recurring_payment.ToString());
                // await accountServiceAdapter!.UpdateGradingByUser(item.AffiliateId, item.Grading.Id);
            }
            else
            {
                if (walletRepository is not null)
                    await CreateDebitPayment(
                        walletRepository,
                        item.AffiliateId,
                        item.UserName,
                        item.Grading.PersonalPurchases,
                        string.Format(Constants.ConceptRecurringPayment, item.Grading.PersonalPurchases, item.Grading.Name),
                        WalletConceptType.recurring_payment.ToString()
                    );
                userDictionary[item.AffiliateId] = item.Grading.PurchasesNetwork;
                // await accountServiceAdapter!.UpdateGradingByUser(item.AffiliateId, item.Grading.Id);
            }

        return userDictionary;
    }

    private static async Task BinaryProcess(
        ModelFourMessage                        model,
        IReadOnlyCollection<UserGradingRequest> listUsersGraded,
        IAccountServiceAdapter?                 accountServiceAdapter,
        IWalletRepository?                      walletRepository,
        Dictionary<int, decimal>                userDictionary)
    {
        try
        {
            var resultPoints    = await accountServiceAdapter!.CalculatePointPerUser(userDictionary);
            var resultOldPoints = await walletRepository!.GetUserModelFour(userDictionary.Select(x => x.Key).ToArray());

            foreach (var user in userDictionary)
            {
                var oldPoints              = resultOldPoints.Where(x => x.AffiliateId == user.Key).ToList();
                var leftPoints             = oldPoints.Sum(x => x.CreditLeft - x.DebitLeft);
                var rightPoints            = oldPoints.Sum(x => x.CreditRight - x.DebitRight);
                var userPointsInformation  = resultPoints.Where(x => x.UserId == user.Key)!.FirstOrDefault();
                var userInformation        = listUsersGraded.First(x => x.AffiliateId == user.Key);
                if (userPointsInformation is not null)
                {
                    leftPoints  += userPointsInformation.PointsLeft;
                    rightPoints += userPointsInformation.PointsRight;
                }

                if (leftPoints == rightPoints && leftPoints > 0)
                {
                    if (!(leftPoints > 0))
                        continue;

                    var grading = model.Gradings.Where(x
                            => x.VolumePoints < leftPoints && userInformation.Grading!.Id > x.Id).OrderDescending()
                        .FirstOrDefault();
                    if (grading is null)
                    {
                        await walletRepository.TransactionPoints(
                            user.Key, 0, 0,
                            userPointsInformation!.PointsLeft,
                            userPointsInformation.PointsRight);

                        continue;
                    }

                    await CreateDebitPayment(
                        walletRepository,
                        user.Key,
                        userInformation.UserName,
                        grading!.PersonalPurchases,
                        string.Format(Constants.ConceptBinaryPayment, grading.PersonalPurchases, grading.Name),
                        WalletConceptType.recurring_payment.ToString()
                    );
                    await accountServiceAdapter!.UpdateGradingByUser(user.Key, grading.Id);

                    await CreateCreditPayment(
                        walletRepository,
                        user.Key,
                        userInformation.UserName,
                        (double)leftPoints,
                        string.Format(Constants.ConceptCommissionBinaryPayment, leftPoints, grading.Name),
                        WalletConceptType.recurring_payment.ToString()
                    );

                    await walletRepository.TransactionPoints(
                        user.Key, leftPoints, leftPoints, 0m, 0m);
                }
                else if (leftPoints > 0 || rightPoints > 0)
                {
                    decimal payment;
                    decimal resultRight;
                    decimal resultLeft;
                    if (leftPoints < rightPoints)
                    {
                        payment     = leftPoints;
                        resultRight = rightPoints - leftPoints;
                        resultLeft  = leftPoints;
                    }
                    else
                    {
                        payment     = leftPoints;
                        resultLeft  = leftPoints - rightPoints;
                        resultRight = rightPoints;
                    }

                    if (payment <= 0)
                        continue;

                    var grading = model.Gradings.Where(x
                            => x.VolumePoints < payment && userInformation.Grading!.Id > x.Id)
                        .OrderDescending()
                        .FirstOrDefault();

                    if (grading is null)
                    {
                        await walletRepository.TransactionPoints(
                            user.Key, 0, 0,
                            userPointsInformation!.PointsLeft,
                            userPointsInformation.PointsRight);

                        continue;
                    }

                    await CreateDebitPayment(
                        walletRepository,
                        user.Key,
                        userInformation.UserName,
                        grading.PersonalPurchases,
                        string.Format(Constants.ConceptBinaryPayment, grading.PersonalPurchases, grading.Name),
                        WalletConceptType.recurring_payment.ToString()
                    );

                    await accountServiceAdapter!.UpdateGradingByUser(user.Key, grading.Id);

                    await CreateCreditPayment(
                        walletRepository,
                        user.Key,
                        userInformation.UserName,
                        (double)payment,
                        string.Format(Constants.ConceptCommissionBinaryPayment, payment, grading.Name),
                        WalletConceptType.recurring_payment.ToString()
                    );

                    await walletRepository.TransactionPoints(
                        user.Key,
                        resultLeft,
                        resultRight,
                        0m,
                        0m);
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
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
        // return walletRepository.DebitEcoPoolTransactionSP(creditTransaction);
        return Task.CompletedTask;
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