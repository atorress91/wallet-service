using Microsoft.Extensions.Logging;
using WalletService.Core.Services.IServices;
using StackExchange.Redis;

namespace WalletService.Core.Services;

public class RedisCacheCleanupService:IRedisCacheCleanupService
{
    private readonly IConnectionMultiplexer _connectionMultiplexer;
    private readonly ILogger<RedisCacheCleanupService> _logger;
    private readonly IDatabase _db;
    
    public RedisCacheCleanupService(IConnectionMultiplexer connectionMultiplexer, ILogger<RedisCacheCleanupService> logger)
    {
       _connectionMultiplexer = connectionMultiplexer;
       _logger = logger;
       _db = connectionMultiplexer.GetDatabase();
    }
    
    public async Task CleanupAsync()
    {
        try
        {
            var endpoints = _connectionMultiplexer.GetEndPoints();
            foreach (var endpoint in endpoints)
            {
                var server = _connectionMultiplexer.GetServer(endpoint);
              
                var keys = server.Keys();
                foreach (var key in keys)
                {
                    await _db.KeyDeleteAsync(key);
                }
                _logger.LogInformation($"CacheCleanupService | Redis cache cleaned at {endpoint}");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "CacheCleanupService | Error cleaning Redis cache");
        }
    }
}