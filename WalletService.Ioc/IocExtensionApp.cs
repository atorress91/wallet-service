using System.Reflection;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using FluentValidation.AspNetCore;
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
using WalletService.Core.Factory;
using WalletService.Core.Kafka.Producer;
using WalletService.Core.Kafka.Topics;
using WalletService.Core.Mapper;
using WalletService.Core.PaymentStrategies;
using WalletService.Core.Services;
using WalletService.Core.Services.IServices;
using WalletService.Data.Adapters;
using WalletService.Data.Adapters.IAdapters;
using WalletService.Data.Database;
using WalletService.Data.Repositories;
using WalletService.Data.Repositories.IRepositories;
using WalletService.Models.Configuration;

namespace WalletService.Ioc;

public static class IocExtensionApp
{
    public static void IocAppInjectDependencies(this IServiceCollection services, string[]? args = null)
    {
        InjectSwagger(services);
        InjectConfiguration(services);
        InjectAuthentication(services);
        InjectControllersAndDocumentation(services);
        InjectDataBases(services);
        InjectRepositories(services);
        InjectAdapters(services);
        InjectServices(services);
        InjectPackages(services);
        InjectLogging(services);
        InjectFactories(services);
        InjectStrategies(services);
        InjectSingletonsAndFactories(services);
        RegisterServiceProvider(services);
    }

    private static void InjectAuthentication(IServiceCollection services)
        => services.AddAuthentication().AddJwtBearer();

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

    private static void InjectSwagger(this IServiceCollection services)
        => services.AddSwaggerGen(c => { c.SwaggerDoc("v1", new OpenApiInfo { Title = "WalletService", Version = "v1" }); });


    private static void InjectDataBases(IServiceCollection services)
    {
        var appConfig = services.BuildServiceProvider().GetRequiredService<IOptions<ApplicationConfiguration>>()
            .Value;

        var connectionString = appConfig.ConnectionStrings?.SqlServerConnection;

        services.AddDbContext<WalletServiceDbContext>(options => { options.UseSqlServer(connectionString).EnableSensitiveDataLogging().EnableDetailedErrors(); });
    }

    private static void InjectLogging(IServiceCollection services)
    {
        var serviceProvider      = services.BuildServiceProvider();
        var env                  = serviceProvider.GetRequiredService<IWebHostEnvironment>();
        var lowerCaseEnvironment = env.EnvironmentName.ToLower();

        services.AddLogging(config =>
        {
            config.ClearProviders();
            config.AddNLog($"nlog.{lowerCaseEnvironment}.config");
        });

        Console.WriteLine($"IocLoggingRegister -> nlog.{lowerCaseEnvironment}.config");
    }

    private static void InjectControllersAndDocumentation(IServiceCollection services, int majorVersion = 1, int minorVersion = 0)
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
            config.DefaultApiVersion                   = new ApiVersion(majorVersion, minorVersion);
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
        
    }

    private static void InjectAdapters(IServiceCollection services)
    {
        services.AddScoped<ICoinPayAdapter, CoinPayAdapter>();
        services.AddScoped<IAccountServiceAdapter, AccountServiceAdapter>();
        services.AddScoped<IInventoryServiceAdapter, InventoryServiceAdapter>();
        services.AddScoped<IConfigurationAdapter, ConfigurationAdapter>();
    }

    private static void InjectFactories(IServiceCollection services)
    {
        services.AddScoped<IPaymentStrategyFactory, PaymentStrategyFactory>();
    }

    private static void InjectStrategies(IServiceCollection services)
    {
        services.AddScoped<BalancePaymentStrategy>();
        services.AddScoped<ReversedBalancePaymentStrategy>();
        services.AddScoped<ToThirdPartiesPaymentStrategy>();
        services.AddScoped<MembershipPaymentStrategy>();
        services.AddScoped<CoinPaymentsPaymentStrategy>();
        services.AddScoped<WireTransferStrategy>();
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
        services.AddScoped<IMediatorPdfService, MediatorPdfService>();
        services.AddScoped<IResultsEcoPoolService, ResultsEcoPoolService>();
        services.AddScoped<IConPaymentService, ConPaymentService>();
        services.AddScoped<IBrevoEmailService, BrevoEmailService>();
        services.AddScoped<IPaymentTransactionService, PaymentTransactionService>();
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
}