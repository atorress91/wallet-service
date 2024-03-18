using StackExchange.Redis;
using WalletService.Core.Lock.Interface;
using IDatabase = StackExchange.Redis.IDatabase;

namespace WalletService.Core.Lock;

public class LockManager : ILockManager
{
    private readonly IDatabase _db;

    public LockManager(IConnectionMultiplexer connectionMultiplexer)
    {
        _db = connectionMultiplexer.GetDatabase();
    }
    public Task<bool> Take(string key, string value, TimeSpan expiry)
    {
        return _db.LockTakeAsync(key, value, expiry);
    }

    public Task<bool> Release(string    key, string value)
    {
        return _db.LockReleaseAsync(key, value);
    }
}