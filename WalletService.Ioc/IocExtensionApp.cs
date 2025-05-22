using System.Reflection;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using FluentValidation.AspNetCore;
using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using NLog.Extensions.Logging;
using Npgsql;
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
using WalletService.Data.UnitOfWork;
using WalletService.Data.UnitOfWork.IUnitOfWork;
using WalletService.Models.Configuration;
using WalletService.Models.Requests.WalletRequest;

namespace WalletService.Ioc;

public static class IocExtensionApp
{
    public static void IocAppInjectDependencies(this IServiceCollection services, string[]? args = null)
    {
        InjectSwagger(services);
        InjectConfiguration(services);
        InjectAuthentication(services);
        InjectControllersAndDocumentation(services);
        InjectCaching(services);
        InjectDataBases(services);
        InjectUnitOfWork(services);
        InjectRepositories(services);
        InjectAdapters(services);
        InjectServices(services);
        InjectPackages(services);
        InjectLogging(services);
        InjectStrategies(services);
        InjectSingletonsAndFactories(services);
        RegisterServiceProvider(services);
        services.InjectHangfire(); 
    }

    private static void InjectCaching(IServiceCollection services)
    {
        var serviceProvider = services.BuildServiceProvider();
        var settings = serviceProvider
            .GetRequiredService<IOptions<ApplicationConfiguration>>().Value;
        var multiplexer = ConnectionMultiplexer
            .Connect(settings.ConnectionStrings!.RedisConnection!);

        services.AddSingleton<IConnectionMultiplexer>(multiplexer);
        services.AddSingleton<RedisCache>();
        services.AddSingleton<InMemoryCache>();
        services.AddSingleton<ILockManager, LockManager>();
        services.AddSingleton<ICache, RedisCache>();
    }

    private static void InjectAuthentication(IServiceCollection services)
        => services.AddAuthentication().AddJwtBearer();

    private static void InjectConfiguration(IServiceCollection services)
    {
        var serviceProvider = services.BuildServiceProvider();
        var env = serviceProvider.GetRequiredService<IHostEnvironment>();
        var lowercaseEnvironment = env.EnvironmentName.ToLower();
        var executableLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? "";
        var builder = new ConfigurationBuilder()
            .SetBasePath(executableLocation)
            .AddJsonFile($"appsettings.{lowercaseEnvironment}.json", false, true)
            .AddEnvironmentVariables();

        var configuration = builder.Build();
        var appSettingsSection = configuration.GetSection("AppSettings");

        services.Configure<ApplicationConfiguration>(appSettingsSection);
        services.AddSingleton(configuration);
        services.RegisterKafkaTopics(lowercaseEnvironment);
    }

