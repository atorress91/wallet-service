using System.Text.Json;
using Confluent.Kafka;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using WalletService.Core.Kafka.Messages;
using WalletService.Data.Repositories.IRepositories;
using WalletService.Models.Configuration;
using WalletService.Models.Requests.WalletRequest;
using WalletService.Utility.Extensions;

namespace WalletService.Core.Kafka.Consumers;

public class ProcessEcoPoolWithInConsumer : BaseKafkaConsumer
{
    public ProcessEcoPoolWithInConsumer(
        ConsumerSettings         consumerSettings,
        ApplicationConfiguration configuration,
        ILogger                  logger,
        IServiceScopeFactory     serviceScopeFactory
    ) : base(consumerSettings, configuration, logger, serviceScopeFactory)
    {
    }

    protected override Task<bool> OnMessage(ConsumeResult<Ignore, string> e)
    {
        var message = JsonSerializer.Deserialize<EcoPoolProcessMessage>(e.Message.Value);
        try
        {
            Logger.LogInformation("[ProcessEcoPoolWithInConsumer] OnMessage | Init");
            return message is not null ? EcoPoolProcess(message) : Task.FromResult(false);
        }
        catch (Exception ex)
        {
            Logger.LogError($"[ProcessEcoPoolWithInConsumer] ERROR | Exception ${ex.ToJsonString()}");
            return Task.FromResult(false);
        }
        finally
        {
            Logger.LogInformation("[ProcessEcoPoolWithInConsumer] OnMessage | End");
        }
    }

    private async Task<bool> EcoPoolProcess(EcoPoolProcessMessage message)
    {
        using var scope            = ServiceScopeFactory.CreateScope();
        var       walletRepository = scope.ServiceProvider.GetService<IWalletRepository>();

        var ecoPoolsType = new List<EcoPoolsType>();
        var levelsType   = new List<LevelsType>();

        var request = new EcoPoolTransactionRequest
        {
            EcoPoolConfigurationId  = message.Configuration.Id,
            CompanyPercentageLevels = message.Configuration.CompanyPercentageLevels.ToDecimal(),
            CompanyPercentage       = message.Configuration.CompanyPercentage.ToDecimal(),
            EcoPoolPercentage       = message.Configuration.EcoPoolPercentage.ToDecimal(),
            Points                  = message.Points,
            Case                    = message.Configuration.Case,
            TotalPercentageLevels   = message.Configuration.Levels.Sum(x => x.Percentage)
        };
        Logger.LogInformation($"[ProcessEcoPoolWithInConsumer] | EcoPoolProcess | Data: {request.ToJsonString()}");

        foreach (var pool in message.Pools)
        {
            var affiliate = message.ListResultAccounts.FirstOrDefault(x => x.Id == pool.Invoice.AffiliateId);
            var product   = message.ListResultProducts.FirstOrDefault(x => x.Id == pool.ProductId);
            var daysDelay = product?.DaysWait ?? 0;

            if (affiliate is null)
            {
                Logger.LogWarning($"[ProcessEcoPoolWithInConsumer] | EcoPoolProcess | Affiliate: {affiliate.ToJsonString()}");
                continue;
            }

            if (pool.Invoice.Date is null)
            {
                Logger.LogWarning($"[ProcessEcoPoolWithInConsumer] | EcoPoolProcess | Date wrong");
                continue;
            }

            var datePoolTemp = pool.Invoice.Date!.Value.AddDays(daysDelay);
            var firstDate    = new DateTime(datePoolTemp.Year, datePoolTemp.Month, datePoolTemp.Day, 00, 00, 00);
            var countDays    = message.EndDate.Subtract(firstDate).Days + 1;
            var daysInMonth  = message.EndDate.Subtract(message.StarDate).Days + 1;

            var levelsMapped = affiliate.FamilyTree.Select(s => new LevelsType
            {
                Percentage    = message.Configuration.Levels.FirstOrDefault(x => x.Level == s.Level)!.Percentage,
                Level         = s.Level,
                PoolId        = pool.Id,
                AffiliateId   = s.Id,
                AffiliateName = s.UserName,
                Side          = s.Side
            }).ToList();

            var productName = product?.Name ?? string.Empty;
            levelsType.AddRange(levelsMapped);
            ecoPoolsType.Add(new EcoPoolsType
            {
                AffiliateId       = affiliate.Id,
                AffiliateUserName = affiliate.UserName,
                PoolId            = pool.Id,
                CountDays         = countDays,
                DaysInMonth       = daysInMonth,
                Amount            = pool.BaseAmount,
                LastDayDate       = message.Configuration.DateEnd,
                PaymentDate       = firstDate,
                ProductExternalId = pool.ProductId,
                ProductName       = productName
            });
        }

        request.LevelsType   = levelsType;
        request.EcoPoolsType = ecoPoolsType;
        await walletRepository!.CreateEcoPoolSP(request);
        Logger.LogInformation($"[ProcessEcoPoolWithInConsumer] | EcoPoolProcess | Batch Completed");

        return true;
    }
}