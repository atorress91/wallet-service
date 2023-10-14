using WalletService.Data.Database.Models;

namespace WalletService.Data.Repositories.IRepositories;

public interface IWalletHistoryRepository
{
    Task<List<WalletsHistories>> GetAllWalletsHistoriesAsync();
    Task<WalletsHistories?> GetWalletHistoriesByIdAsync(int            id);
    Task<WalletsHistories> CreateWalletHistoriesAsync(WalletsHistories request);
    Task<WalletsHistories> UpdateWalletHistoriesAsync(WalletsHistories request);
    Task<WalletsHistories> DeleteWalletHistoriesAsync(WalletsHistories request);
}