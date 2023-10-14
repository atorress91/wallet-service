using WalletService.Ioc;
using WalletService.Worker.Workers;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddHostedService<Worker>();
        // services.AddHostedService(serviceProvider => new ProcessDeleteTransactionsWorker("0 */1 * * *", serviceProvider));
        services.IocWorkerInjectDependencies();
    })
    .Build();

await host.RunAsync();