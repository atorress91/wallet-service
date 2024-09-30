namespace WalletService.Core.Services.IServices;

public interface IRedisCacheCleanupService
{
    Task CleanupAsync();
}