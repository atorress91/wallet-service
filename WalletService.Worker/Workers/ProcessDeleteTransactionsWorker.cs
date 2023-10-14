using NLog;
using WalletService.Core.Services.IServices;
using WalletService.Utility.Extensions;
using WalletService.Worker.Abstract;

namespace WalletService.Worker.Workers;

public class ProcessDeleteTransactionsWorker : CronJobService
{
    private readonly IServiceProvider                         _serviceProvider;
    private readonly Logger _logger;

    public ProcessDeleteTransactionsWorker(
        string                                   cronExpression,
        IServiceProvider                         serviceProvider) : base(cronExpression)
    {
        _serviceProvider = serviceProvider;
        _logger          = LogManager.GetCurrentClassLogger();
    }

    protected override async Task Work(CancellationToken cancellationToken)
    {
        try
        {
            using var scope          = _serviceProvider.CreateScope();
            var       invoiceService = scope.ServiceProvider.GetRequiredService<IInvoiceService>();
            _logger.Info("[ProcessDeleteTransactionsWorker] | Work | Started");

            await invoiceService.RevertUnconfirmedOrUnpaidTransactions();
        }
        catch (Exception e)
        {
            _logger.Error($"[ProcessDeleteTransactionsWorker] | Work | Exception | {e.ToJsonString()}");
        }
    }
}