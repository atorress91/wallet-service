using WalletService.Data.Database.Models;

namespace WalletService.Data.Repositories.IRepositories;

public interface IWalletWaitRepository
{
    Task<List<WalletsWaits>> GetAllWalletsWaits();
    Task<WalletsWaits?> GetWalletWaitById(int             id);
    Task<WalletsWaits> CreateWalletWaitAsync(WalletsWaits request);
    Task<WalletsWaits> UpdateWalletWaitAsync(WalletsWaits request);
    Task<WalletsWaits> DeleteWalletWaitAsync(WalletsWaits request);
}