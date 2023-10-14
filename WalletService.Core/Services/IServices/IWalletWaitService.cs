using WalletService.Models.DTO.WalletWaitDto;
using WalletService.Models.Requests.WalletWaitRequest;

namespace WalletService.Core.Services.IServices;

public interface IWalletWaitService
{
    Task<IEnumerable<WalletWaitDto>> GetAllWalletsWaits();
    Task<WalletWaitDto?> GetWalletWaitById(int id);
    Task<WalletWaitDto?> CreateWalletWaitAsync(WalletWaitRequest request);
    Task<WalletWaitDto?> UpdateWalletWaitAsync(int id, WalletWaitRequest request);
    Task<WalletWaitDto?> DeleteWalletWaitAsync(int id);
}