using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NLog.Extensions.Logging;
using StackExchange.Redis;
using WalletService.Core.Caching;
using WalletService.Core.Caching.Interface;
using WalletService.Core.Kafka.Producer;
using WalletService.Core.Kafka.Topics;
using WalletService.Core.Lock;
using WalletService.Core.Lock.Interface;
using WalletService.Core.Mapper;
using WalletService.Core.PaymentStrategies;
using WalletService.Core.PaymentStrategies.IPaymentStrategies;
using WalletService.Core.Services;
using WalletService.Core.Services.IServices;
using WalletService.Data.Adapters;
using WalletService.Data.Adapters.IAdapters;
using WalletService.Data.Database;
using WalletService.Data.Repositories;
using WalletService.Data.Repositories.IRepositories;
using WalletService.Models.Configuration;

namespace WalletService.Ioc;

public static class IocExtensionWorker
{
    public static void IocWorkerInjectDependencies(this IServiceCollection services, string[]? args = null)
    {
        InjectConfiguration(services);
        InjectCaching(services);
        InjectDataBases(services);
        InjectPackages(services);
        InjectRepositories(services);
        InjectServices(services);
        InjectAdapters(services);
        InjectLogging(services);
        InjectStrategies(services);
        InjectSingletonsAndFactories(services);
        RegisterServiceProvider(services);
    }

    private static void InjectCaching(IServiceCollection services)
    {
        var serviceProvider = services.BuildServiceProvider();
        var settings        = serviceProvider.GetRequiredService<IOptions<ApplicationConfiguration>>().Value;
        var multiplexer = ConnectionMultiplexer.Connect(settings.ConnectionStrings!.RedisConnection!);
        
        services.AddSingleton<IConnectionMultiplexer>(multiplexer);
        services.AddSingleton<RedisCache>();
        services.AddSingleton<InMemoryCache>();
        services.AddSingleton<ILockManager, LockManager>();
    }

    private static void InjectConfiguration(IServiceCollection services)
    {
        var serviceProvider      = services.BuildServiceProvider();
        var env                  = serviceProvider.GetRequiredService<IHostEnvironment>();
        var lowercaseEnvironment = env.EnvironmentName.ToLower();
        var executableLocation   = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? "";
        var builder = new ConfigurationBuilder()
            .SetBasePath(executableLocation)
            .AddJsonFile($"appsettings.{lowercaseEnvironment}.json", false, true)
            .AddEnvironmentVariables();

        var configuration      = builder.Build();
        var appSettingsSection = configuration.GetSection("AppSettings");

        services.Configure<ApplicationConfiguration>(appSettingsSection);
        services.AddSingleton(configuration);
        services.RegisterKafkaTopics(lowercaseEnvironment);
    }
    
    private static void InjectStrategies(IServiceCollection services)
    {
        services.AddScoped<IBalancePaymentStrategy,BalancePaymentStrategy>();
        services.AddScoped<IBalancePaymentStrategyModel2,BalancePaymentStrategyModel2>();
        services.AddScoped<ToThirdPartiesPaymentStrategy>();
        services.AddScoped<ICoinPayPaymentStrategy,CoinPayPaymentStrategy>();
        services.AddScoped<ICoinPaymentsPaymentStrategy,CoinPaymentsPaymentStrategy>();
        services.AddScoped<IWireTransferStrategy,WireTransferStrategy>();
        services.AddScoped<IBalancePaymentStrategyModel1A,BalancePaymentStrategy1A>();
        services.AddScoped<IBalancePaymentStrategyModel1B,BalancePaymentStrategy1B>();
    }

    private static void InjectDataBases(IServiceCollection services)
    {
        var appConfig = services.BuildServiceProvider()
            .GetRequiredService<IOptions<ApplicationConfiguration>>().Value;

        var connectionString = appConfig.ConnectionStrings?.SqlServerConnection;

        services.AddDbContext<WalletServiceDbContext>(options =>
        {
            options.UseSqlServer(connectionString).EnableSensitiveDataLogging().EnableDetailedErrors();
        });
    }
    
    private static void InjectLogging(IServiceCollection services)
    {
        var serviceProvider      = services.BuildServiceProvider();
        var env                  = serviceProvider.GetRequiredService<IHostEnvironment>();
        var configuration        = serviceProvider.GetRequiredService<IConfiguration>();
        var lowerCaseEnvironment = env.EnvironmentName.ToLower();

        services.AddLogging(config =>
        {
            config.ClearProviders();
            config.AddConfiguration(configuration);
            config.AddNLog($"nlog.{lowerCaseEnvironment}.config");
        });

        Console.WriteLine($"IocLoggingRegister -> nlog.{lowerCaseEnvironment}.config");
    }

