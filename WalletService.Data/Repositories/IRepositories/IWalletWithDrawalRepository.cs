using WalletService.Data.Database.Models;

namespace WalletService.Data.Repositories.IRepositories;

public interface IWalletWithDrawalRepository
{
    Task<List<WalletsWithdrawals>> GetAllWalletsWithdrawals();
    Task<WalletsWithdrawals?> GetWalletWithdrawalById(int                   id);
    Task<WalletsWithdrawals> CreateWalletWithdrawalAsync(WalletsWithdrawals request);
    Task<WalletsWithdrawals> UpdateWalletWithdrawalAsync(WalletsWithdrawals request);
    Task<WalletsWithdrawals> DeleteWalletWithdrawalAsync(WalletsWithdrawals request);
}