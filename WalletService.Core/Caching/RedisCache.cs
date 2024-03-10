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
        var deserializeObject = JsonConvert.DeserializeObject<T?>(keyValue.ToString(), jsonSerializerSettings);
        
        return deserializeObject;
    }

    public Task Delete(string key)
    {
        throw new NotImplementedException();
    }

    public Task<List<T?>> GetMultiple<T>(List<string> keys)
    {
        throw new NotImplementedException();
    }

    public Task<T> Remember<T>(string key, TimeSpan? timeOut, Func<Task<T>> action)
    {
        throw new NotImplementedException();
    }

    public Task<bool> KeyExists(string key)
    {
        throw new NotImplementedException();
    }
}