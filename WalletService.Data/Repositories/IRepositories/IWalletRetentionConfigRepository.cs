using WalletService.Data.Database.Models;

namespace WalletService.Data.Repositories;

public interface IWalletRetentionConfigRepository
{
    Task<List<WalletsRetentionsConfigs>> GetAllWalletsRetentionConfig();
    Task<WalletsRetentionsConfigs?> GetWalletRetentionConfigById(int                                                   id);
    Task<IEnumerable<WalletsRetentionsConfigs>> CreateWalletRetentionConfigAsync(IEnumerable<WalletsRetentionsConfigs> request);
    Task<IEnumerable<WalletsRetentionsConfigs>> UpdateWalletRetentionConfigAsync(IEnumerable<WalletsRetentionsConfigs> request);
    Task<WalletsRetentionsConfigs> DeleteWalletRetentionConfigAsync(WalletsRetentionsConfigs                           request);
}