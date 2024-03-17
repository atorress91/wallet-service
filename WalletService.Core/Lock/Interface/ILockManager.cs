namespace WalletService.Core.Lock.Interface;

public interface ILockManager
{
    public Task<bool> Take(string    key, string value, TimeSpan expiry);
    public Task<bool> Release(string key, string value);
}