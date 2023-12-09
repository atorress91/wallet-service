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

public class ProcessModelThreeWithOutConsumer : BaseKafkaConsumer
{
    public ProcessModelThreeWithOutConsumer(
        ConsumerSettings         consumerSettings,
        ApplicationConfiguration configuration,
        ILogger                  logger,
        IServiceScopeFactory     serviceScopeFactory
    ) : base(consumerSettings, configuration, logger, serviceScopeFactory) { }

    protected override Task<bool> OnMessage(ConsumeResult<Ignore, string> e)
    {
        var message = JsonSerializer.Deserialize<ModelThreeMessage>(e.Message.Value);
        try
        {
            Logger.LogInformation("[ProcessModelThreeWithOutConsumer] OnMessage | Init");
            return message is not null ? Process(message) : Task.FromResult(false);
        }
        catch (Exception ex)
        {
            Logger.LogError("[ProcessModelThreeWithOutConsumer] Error | Data: {Exception}", ex);
            return Task.FromResult(false);
        }
        finally
        {
            Logger.LogInformation("[ProcessModelThreeWithOutConsumer] OnMessage | End");
        }
    }

    private async Task<bool> Process(ModelThreeMessage message)
    {
        using var scope            = ServiceScopeFactory.CreateScope();
        var       walletRepository = scope.ServiceProvider.GetService<IWalletRepository>();

        var ecoPoolsType = new List<ModelThreeType>();
        var levelsType   = new List<ModelThreeLevelsType>();
        var request = new ModelThreeTransactionRequest
        {
            EcoPoolConfigurationId  = message.Configuration.Id,
            CompanyPercentageLevels = message.Configuration.CompanyPercentageLevels.ToDecimal(),
            CompanyPercentage       = message.Configuration.CompanyPercentage.ToDecimal(),
            EcoPoolPercentage       = message.Configuration.EcoPoolPercentage.ToDecimal(),
            Points                  = message.Points,
            Case                    = message.Configuration.Case,
            TotalPercentageLevels   = message.Configuration.Levels.Sum(x => x.Percentage)
        };
        Logger.LogInformation("[ProcessModelThreeWithOutConsumer] ProcessListOutsideMonth | Init");

        
        foreach (var pool in message.Pools)
        {
            var affiliate = message.ListResultAccounts.FirstOrDefault(x => x.Id == pool.Invoice.AffiliateId);
            var product   = message.ListResultProducts.FirstOrDefault(x => x.Id == pool.ProductId);

            if (affiliate is null)
            {
                Logger.LogWarning($"[ProcessModelThreeWithOutConsumer] | EcoPoolProcess | Affiliate: {affiliate.ToJsonString()}");
                continue;
            }
            if (pool.Invoice.Date is null)
            {
                Logger.LogWarning($"[ProcessModelThreeWithOutConsumer] | EcoPoolProcess | Date wrong");
                continue;
            }

            var datePoolTemp    = pool.Invoice.Date!.Value;
            var firstDayInMonth = new DateTime(datePoolTemp.Year, datePoolTemp.Month, 1);
            var daysInMonth     = DateTime.DaysInMonth(datePoolTemp.Year, datePoolTemp.Month);
            var lastDayInMonth  = new DateTime(datePoolTemp.Year, datePoolTemp.Month, daysInMonth, 23, 59, 59);

            var levelsMapped = affiliate.FamilyTree.Select(s => new ModelThreeLevelsType
            {
                Percentage    = message.Configuration.Levels.FirstOrDefault(x => x.Level == s.Level)!.Percentage,
                Level         = s.Level,
                AffiliateId   = s.Id,
                PoolId        = pool.Id,
                AffiliateName = s.UserName,
                Side          = s.Side
            }).ToList();

            var productName = product?.Name ?? string.Empty;
            levelsType.AddRange(levelsMapped);
            ecoPoolsType.Add(new ModelThreeType
            {
                AffiliateId       = affiliate.Id,
                AffiliateUserName = affiliate.UserName,
                PoolId            = pool.Id,
                CountDays         = daysInMonth,
                DaysInMonth       = daysInMonth,
                Amount            = pool.BaseAmount,
                LastDayDate       = lastDayInMonth,
                PaymentDate       = firstDayInMonth,
                ProductExternalId = pool.ProductId,
                ProductName       = productName
            });
        }

        request.LevelsType   = levelsType;
        request.EcoPoolsType = ecoPoolsType;

        await walletRepository!.CreateModelThreeSP(request);

        Logger.LogInformation($"[ProcessModelThreeWithOutConsumer] | EcoPoolProcess | Batch Completed");
        return true;
    }
}