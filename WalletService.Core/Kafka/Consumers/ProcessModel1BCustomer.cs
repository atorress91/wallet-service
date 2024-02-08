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

public class ProcessModel1BConsumer : BaseKafkaConsumer
{
    public ProcessModel1BConsumer(
        ConsumerSettings         consumerSettings,
        ApplicationConfiguration configuration,
        ILogger                  logger,
        IServiceScopeFactory     serviceScopeFactory
    ) : base(consumerSettings, configuration, logger, serviceScopeFactory) { }

    protected override Task<bool> OnMessage(ConsumeResult<Ignore, string> e)
    {
        var message = JsonSerializer.Deserialize<Model1BMessage>(e.Message.Value);
        try
        {
            Logger.LogInformation("[ProcessModel1BConsumer] OnMessage | Init");
            return message is not null ? Process(message) : Task.FromResult(false);
        }
        catch (Exception ex)
        {
            Logger.LogError($"[ProcessModel1BConsumer] ERROR | Exception ${ex.ToJsonString()}");
            return Task.FromResult(false);
        }
        finally
        {
            Logger.LogInformation("[ProcessModel1BConsumer] OnMessage | End");
        }
    }

    private async Task<bool> Process(Model1BMessage message)
    {
        using var scope            = ServiceScopeFactory.CreateScope();
        var       walletRepository = scope.ServiceProvider.GetService<IWalletRepository>();

        var ecoPoolsType = new List<Model1BType>();
        var levelsType   = new List<Model1BLevelsType>();

        var request = new Model1BTransactionRequest
        {
            EcoPoolConfigurationId  = message.Configuration.Id,
            CompanyPercentageLevels = message.Configuration.CompanyPercentageLevels.ToDecimal(),
            CompanyPercentage       = message.Configuration.CompanyPercentage.ToDecimal(),
            EcoPoolPercentage       = message.Configuration.EcoPoolPercentage.ToDecimal(),
            Points                  = message.Points,
            Case                    = message.Configuration.Case,
            TotalPercentageLevels   = message.Configuration.Levels.Sum(x => x.Percentage)
        };
        Logger.LogInformation($"[ProcessModel1BConsumer] | EcoPoolProcess | Data: {request.ToJsonString()}");

        CalculateModelTwoWithInMonth(message, levelsType, ecoPoolsType);
        CalculateModelTwoWithOutMonth(message, levelsType, ecoPoolsType);
        
        request.LevelsType   = levelsType;
        request.EcoPoolsType = ecoPoolsType;
        await walletRepository!.CreateModel1BSp(request);
        Logger.LogInformation($"[ProcessModel1BConsumer] | EcoPoolProcess | Batch Completed");

        return true;
    }
    
    private void CalculateModelTwoWithOutMonth(Model1BMessage message, List<Model1BLevelsType> levelsType, List<Model1BType> ecoPoolsType)
    {
        foreach (var pool in message.ItemWithOutMonth)
        {
            var affiliate = message.ListResultAccounts.FirstOrDefault(x => x.Id == pool.Invoice.AffiliateId);
            var product   = message.ListResultProducts.FirstOrDefault(x => x.Id == pool.ProductId);

            if (affiliate is null)
            {
                Logger.LogWarning($"[ProcessModel1BConsumer] | EcoPoolProcess | Affiliate: {affiliate.ToJsonString()}");
                continue;
            }

            if (pool.Invoice.Date is null)
            {
                Logger.LogWarning($"[ProcessModel1BConsumer] | EcoPoolProcess | Date wrong");
                continue;
            }

            var datePoolTemp    = pool.Invoice.Date!.Value;
            var firstDayInMonth = new DateTime(datePoolTemp.Year, datePoolTemp.Month, 1);
            var daysInMonth     = DateTime.DaysInMonth(datePoolTemp.Year, datePoolTemp.Month);
            var lastDayInMonth  = new DateTime(datePoolTemp.Year, datePoolTemp.Month, daysInMonth, 23, 59, 59);

            var levelsMapped = affiliate.FamilyTree.Select(s => new Model1BLevelsType
            {
                Percentage    = message.Configuration.Levels.FirstOrDefault(x => x.Level == s.Level)!.Percentage,
                Level         = s.Level,
                AffiliateId   = s.Id,
                PoolId        = pool.Id,
                AffiliateName = s.UserName,
                Side          = s.Side,
                UserCreatedAt = s.UserCreatedAt
            }).ToList();

            var productName = product?.Name ?? string.Empty;
            levelsType.AddRange(levelsMapped);
            ecoPoolsType.Add(new Model1BType
            {
                AffiliateId       = affiliate.Id,
                AffiliateUserName = affiliate.UserName,
                PoolId            = pool.Id,
                CountDays         = daysInMonth,
                DaysInMonth       = daysInMonth,
                Amount            = pool.BaseAmount!.Value,
                LastDayDate       = lastDayInMonth,
                PaymentDate       = firstDayInMonth,
                ProductExternalId = pool.ProductId,
                ProductName       = productName,
                UserCreatedAt     = affiliate.UserCreatedAt
            });
        }
    }

    private void CalculateModelTwoWithInMonth(Model1BMessage message, List<Model1BLevelsType> levelsType, List<Model1BType> ecoPoolsType)
    {
        foreach (var pool in message.ItemWithOutMonth)
        {
            var affiliate = message.ListResultAccounts.FirstOrDefault(x => x.Id == pool.Invoice.AffiliateId);
            var product   = message.ListResultProducts.FirstOrDefault(x => x.Id == pool.ProductId);
            var daysDelay = product?.DaysWait ?? 0;

            if (affiliate is null)
            {
                Logger.LogWarning($"[ProcessModel1BConsumer] | EcoPoolProcess | Affiliate: {affiliate.ToJsonString()}");
                continue;
            }

            if (pool.Invoice.Date is null)
            {
                Logger.LogWarning($"[ProcessModel1BConsumer] | EcoPoolProcess | Date wrong");
                continue;
            }

            var datePoolTemp = pool.Invoice.Date!.Value.AddDays(daysDelay);
            var firstDate    = new DateTime(datePoolTemp.Year, datePoolTemp.Month, datePoolTemp.Day, 00, 00, 00);
            var countDays    = message.EndDate.Subtract(firstDate).Days + 1;
            var daysInMonth  = message.EndDate.Subtract(message.StarDate).Days + 1;

            var levelsMapped = affiliate.FamilyTree.Select(s => new Model1BLevelsType
            {
                Percentage    = message.Configuration.Levels.FirstOrDefault(x => x.Level == s.Level)!.Percentage,
                Level         = s.Level,
                PoolId        = pool.Id,
                AffiliateId   = s.Id,
                AffiliateName = s.UserName,
                Side          = s.Side,
                UserCreatedAt = s.UserCreatedAt
            }).ToList();

            var productName = product?.Name ?? string.Empty;
            levelsType.AddRange(levelsMapped);
            ecoPoolsType.Add(new Model1BType
            {
                AffiliateId       = affiliate.Id,
                AffiliateUserName = affiliate.UserName,
                PoolId            = pool.Id,
                CountDays         = countDays,
                DaysInMonth       = daysInMonth,
                Amount            = pool.BaseAmount!.Value,
                LastDayDate       = message.Configuration.DateEnd,
                PaymentDate       = firstDate,
                ProductExternalId = pool.ProductId,
                ProductName       = productName,
                UserCreatedAt     = affiliate.UserCreatedAt
            });
        }
    }
}