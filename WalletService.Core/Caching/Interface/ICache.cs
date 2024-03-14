using Org.BouncyCastle.Asn1.X509;

namespace WalletService.Core.Caching.Interface;

public interface ICache
{
    Task Set(string                            key, object? value, TimeSpan? timeOut = null, TimeSpan? slidingExpiration = null);
    Task<T?> Get<T>(string                     key);
    Task     Delete(string                     key);
    Task<List<T?>> GetMultiple<T>(List<string> keys);
    Task<T> Remember<T>(string                 key, TimeSpan? timeOut, Func<Task<T>> action);
    Task<bool> KeyExists(string                key);
}