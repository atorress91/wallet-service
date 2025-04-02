using WalletService.Data.Database.Models;

namespace WalletService.Data.Repositories.IRepositories;

public interface IWalletWaitRepository
{
    Task<List<WalletsWait>> GetAllWalletsWaits();
    Task<WalletsWait?> GetWalletWaitById(int             id);
    Task<WalletsWait> CreateWalletWaitAsync(WalletsWait request);
    Task<WalletsWait> UpdateWalletWaitAsync(WalletsWait request);
    Task<WalletsWait> DeleteWalletWaitAsync(WalletsWait request);
}