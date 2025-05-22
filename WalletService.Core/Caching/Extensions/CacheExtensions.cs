using WalletService.Core.Caching.Interface;
using WalletService.Models.Constants;

namespace WalletService.Core.Caching.Extensions;

public static class CacheExtensions
{
    public static async Task InvalidateBalanceAsync(this ICache cache, int affiliateId)
    {
        string[] keys =
        [
            string.Format(CacheKeys.BalanceInformationModel2, affiliateId),
            string.Format(CacheKeys.BalanceInformationModel1A, affiliateId),
            string.Format(CacheKeys.BalanceInformationModel1B, affiliateId)
        ];

        foreach (var key in keys)
            await cache.Delete(key);  
    }
    
    public static Task InvalidateBalanceAsync(this ICache cache, params int[] affiliateIds) =>
        Task.WhenAll(affiliateIds.Select(cache.InvalidateBalanceAsync));
}