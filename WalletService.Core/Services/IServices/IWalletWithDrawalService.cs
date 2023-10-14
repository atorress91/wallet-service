using WalletService.Data.Database.Models;
using WalletService.Models.DTO.WalletWithDrawalDto;
using WalletService.Models.Requests.WalletWithDrawalRequest;

namespace WalletService.Core.Services.IServices;

public interface IWalletWithdrawalService
{
    Task<IEnumerable<WalletWithDrawalDto>> GetAllWalletsWithdrawals();
    Task<WalletWithDrawalDto?> GetWalletWithdrawalById(int id);
    Task<WalletWithDrawalDto?> CreateWalletWithdrawalAsync(WalletWithDrawalRequest request);
    Task<WalletWithDrawalDto?> UpdateWalletWithdrawalAsync(int id, WalletWithDrawalRequest request);
    Task<WalletWithDrawalDto?> DeleteWalletWithdrawalAsync(int id);
}