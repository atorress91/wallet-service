using Newtonsoft.Json;
using StackExchange.Redis;
using WalletService.Core.Caching.Interface;
using WalletService.Utility.Extensions;

namespace WalletService.Core.Caching;

public class RedisCache : ICache
{
    private readonly IDatabase _db;

    public RedisCache(IConnectionMultiplexer connectionMultiplexer)
        => _db = connectionMultiplexer.GetDatabase();

    public async Task Set(string key, object? value, TimeSpan? timeOut = null, TimeSpan? slidingExpiration = null)
    {
        string? jsonStringData = null;
        var stringValue    = value?.ToString();

        if (stringValue is not null && stringValue.IsValidJson())
            jsonStringData = stringValue;
        else if (value is not null)
            jsonStringData = value.ToJsonString(new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All, 
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });

        await _db.StringSetAsync(key, jsonStringData, timeOut);
    }

    public async Task<T?> Get<T>(string key)
    {
        var keyValue = await _db.StringGetAsync(key);

        if (! keyValue.HasValue)
            return default;

        var jsonSerializerSettings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.All,
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        };
        var deserializeObject = JsonConvert.DeserializeObject<T>(keyValue.ToString(), jsonSerializerSettings);
        
        return deserializeObject;
    }

    public async Task Delete(string key)
        => await _db.KeyDeleteAsync(key);

    public async Task<List<T?>> GetMultiple<T>(List<string> keys)
    {
        var redisKeys = keys.Select(x => new RedisKey(x)).ToArray();
        var data      = await _db.StringGetAsync(redisKeys);

        if (data.Assigned())
        {
            return data
                .Where(x => x.Assigned() && x.HasValue)
                .Select(s => s.ToString().ToJsonObject<T>())
                .ToList();
        }

        return new List<T?>();
    }

    public async Task<T> Remember<T>(string key, TimeSpan? timeOut, Func<Task<T>> action)
    {
        var value = await Get<T>(key);

        if (value is null || value.NotAssigned())
        {
            value = await action();
            await Set(key, value, timeOut);
        }

        return value;
    }

    public Task<bool> KeyExists(string key)
        => _db.KeyExistsAsync(new RedisKey(key));
}