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

public class ProcessModelTwoConsumer : BaseKafkaConsumer
{
      public ProcessModelTwoConsumer(
        ConsumerSettings         consumerSettings,
        ApplicationConfiguration configuration,
        ILogger                  logger,
        IServiceScopeFactory     serviceScopeFactory
    ) : base(consumerSettings, configuration, logger, serviceScopeFactory)
    {
    }

    protected override Task<bool> OnMessage(ConsumeResult<Ignore, string> e)
    {
        var message = JsonSerializer.Deserialize<ModelTwoMessage>(e.Message.Value);
        try
        {
            Logger.LogInformation("[ProcessModelTwoConsumer] OnMessage | Init");
            return message is not null ? Process(message) : Task.FromResult(false);
        }
        catch (Exception ex)
        {
            Logger.LogError($"[ProcessModelTwoConsumer] ERROR | Exception ${ex.ToJsonString()}");
            return Task.FromResult(false);
        }
        finally
        {
            Logger.LogInformation("[ProcessModelTwoConsumer] OnMessage | End");
        }
    }

    private async Task<bool> Process(ModelTwoMessage message)
    {
        using var scope            = ServiceScopeFactory.CreateScope();
        var       walletRepository = scope.ServiceProvider.GetService<IWalletRepository>();

        var ecoPoolsType = new List<ModelTwoType>();
        var levelsType   = new List<ModelTwoLevelsType>();

        var request = new ModelTwoTransactionRequest
        {
            EcoPoolConfigurationId = message.Configuration.Id,
            Percentage             = message.Configuration.PercentageModelTwo,
            Case                   = message.Configuration.Case,
            TotalPercentageLevels  = message.Configuration.Levels.Sum(x => x.Percentage)
        };
        
        Logger.LogInformation($"[ProcessModelTwoConsumer] | EcoPoolProcess | Data: {request.ToJsonString()}");

        foreach (var item in message.EducatedCourses)
        {
            var affiliate = message.ListResultAccounts.FirstOrDefault(x => x.Id == item.Invoice.AffiliateId);
            var product   = message.ListResultProducts.FirstOrDefault(x => x.Id == item.ProductId);
 
            if (affiliate is null)
            {
                Logger.LogWarning($"[ProcessModelTwoConsumer] | EcoPoolProcess | Affiliate: {affiliate.ToJsonString()}");
                continue;
            }

            if (item.Invoice.Date is null)
            {
                Logger.LogWarning($"[ProcessModelTwoConsumer] | EcoPoolProcess | Date wrong");
                continue;
            }
            
            var levelsMapped = affiliate.FamilyTree.Select(s => new ModelTwoLevelsType
            {
                Percentage    = message.Configuration.Levels.FirstOrDefault(x => x.Level == s.Level)!.Percentage,
                Level         = s.Level,
                PoolId        = item.Id,
                AffiliateId   = s.Id,
                AffiliateName = s.UserName,
                Side          = s.Side,
                UserCreatedAt = s.UserCreatedAt
            }).ToList();

            var productName = product?.Name ?? string.Empty;
            levelsType.AddRange(levelsMapped);
            ecoPoolsType.Add(new ModelTwoType
            {
                AffiliateId       = affiliate.Id,
                AffiliateUserName = affiliate.UserName,
                PoolId            = item.Id,
                Amount            = product!.ModelTwoPercentage ?? 0,
                LastDayDate       = message.Configuration.DateEnd,
                PaymentDate       = DateTime.Now,
                ProductExternalId = item.ProductId,
                ProductName       = productName,
                UserCreatedAt     = affiliate.UserCreatedAt
            });
        }

        request.LevelsType   = levelsType;
        request.EcoPoolsType = ecoPoolsType;
        await walletRepository!.CreateModelTwoSP(request);
        Logger.LogInformation($"[ProcessModelTwoConsumer] | EcoPoolProcess | Batch Completed");

        return true;
    }
}