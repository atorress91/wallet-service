using System.Text.Json;
using AutoMapper;
using WalletService.Core.Kafka.Messages;
using WalletService.Core.Kafka.Producer;
using WalletService.Core.Kafka.Topics;
using WalletService.Core.Services.IServices;
using WalletService.Data.Adapters.IAdapters;
using WalletService.Data.Database.Models;
using WalletService.Data.Repositories.IRepositories;
using WalletService.Models.Constants;
using WalletService.Models.DTO.InvoiceDto;
using WalletService.Models.DTO.ProcessGradingDto;
using WalletService.Models.DTO.ProductWalletDto;
using WalletService.Models.Responses;
using WalletService.Utility.Extensions;

namespace WalletService.Core.Services;

public class ProcessGradingService : BaseService, IProcessGradingService
{

    private readonly KafkaProducer _kafkaProducer;
    private readonly IEcoPoolConfigurationRepository _configurationRepository;
    private readonly IWalletRepository _walletRepository;
    private readonly IConfigurationAdapter _configurationAdapter;
    private readonly IAccountServiceAdapter _accountServiceAdapter;
    private readonly IInventoryServiceAdapter _inventoryServiceAdapter;

    public ProcessGradingService(
        IMapper                         mapper,
        KafkaProducer                   kafkaProducer,
        IEcoPoolConfigurationRepository configurationRepository,
        IWalletRepository               walletRepository,
        IConfigurationAdapter           configurationAdapter,
        IAccountServiceAdapter          accountServiceAdapter,
        IInventoryServiceAdapter        inventoryServiceAdapter
    ) : base(mapper)
    {
        _configurationRepository = configurationRepository;
        _kafkaProducer           = kafkaProducer;
        _walletRepository        = walletRepository;
        _configurationAdapter    = configurationAdapter;
        _accountServiceAdapter   = accountServiceAdapter;
        _inventoryServiceAdapter = inventoryServiceAdapter;
    }


    public async Task EcoPoolProcess()
    {
        var configuration = await _configurationRepository.GetConfiguration();
        if (configuration is null)
            return;

        var starDate = new DateTime(configuration.DateInit.Year, configuration.DateInit.Month, configuration.DateInit.Day, 00, 00, 00);
        var endDate  = new DateTime(configuration.DateEnd.Year, configuration.DateEnd.Month, configuration.DateEnd.Day, 23, 59, 59);

        var poolsWithinMoth  = await _walletRepository.GetDebitsEcoPoolWithinMonth(starDate, configuration.DateEnd);
        var poolsOutsideMoth = await _walletRepository.GetDebitsEcoPoolOutsideMonth(starDate);
        var accounts         = poolsWithinMoth.Union(poolsOutsideMoth).Select(x => x.Invoice.AffiliateId).Distinct().ToArray();
        var products         = poolsWithinMoth.Union(poolsOutsideMoth).Select(x => x.ProductId).Distinct().ToArray();
        
        var listResultAccounts = await GetListAccount(accounts, configuration);
        
        var listResultProducts         = await GetListProducts(products);
        var pointConfigurationResponse = await _configurationAdapter.GetPointsConfiguration();
        var points                     = pointConfigurationResponse.Content?.ToDecimal();
        configuration.Totals = poolsWithinMoth.Count + poolsOutsideMoth.Count;
        await _configurationRepository.UpdateConfiguration(configuration);
        var configurationMapped    = Mapper.Map<EcoPoolConfigurationDto>(configuration);
        var poolsWithinMothMapped  = Mapper.Map<ICollection<InvoicePackDto>>(poolsWithinMoth);
        var poolsWithoutMothMapped = Mapper.Map<ICollection<InvoicePackDto>>(poolsOutsideMoth);

        await SendProcess(poolsWithinMothMapped, configurationMapped, points, endDate, starDate, listResultAccounts, listResultProducts,
            KafkaTopics.ProcessEcoPoolWithInTopic);
        await SendProcess(poolsWithoutMothMapped, configurationMapped, points, endDate, starDate, listResultAccounts,
            listResultProducts, KafkaTopics.ProcessEcoPoolWithOutTopic);
    }

    private Task SendProcess(
        ICollection<InvoicePackDto>      pools,
        EcoPoolConfigurationDto          configuration,
        decimal?                         points,
        DateTime                         endDate,
        DateTime                         starDate,
        ICollection<UserEcoPoolResponse> listResultAccounts,
        ICollection<ProductWalletDto>    listResultProducts,
        string                           topic)
    {
        var batchesSize = (decimal)pools.Count / Constants.Batches;

        for (var i = 0; i < batchesSize; i++)
        {
            var packListPool = pools.Skip(i * Constants.Batches).Take(Constants.Batches).ToList();
            var key          = Constants.PartitionKeys[i % 5];
            _ = Task.Run(
                async ()
                    => await _kafkaProducer.ProduceWithKeyAsync(topic, new EcoPoolProcessMessage
                    {
                        Configuration      = configuration,
                        Pools              = packListPool,
                        Points             = points ?? 1,
                        EndDate            = endDate,
                        StarDate           = starDate,
                        ListResultAccounts = listResultAccounts,
                        ListResultProducts = listResultProducts
                    }, key));
        }

        return Task.CompletedTask;
    }

    private async Task<ICollection<UserEcoPoolResponse>> GetListAccount(
        IReadOnlyCollection<int> accounts,
        EcoPoolConfiguration     configuration)
    {
        const int limit          = 500;
        var       batchesAccount = (decimal)accounts.Count / 500;

        var listResultAccounts = new List<UserEcoPoolResponse>();

        for (var i = 0; i < batchesAccount; i++)
        {
            var batchList = accounts.Skip(i * limit).Take(limit).ToArray();
            var accountResponse = await _accountServiceAdapter
                .GetAccountsToEcoPool(batchList, configuration.Levels.Count);

            if (!accountResponse.IsSuccessful)
                continue;

            if (accountResponse.Content is null)
                continue;

            var response = JsonSerializer.Deserialize<GetAccountsEcoPoolResponse>(accountResponse.Content);
            listResultAccounts.AddRange(response!.Data);
        }

        return listResultAccounts;
    }

    private async Task<ICollection<ProductWalletDto>> GetListProducts(
        IReadOnlyCollection<int> products)
    {
        const int limit          = 500;
        var       batchesProduct = (decimal)products.Count / 500;

        var listResultProducts = new List<ProductWalletDto>();

        for (var i = 0; i < batchesProduct; i++)
        {
            var batchList       = products.Skip(i * limit).Take(limit).ToArray();
            var productResponse = await _inventoryServiceAdapter.GetProductsIds(batchList);

            if (!productResponse.IsSuccessful)
                continue;

            var response = JsonSerializer.Deserialize<ProductsResponse>(productResponse.Content!);
            listResultProducts.AddRange(response!.Data);
        }

        return listResultProducts;
    }
    
    public Task PaymentProcess()
    {
        _ = Task.Run(async ()
                => await _kafkaProducer.ProduceAsync(KafkaTopics.ProcessEcoPoolPaymentTopic, string.Empty));
        return Task.CompletedTask;
    }
}