    private static void InjectRepositories(IServiceCollection services)
    {
        services.AddScoped<IWalletHistoryRepository, WalletHistoryRepository>();
        services.AddScoped<IWalletPeriodRepository, WalletPeriodRepository>();
        services.AddScoped<IWalletRequestRepository, WalletRequestRepository>();
        services.AddScoped<IWalletRetentionConfigRepository, WalletRetentionConfigRepository>();
        services.AddScoped<IWalletRepository, WalletRepository>();
        services.AddScoped<IWalletWaitRepository, WalletWaitRepository>();
        services.AddScoped<IWalletWithDrawalRepository, WalletWithDrawalRepository>();
        services.AddScoped<IInvoiceDetailRepository, InvoiceDetailRepository>();
        services.AddScoped<IInvoiceRepository, InvoiceRepository>();
        services.AddScoped<IInvoiceRepository, InvoiceRepository>();
        services.AddScoped<INetworkPurchaseRepository, NetworkPurchaseRepository>();
        services.AddScoped<IEcoPoolConfigurationRepository, EcoPoolConfigurationRepository>();
        services.AddScoped<IResultsEcoPoolRepository, ResultsEcoPoolRepository>();
        services.AddScoped<IApiClientRepository, ApiClientRepository>();
        services.AddScoped<ICoinPaymentTransactionRepository, CoinPaymentTransactionRepository>();
        services.AddScoped<IWalletModel1ARepository, WalletModel1ARepository>();
        services.AddScoped<IWalletModel1BRepository, WalletModel1BRepository>();
        services.AddScoped<IBrandRepository, BrandRepository>();
        services.AddScoped<IBonusRepository, BonusRepository>();
    }

    private static void InjectAdapters(IServiceCollection services)
    {
        services.AddScoped<ICoinPayAdapter, CoinPayAdapter>();
        services.AddScoped<IAccountServiceAdapter, AccountServiceAdapter>();
        services.AddScoped<IInventoryServiceAdapter, InventoryServiceAdapter>();
        services.AddScoped<IConfigurationAdapter, ConfigurationAdapter>();
    }

    private static void InjectServices(IServiceCollection services)
    {
        services.AddScoped<ICoinPayService, CoinPayService>();
        services.AddScoped<IWalletHistoryService, WalletHistoryService>();
        services.AddScoped<IWalletPeriodService, WalletPeriodService>();
        services.AddScoped<IWalletRequestService, WalletRequestService>();
        services.AddScoped<IWalletRetentionConfigService, WalletRetentionConfigService>();
        services.AddScoped<IWalletService, Core.Services.WalletService>();
        services.AddScoped<IWalletWaitService, WalletWaitService>();
        services.AddScoped<IWalletWithdrawalService, WalletWithDrawalService>();
        services.AddScoped<IInvoiceDetailService, InvoiceDetailService>();
        services.AddScoped<IInvoiceService, InvoiceService>();
        services.AddScoped<IProcessGradingService, ProcessGradingService>();
        services.AddScoped<IEcoPoolConfigurationService, EcoPoolConfigurationService>();
        services.AddScoped<IEcosystemPdfService, EcosystemPdfService>();
        services.AddScoped<IResultsEcoPoolService, ResultsEcoPoolService>();
        services.AddScoped<IConPaymentService, ConPaymentService>();
        services.AddScoped<IBrevoEmailService, BrevoEmailService>();
        services.AddScoped<IPaymentTransactionService, PaymentTransactionService>();
        services.AddScoped<IWalletModel1AService, WalletModel1AService>();
        services.AddScoped<IWalletModel1BService, WalletModel1BService>();
        services.AddScoped<IUserStatisticsService, UserStatisticsService>();
        services.AddScoped<IBrandService, BrandService>();
        services.AddScoped<IRecyCoinPdfService, RecyCoinPdfService>();
    }

    private static void InjectPackages(IServiceCollection services)
    {
        services.AddAutoMapper(x => { x.AddProfile(new MapperProfile()); });
    }

    private static void RegisterServiceProvider(IServiceCollection services)
    {
        services.AddSingleton<HttpClient>();
        services.AddSingleton(services.BuildServiceProvider());
    }
    
    private static void InjectSingletonsAndFactories(IServiceCollection services)
    {
        services.AddHttpClient();
        services.AddHttpContextAccessor();

        services.AddSingleton(_ = new KafkaProducer(services));
    }
}