    private static void InjectSwagger(this IServiceCollection services)
        => services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "WalletService", Version = "v1" });
        });


    private static void InjectDataBases(IServiceCollection services)
    {
        var appConfig = services.BuildServiceProvider().GetRequiredService<IOptions<ApplicationConfiguration>>()
            .Value;

        var connectionString = appConfig.ConnectionStrings?.PostgreSqlConnection;

        var dataSourceBuilder = new NpgsqlDataSourceBuilder(connectionString);
        dataSourceBuilder.MapComposite<InvoiceDetailsTransactionRequest>(
            "wallet_service.invoices_details_type_with_brand");
        var dataSource = dataSourceBuilder.Build();

        services.AddDbContext<WalletServiceDbContext>(options =>
        {
            options.UseNpgsql(dataSource)
                .EnableSensitiveDataLogging()
                .EnableDetailedErrors();
        });
    }

    private static void InjectLogging(IServiceCollection services)
    {
        var serviceProvider = services.BuildServiceProvider();
        var env = serviceProvider.GetRequiredService<IWebHostEnvironment>();
        var lowerCaseEnvironment = env.EnvironmentName.ToLower();

        services.AddLogging(config =>
        {
            config.ClearProviders();
            config.AddNLog($"nlog.{lowerCaseEnvironment}.config");
        });

        Console.WriteLine($"IocLoggingRegister -> nlog.{lowerCaseEnvironment}.config");
    }

    private static void InjectControllersAndDocumentation(IServiceCollection services, int majorVersion = 1,
        int minorVersion = 0)
    {
        services.AddResponseCompression(options =>
        {
            options.Providers.Add<GzipCompressionProvider>();
            options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[] { "text/plain" });
        });

        services.AddControllers().AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
            options.JsonSerializerOptions.TypeInfoResolver = new DefaultJsonTypeInfoResolver();
        });

        services.AddFluentValidationAutoValidation();
        services.AddFluentValidationClientsideAdapters();
        services.AddApiVersioning(config =>
        {
            config.DefaultApiVersion = new ApiVersion(majorVersion, minorVersion);
            config.AssumeDefaultVersionWhenUnspecified = true;
        });

        services.AddCors(options =>
        {
            options.AddDefaultPolicy(
                builder =>
                {
                    builder.AllowAnyOrigin()
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });
        });
    }

    private static void InjectUnitOfWork(IServiceCollection services)
    {
        services.AddScoped<DbContext>(provider => 
            provider.GetService<WalletServiceDbContext>());
        services.AddScoped<IUnitOfWork, UnitOfWork>();
    }

    private static void InjectRepositories(IServiceCollection services)
    {
        services.AddScoped<IWalletHistoryRepository, WalletHistoryRepository>();
        services.AddScoped<IWalletPeriodRepository, WalletPeriodRepository>();
        services.AddScoped<IWalletRequestRepository, WalletRequestRepository>();
        services.AddScoped<IWalletRetentionConfigRepository, WalletRetentionConfigRepository>();
        services.AddScoped<IWalletRepository, WalletRepository>();
        services.AddScoped<IWalletModel1ARepository, WalletModel1ARepository>();
        services.AddScoped<IWalletModel1BRepository, WalletModel1BRepository>();
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
        services.AddScoped<IBrandRepository, BrandRepository>();
        services.AddScoped<IBonusRepository, BonusRepository>();
        services.AddScoped<ICreditRepository, CreditRepository>();
        services.AddScoped<IMatrixQualificationRepository, MatrixQualificationRepository>();
        services.AddScoped<IMatrixEarningsRepository, MatrixEarningsRepository>();
    }

    private static void InjectAdapters(IServiceCollection services)
    {
        services.AddScoped<ICoinPayAdapter, CoinPayAdapter>();
        services.AddScoped<IAccountServiceAdapter, AccountServiceAdapter>();
        services.AddScoped<IInventoryServiceAdapter, InventoryServiceAdapter>();
        services.AddScoped<IConfigurationAdapter, ConfigurationAdapter>();
        services.AddScoped<IPagaditoAdapter, PagaditoAdapter>();
    }

    private static void InjectStrategies(IServiceCollection services)
    {
        services.AddScoped<IBalancePaymentStrategy, BalancePaymentStrategy>();
        services.AddScoped<IBalancePaymentStrategyModel2, BalancePaymentStrategyModel2>();
        services.AddScoped<ToThirdPartiesPaymentStrategy>();
        services.AddScoped<ICoinPayPaymentStrategy, CoinPayPaymentStrategy>();
        services.AddScoped<ICoinPaymentsPaymentStrategy, CoinPaymentsPaymentStrategy>();
        services.AddScoped<IWireTransferStrategy, WireTransferStrategy>();
        services.AddScoped<IBalancePaymentStrategyModel1A, BalancePaymentStrategy1A>();
        services.AddScoped<IBalancePaymentStrategyModel1B, BalancePaymentStrategy1B>();
        services.AddScoped<IPagaditoPaymentStrategy, PagaditoPaymentStrategy>();
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
        services.AddScoped<IPagaditoService, PagaditoService>();
        services.AddScoped<IUserStatisticsService, UserStatisticsService>();
        services.AddScoped<IBrandService, BrandService>();
        services.AddScoped<IRecyCoinPdfService, RecyCoinPdfService>();
        services.AddScoped<IHouseCoinPdfService, HouseCoinPdfService>();
        services.AddScoped<IRedisCacheCleanupService, RedisCacheCleanupService>();
        services.AddScoped<IExitoJuntosPdfService, ExitoJuntosPdfService>();
        services.AddScoped<IMatrixService, MatrixService>();
        services.AddScoped<IMatrixEarningsService, MatrixEarningsService>();
        services.AddScoped<IMatrixQualificationService, MatrixQualificationService>();
        services.AddScoped<MatrixService>(); 
    }

    private static void InjectPackages(IServiceCollection services)
        => services.AddAutoMapper(x => { x.AddProfile(new MapperProfile()); });

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
    
    private static void InjectHangfire(this IServiceCollection services)
    {
        var provider  = services.BuildServiceProvider();
        var appConfig = provider.GetRequiredService<IOptions<ApplicationConfiguration>>().Value;

        var cs = appConfig.ConnectionStrings?.PostgreSqlConnection
                 ?? throw new InvalidOperationException("Connection string is missing in AppSettings:ConnectionStrings:PostgreSqlConnection");
        
        var storageOptions = new PostgreSqlStorageOptions
        {
            SchemaName               = "hangfire",
            PrepareSchemaIfNecessary = true,
            QueuePollInterval        = TimeSpan.FromSeconds(15)
        };

        services.AddHangfire(cfg => cfg
            .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
            .UseSimpleAssemblyNameTypeSerializer()
            .UseRecommendedSerializerSettings()
            .UsePostgreSqlStorage(opt => opt.UseNpgsqlConnection(cs), storageOptions));

        services.AddHangfireServer();
    }
}