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
using WalletService.Models.DTO.InvoiceDetailDto;
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
        var lastMonth    = DateTime.Today.AddMonths(-2);

        var poolsWithinMoth  = await _walletRepository.GetDebitsEcoPoolWithinMonth(starDate, configuration.DateEnd);
        var poolsOutsideMoth = await _walletRepository.GetDebitsEcoPoolOutsideMonth(starDate);
        var itemForModelTwo  = await _walletRepository.GetInvoicesDetailsItemsForModelTwo(lastMonth.Month, lastMonth.Year);

        var accountsModelThree = poolsWithinMoth.Union(poolsOutsideMoth).Select(x => x.Invoice.AffiliateId).Distinct().ToArray();
        var productsModelThree = poolsWithinMoth.Union(poolsOutsideMoth).Select(x => x.ProductId).Distinct().ToArray();
        
        var accountsModelTwo   = itemForModelTwo.Select(x => x.Invoice.AffiliateId).Distinct().ToArray();
        var productsModelTwo   = itemForModelTwo.Select(x => x.ProductId).Distinct().ToArray();

        var accounts = accountsModelThree.Union(accountsModelTwo).Distinct().ToArray();
        var products = productsModelThree.Union(productsModelTwo).Distinct().ToArray();        

        var listResultProducts = await GetListProducts(products);
        var listResultAccounts         = await GetListAccount(accounts, configuration);
        var pointConfigurationResponse = await _configurationAdapter.GetPointsConfiguration();
        var points                     = pointConfigurationResponse.Content?.ToDecimal();
        configuration.Totals         = poolsWithinMoth.Count + poolsOutsideMoth.Count;
        configuration.ModelTwoTotals = itemForModelTwo.Count;

        await _configurationRepository.UpdateConfiguration(configuration);
        var configurationMapped    = Mapper.Map<EcoPoolConfigurationDto>(configuration);
        var poolsWithinMothMapped  = Mapper.Map<ICollection<InvoicePackDto>>(poolsWithinMoth);
        var poolsWithoutMothMapped = Mapper.Map<ICollection<InvoicePackDto>>(poolsOutsideMoth);
        var itemForModelTwoMapped = Mapper.Map<ICollection<InvoiceDetailDto>>(itemForModelTwo);

        await SendModelTwoProcess(itemForModelTwoMapped, configurationMapped, listResultAccounts, listResultProducts,
            KafkaTopics.ProcessModelTwoTopic);
        
        await SendModelThreeProcess(poolsWithinMothMapped, configurationMapped, points, endDate, starDate, listResultAccounts, listResultProducts,
            KafkaTopics.ProcessModelThreeWithInTopic);
        
        await SendModelThreeProcess(poolsWithoutMothMapped, configurationMapped, points, endDate, starDate, listResultAccounts,
            listResultProducts, KafkaTopics.ProcessModelThreeWithOutTopic);
    }

    private Task SendModelThreeProcess(
        ICollection<InvoicePackDto>      pools,
        EcoPoolConfigurationDto          configuration,
        decimal?                         points,
        DateTime                         endDate,
        DateTime                         starDate,
        ICollection<UserModelTwoThreeResponse> listResultAccounts,
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
                    => await _kafkaProducer.ProduceWithKeyAsync(topic, new ModelThreeMessage
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
    
    private Task SendModelTwoProcess(
        ICollection<InvoiceDetailDto>           itemsModelTwo,
        EcoPoolConfigurationDto                configuration,
        ICollection<UserModelTwoThreeResponse> listResultAccounts,
        ICollection<ProductWalletDto>          listResultProducts,
        string                                 topic)
    {
        var batchesSize = (decimal)itemsModelTwo.Count / Constants.Batches;

        for (var i = 0; i < batchesSize; i++)
        {
            var batchModelTwo = itemsModelTwo.Skip(i * Constants.Batches).Take(Constants.Batches).ToList();
            var key          = Constants.PartitionKeys[i % 5];
            _ = Task.Run(
                async ()
                    => await _kafkaProducer.ProduceWithKeyAsync(topic, new ModelTwoMessage()
                    {
                        EducatedCourses    = batchModelTwo,
                        Configuration      = configuration,
                        ListResultAccounts = listResultAccounts,
                        ListResultProducts = listResultProducts
                    }, key));
        }

        return Task.CompletedTask;
    }
    

    private async Task<ICollection<UserModelTwoThreeResponse>> GetListAccount(
        IReadOnlyCollection<int> accounts,
        EcoPoolConfiguration     configuration)
    {
        const int limit          = 100;
        var       batchesAccount = (decimal)accounts.Count / limit;

        var listResultAccounts = new List<UserModelTwoThreeResponse>();

        for (var i = 0; i < batchesAccount; i++)
        {
            try
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
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        return listResultAccounts;
    }

    private async Task<ICollection<ProductWalletDto>> GetListProducts(
        IReadOnlyCollection<int> products)
    {
        const int limit          = 100;
        var       batchesProduct = (decimal)products.Count / limit;

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
                => await _kafkaProducer.ProduceAsync(KafkaTopics.ProcessPaymentModelTwoThreeTopic, string.Empty));
        return Task.CompletedTask;
    }
}