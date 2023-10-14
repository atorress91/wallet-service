using Cronos;

namespace WalletService.Worker.Abstract;

public abstract class CronJobService : IHostedService, IDisposable
{
    private          System.Timers.Timer _timer;
    private readonly CronExpression      _expression;
    private readonly TimeZoneInfo        _timeZoneInfo;

    protected CronJobService(string cronExpression)
    {   
        _expression   = CronExpression.Parse(cronExpression);
        _timeZoneInfo = TimeZoneInfo.Local;
    }

    protected abstract Task Work(CancellationToken cancellationToken);

    public virtual async Task StartAsync(CancellationToken cancellationToken)
    {
        await ScheduleJob(cancellationToken);
    }
    private async Task ScheduleJob(CancellationToken cancellationToken)
    {
        var next = _expression.GetNextOccurrence(DateTimeOffset.Now, _timeZoneInfo);
        if (next.HasValue)
        {
            var delay = next.Value - DateTimeOffset.Now;
            if (delay.TotalMilliseconds <= 0) await ScheduleJob(cancellationToken);

            _timer = new System.Timers.Timer(delay.TotalMilliseconds);
            _timer.Elapsed += async (sender, args) =>
            {
                _timer.Dispose();
                _timer = null!;

                if (!cancellationToken.IsCancellationRequested) await Work(cancellationToken);
                if (!cancellationToken.IsCancellationRequested) await ScheduleJob(cancellationToken);
            };
            _timer.Start();
        }

        await Task.CompletedTask;
    }

    public virtual async Task StopAsync(CancellationToken cancellationToken)
    {
        _timer.Stop();
        await Task.CompletedTask;
    }

    public virtual void Dispose()
    {
        _timer.Dispose();
    }
}