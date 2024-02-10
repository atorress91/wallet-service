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
using WalletService.Models.Enums;
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


    public async Task ExecuteFirstProcess()
    {
        var model1AConfiguration = await _configurationRepository.GetConfigurationByType(ModelTypeConfiguration.Model_1A.ToString());
        var model1BConfiguration = await _configurationRepository.GetConfigurationByType(ModelTypeConfiguration.Model_1B.ToString());
        var model2Configuration  = await _configurationRepository.GetConfiguration();
        var model3Configuration  = await _configurationRepository.GetConfigurationByType(ModelTypeConfiguration.Model_3.ToString());
        
        if (model1AConfiguration is not null)
            await ProcessModel1A(model1AConfiguration);
        if (model1BConfiguration is not null)
            await ProcessModel1B(model1BConfiguration);
        if (model2Configuration is not null)
            await ProcessModel2(model2Configuration);
        if (model3Configuration is not null)
            await ProcessModel3(model3Configuration);
    }

    private async Task ProcessModel2(ModelConfiguration model2Configuration)
    {
        var configuration2Mapped = Mapper.Map<ModelConfigurationDto>(model2Configuration);
        var starDate = new DateTime(model2Configuration.DateInit.Year, model2Configuration.DateInit.Month, model2Configuration.DateInit.Day,
            00, 00, 00);
        var endDate = new DateTime(model2Configuration.DateEnd.Year, model2Configuration.DateEnd.Month, model2Configuration.DateEnd.Day, 23,
            59, 59);

        var poolsModel2Within  = await _walletRepository.GetDebitsModel2WithinMonth(starDate, model2Configuration.DateEnd);
        // var poolsModel2Outside = await _walletRepository.GetDebitsModel2OutsideMonth(starDate);
        var poolsModel2Outside = new List<InvoicesDetails>();

        var accountsModel2 = poolsModel2Within.Union(poolsModel2Outside)
            .Select(x  => x.Invoice.AffiliateId).Distinct().ToArray();
        var productsModel2 = poolsModel2Within.Union(poolsModel2Outside)
            .Select(x => x.ProductId).Distinct().ToArray();

        model2Configuration.Totals = poolsModel2Within.Count + poolsModel2Outside.Count;
        await _configurationRepository.UpdateConfiguration(model2Configuration);

        var listResultModel2Products   = await GetListProducts(productsModel2);
        var listResultModel2Accounts   = await GetListAccount(accountsModel2, model2Configuration);
        var pointConfigurationResponse = await _configurationAdapter.GetPointsConfiguration();
        var points                     = pointConfigurationResponse.Content?.ToDecimal();

        var poolsWithinMothMapped  = Mapper.Map<ICollection<InvoiceDetailDto>>(poolsModel2Within);
        var poolsWithoutMothMapped = Mapper.Map<ICollection<InvoiceDetailDto>>(poolsModel2Outside);

        await SendModel2Process(poolsWithinMothMapped, poolsWithoutMothMapped, configuration2Mapped, points, endDate, starDate, listResultModel2Accounts,
            listResultModel2Products,
            KafkaTopics.ProcessModel2Topic);
        
    }
    private async Task ProcessModel1A(ModelConfiguration model1AConfiguration)
    {
        var configuration1AMapped = Mapper.Map<ModelConfigurationDto>(model1AConfiguration);
        var starDate = new DateTime(model1AConfiguration.DateInit.Year, model1AConfiguration.DateInit.Month, model1AConfiguration.DateInit.Day,
            00, 00, 00);
        var endDate = new DateTime(model1AConfiguration.DateEnd.Year, model1AConfiguration.DateEnd.Month, model1AConfiguration.DateEnd.Day, 23,
            59, 59);

        var itemsModel1AWithin  = await _walletRepository.GetDebitsModel1AWithinMonth(starDate, model1AConfiguration.DateEnd);
        var itemsModel1AOutside = await _walletRepository.GetDebitsModel1AOutsideMonth(starDate);

        var accountsModel1A = itemsModel1AWithin.Union(itemsModel1AOutside)
            .Select(x => x.Invoice.AffiliateId).Distinct().ToArray();
        var productsModel1A = itemsModel1AWithin.Union(itemsModel1AOutside)
            .Select(x => x.ProductId).Distinct().ToArray();

        model1AConfiguration.Totals = itemsModel1AWithin.Count + itemsModel1AOutside.Count;
        await _configurationRepository.UpdateConfiguration(model1AConfiguration);

        var listResultModel1AProducts   = await GetListProducts(productsModel1A);
        var listResultModel1AAccounts   = await GetListAccount(accountsModel1A, model1AConfiguration);
        var pointConfigurationResponse = await _configurationAdapter.GetPointsConfiguration();
        var points                     = pointConfigurationResponse.Content?.ToDecimal();

        var itemsWithinMothMapped  = Mapper.Map<ICollection<InvoiceDetailDto>>(itemsModel1AWithin);
        var itemsWithoutMothMapped = Mapper.Map<ICollection<InvoiceDetailDto>>(itemsModel1AOutside);

        await SendModel1AProcess(itemsWithinMothMapped, itemsWithoutMothMapped,configuration1AMapped, points, endDate, starDate, listResultModel1AAccounts,
            listResultModel1AProducts,
            KafkaTopics.ProcessModel1ATopic);
        
    }
    private async Task ProcessModel1B(ModelConfiguration model1BConfiguration)
    {
        var configuration1BMapped = Mapper.Map<ModelConfigurationDto>(model1BConfiguration);
        var starDate = new DateTime(model1BConfiguration.DateInit.Year, model1BConfiguration.DateInit.Month, model1BConfiguration.DateInit.Day,
            00, 00, 00);
        var endDate = new DateTime(model1BConfiguration.DateEnd.Year, model1BConfiguration.DateEnd.Month, model1BConfiguration.DateEnd.Day, 23,
            59, 59);

        var itemsModel1BWithin  = await _walletRepository.GetDebitsModel1BWithinMonth(starDate, model1BConfiguration.DateEnd);
        var itemsModel1BOutside = await _walletRepository.GetDebitsModel1BOutsideMonth(starDate);

        var accountsModel1B = itemsModel1BWithin.Union(itemsModel1BOutside)
            .Select(x => x.Invoice.AffiliateId).Distinct().ToArray();
        var productsModel1B = itemsModel1BWithin.Union(itemsModel1BOutside)
            .Select(x => x.ProductId).Distinct().ToArray();

        model1BConfiguration.Totals = itemsModel1BWithin.Count + itemsModel1BOutside.Count;
        await _configurationRepository.UpdateConfiguration(model1BConfiguration);

        var listResultModel1BProducts   = await GetListProducts(productsModel1B);
        var listResultModel1BAccounts   = await GetListAccount(accountsModel1B, model1BConfiguration);
        var pointConfigurationResponse = await _configurationAdapter.GetPointsConfiguration();
        var points                     = pointConfigurationResponse.Content?.ToDecimal();

        var itemsWithinMothMapped  = Mapper.Map<ICollection<InvoiceDetailDto>>(itemsModel1BWithin);
        var itemsWithoutMothMapped = Mapper.Map<ICollection<InvoiceDetailDto>>(itemsModel1BOutside);

        await SendModel1BProcess(itemsWithinMothMapped, itemsWithoutMothMapped,configuration1BMapped, points, endDate, starDate, listResultModel1BAccounts,
            listResultModel1BProducts,
            KafkaTopics.ProcessModel1BTopic);
    }

    private async Task ProcessModel3(ModelConfiguration model3Configuration)
    {
        var configuration3Mapped = Mapper.Map<ModelConfigurationDto>(model3Configuration);
        
        var starDate = new DateTime(model3Configuration.DateInit.Year, model3Configuration.DateInit.Month, model3Configuration.DateInit.Day,
            00, 00, 00);
        
        var endDate = new DateTime(model3Configuration.DateEnd.Year, model3Configuration.DateEnd.Month, model3Configuration.DateEnd.Day, 23,
            59, 59);
        
        var itemForModel3        = await _walletRepository.GetInvoicesDetailsItemsForModel3(starDate, endDate);

        var accountsModel3 = itemForModel3.Select(x => x.Invoice.AffiliateId).Distinct().ToArray();
        var productsModel3 = itemForModel3.Select(x => x.ProductId).Distinct().ToArray();

        model3Configuration.Totals = itemForModel3.Count;
        await _configurationRepository.UpdateConfiguration(model3Configuration);

        var listResultModel3Products = await GetListProducts(productsModel3);
        var listResultModel3Accounts = await GetListAccount(accountsModel3, model3Configuration);

        var itemForModel3Mapped = Mapper.Map<ICollection<InvoiceDetailDto>>(itemForModel3);

        await SendModel3Process(itemForModel3Mapped, configuration3Mapped, listResultModel3Accounts, listResultModel3Products,
            KafkaTopics.ProcessModel3Topic);
    }

    private Task SendModel2Process(
        ICollection<InvoiceDetailDto>  itemWithIn,
        ICollection<InvoiceDetailDto>  itemWithOut, 
        ModelConfigurationDto          configuration,
        decimal?                       points,
        DateTime                       endDate,
        DateTime                       starDate,
        ICollection<UserModelResponse> listResultAccounts,
        ICollection<ProductWalletDto>  listResultProducts,
        string                         topic)
    {
        if (itemWithIn is { Count: > 0 })
        {
            var batchesSize = (decimal)itemWithIn.Count / Constants.Batches;

            for (var i = 0; i < batchesSize; i++)
            {
                var packListPool = itemWithIn.Skip(i * Constants.Batches).Take(Constants.Batches).ToList();
                var key          = Constants.PartitionKeys[i % 5];
                _ = Task.Run(
                    async ()
                        => await _kafkaProducer.ProduceWithKeyAsync(topic, new Model2Message
                        {
                            Configuration      = configuration,
                            ItemWithInMonth    = packListPool,
                            Points             = points ?? 1,
                            EndDate            = endDate,
                            StarDate           = starDate,
                            ListResultAccounts = listResultAccounts,
                            ListResultProducts = listResultProducts
                        }, key));
            }
        }
        if (itemWithOut is { Count: > 0 })
        {
            var batchesSize = (decimal)itemWithOut.Count / Constants.Batches;

            for (var i = 0; i < batchesSize; i++)
            {
                var packListPool = itemWithOut.Skip(i * Constants.Batches).Take(Constants.Batches).ToList();
                var key          = Constants.PartitionKeys[i % 5];
                _ = Task.Run(
                    async ()
                        => await _kafkaProducer.ProduceWithKeyAsync(topic, new Model2Message
                        {
                            Configuration      = configuration,
                            ItemWithOutMonth    = packListPool,
                            Points             = points ?? 1,
                            EndDate            = endDate,
                            StarDate           = starDate,
                            ListResultAccounts = listResultAccounts,
                            ListResultProducts = listResultProducts
                        }, key));
            }
        }
        
        return Task.CompletedTask;
    }
    
    private Task SendModel1BProcess(
        ICollection<InvoiceDetailDto>          itemWithIn,
        ICollection<InvoiceDetailDto>          itemWithOut,
        ModelConfigurationDto                  configuration,
        decimal?                               points,
        DateTime                               endDate,
        DateTime                               starDate,
        ICollection<UserModelResponse>         listResultAccounts,
        ICollection<ProductWalletDto>          listResultProducts,
        string                                 topic)
    {
        if (itemWithIn is { Count: > 0 })
        {
            var batchesSize = (decimal)itemWithIn.Count / Constants.Batches;

            for (var i = 0; i < batchesSize; i++)
            {
                var packListPool = itemWithIn.Skip(i * Constants.Batches).Take(Constants.Batches).ToList();
                var key          = Constants.PartitionKeys[i % 5];
                _ = Task.Run(
                    async ()
                        => await _kafkaProducer.ProduceWithKeyAsync(topic, new Model1BMessage
                        {
                            Configuration      = configuration,
                            ItemWithInMonth    = packListPool,
                            Points             = points ?? 1,
                            EndDate            = endDate,
                            StarDate           = starDate,
                            ListResultAccounts = listResultAccounts,
                            ListResultProducts = listResultProducts
                        }, key));
            }
        }
        if (itemWithOut is { Count: > 0 })
        {
            var batchesSize = (decimal)itemWithOut.Count / Constants.Batches;

            for (var i = 0; i < batchesSize; i++)
            {
                var packListPool = itemWithOut.Skip(i * Constants.Batches).Take(Constants.Batches).ToList();
                var key          = Constants.PartitionKeys[i % 5];
                _ = Task.Run(
                    async ()
                        => await _kafkaProducer.ProduceWithKeyAsync(topic, new Model1BMessage
                        {
                            Configuration      = configuration,
                            ItemWithOutMonth    = packListPool,
                            Points             = points ?? 1,
                            EndDate            = endDate,
                            StarDate           = starDate,
                            ListResultAccounts = listResultAccounts,
                            ListResultProducts = listResultProducts
                        }, key));
            }
        }
        
        return Task.CompletedTask;
    }    private Task SendModel1AProcess(
        ICollection<InvoiceDetailDto>          itemWithIn,
        ICollection<InvoiceDetailDto>          itemWithOut,
        ModelConfigurationDto                  configuration,
        decimal?                               points,
        DateTime                               endDate,
        DateTime                               starDate,
        ICollection<UserModelResponse>         listResultAccounts,
        ICollection<ProductWalletDto>          listResultProducts,
        string                                 topic)
    {
        if (itemWithIn is { Count: > 0 })
        {
            var batchesSize = (decimal)itemWithIn.Count / Constants.Batches;

            for (var i = 0; i < batchesSize; i++)
            {
                var packListPool = itemWithIn.Skip(i * Constants.Batches).Take(Constants.Batches).ToList();
                var key          = Constants.PartitionKeys[i % 5];
                _ = Task.Run(
                    async ()
                        => await _kafkaProducer.ProduceWithKeyAsync(topic, new Model1AMessage
                        {
                            Configuration      = configuration,
                            ItemWithInMonth    = packListPool,
                            Points             = points ?? 1,
                            EndDate            = endDate,
                            StarDate           = starDate,
                            ListResultAccounts = listResultAccounts,
                            ListResultProducts = listResultProducts
                        }, key));
            }
        }
        
        if (itemWithOut is { Count: > 0 })
        {
            var batchesSize = (decimal)itemWithOut.Count / Constants.Batches;

            for (var i = 0; i < batchesSize; i++)
            {
                var packListPool = itemWithOut.Skip(i * Constants.Batches).Take(Constants.Batches).ToList();
                var key          = Constants.PartitionKeys[i % 5];
                _ = Task.Run(
                    async ()
                        => await _kafkaProducer.ProduceWithKeyAsync(topic, new Model1AMessage
                        {
                            Configuration      = configuration,
                            ItemWithOutMonth   = packListPool,
                            Points             = points ?? 1,
                            EndDate            = endDate,
                            StarDate           = starDate,
                            ListResultAccounts = listResultAccounts,
                            ListResultProducts = listResultProducts
                        }, key));
            }
        }
        
        return Task.CompletedTask;
    }
    
    private Task SendModel3Process(
        ICollection<InvoiceDetailDto>           itemsModelTwo,
        ModelConfigurationDto                configuration,
        ICollection<UserModelResponse> listResultAccounts,
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
                    => await _kafkaProducer.ProduceWithKeyAsync(topic, new Model3Message()
                    {
                        EducatedCourses    = batchModelTwo,
                        Configuration      = configuration,
                        ListResultAccounts = listResultAccounts,
                        ListResultProducts = listResultProducts
                    }, key));
        }

        return Task.CompletedTask;
    }
    

    private async Task<ICollection<UserModelResponse>> GetListAccount(
        IReadOnlyCollection<int> accounts,
        ModelConfiguration     configuration)
    {
        const int limit          = 100;
        var       batchesAccount = (decimal)accounts.Count / limit;

        var listResultAccounts = new List<UserModelResponse>();

        for (var i = 0; i < batchesAccount; i++)
        {
            try
            {
                var batchList = accounts.Skip(i * limit).Take(limit).ToArray();
                var accountResponse = await _accountServiceAdapter
                    .GetAccountsToEcoPool(batchList, configuration.ModelConfigurationLevels.Count);

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
    
    public Task ExecuteSecondProcess()
    {
        _ = Task.Run(async ()
                => await _kafkaProducer.ProduceAsync(KafkaTopics.ProcessPaymentModelTwoThreeTopic, string.Empty));
        return Task.CompletedTask;
    }
}