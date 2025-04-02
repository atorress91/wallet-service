using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using WalletService.Core.Services.IServices;

namespace WalletService.Core.BackgroundServices;

public class RedisCacheCleanupBackgroundService:BackgroundService
{
    private readonly IServiceProvider _services;
    private readonly ILogger<RedisCacheCleanupBackgroundService> _logger;

    public RedisCacheCleanupBackgroundService(IServiceProvider services, ILogger<RedisCacheCleanupBackgroundService> logger)
    {
       _services = services;
       _logger = logger;
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("Redis cleanup service running.");

            using (var scope = _services.CreateScope())
            {
                var cleanupService = scope.ServiceProvider.GetRequiredService<IRedisCacheCleanupService>();
                await cleanupService.CleanupAsync();
            }
            
            await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
        }
    }
}