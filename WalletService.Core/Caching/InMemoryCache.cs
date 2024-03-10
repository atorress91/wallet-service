using Microsoft.Extensions.Caching.Memory;
using WalletService.Core.Caching.Interface;
using WalletService.Utility.Extensions;

namespace WalletService.Core.Caching;

public class InMemoryCache : ICache
{
    private readonly MemoryCache _memoryCache = new(new MemoryCacheOptions());
    private readonly SemaphoreSlim _semaphore;

    public InMemoryCache()
        => _semaphore = new SemaphoreSlim(1, 1);

    public async Task Set(string key, object? value, TimeSpan? timeOut = null, TimeSpan? slidingExpiration = null)
    {
        try
        {
            await _semaphore.WaitAsync();
            await Task.Run(() =>
            {
                if (timeOut.HasValue)
                    _memoryCache.Set(key, value, timeOut.Value);
                else
                    _memoryCache.Set(key, value, new MemoryCacheEntryOptions
                    {
                        SlidingExpiration = slidingExpiration
                    });
            });
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public async Task<T?> Get<T>(string key)
    {
        try
        {
            await _semaphore.WaitAsync();
            return _memoryCache.Get<T>(key);
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public async Task Delete(string key)
    {
        try
        {
            await _semaphore.WaitAsync();
            await Task.Run(() => { _memoryCache.Remove(key); });
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public async Task<List<T?>> GetMultiple<T>(List<string> keys)
    {
        var list = new List<T?>();
        foreach (var key in keys)
            list.Add(await Get<T?>(key));

        return list;
    }
    
    public async Task<T> Remember<T>(string key, TimeSpan? timeOut, Func<Task<T>> action)
    {
        var data = await Get<T?>(key);
        if (data is not null && data.Assigned())
            return data;

        data = await action();
        await Set(key, data, timeOut);

        return data;
    }

    public Task<bool> KeyExists(string key)
    {
        return Task.FromResult(_memoryCache.TryGetValue(key, out _));
    }
}