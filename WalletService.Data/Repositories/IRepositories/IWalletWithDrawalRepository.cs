using WalletService.Data.Database.Models;

namespace WalletService.Data.Repositories.IRepositories;

public interface IWalletWithDrawalRepository
{
    Task<List<WalletsWithdrawal>> GetAllWalletsWithdrawals();
    Task<WalletsWithdrawal?> GetWalletWithdrawalById(int                   id);
    Task<WalletsWithdrawal> CreateWalletWithdrawalAsync(WalletsWithdrawal request);
    Task<WalletsWithdrawal> UpdateWalletWithdrawalAsync(WalletsWithdrawal request);
    Task<WalletsWithdrawal> DeleteWalletWithdrawalAsync(WalletsWithdrawal request);
}