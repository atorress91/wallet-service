using WalletService.Data.Database.Models;

namespace WalletService.Data.Repositories.IRepositories;

public interface IWalletHistoryRepository
{
    Task<List<WalletsHistory>> GetAllWalletsHistoriesAsync();
    Task<WalletsHistory?> GetWalletHistoriesByIdAsync(int            id);
    Task<WalletsHistory> CreateWalletHistoriesAsync(WalletsHistory request);
    Task<WalletsHistory> UpdateWalletHistoriesAsync(WalletsHistory request);
    Task<WalletsHistory> DeleteWalletHistoriesAsync(WalletsHistory request);
}