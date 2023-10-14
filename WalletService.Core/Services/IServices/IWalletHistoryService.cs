using WalletService.Models.DTO.WalletHistoryDto;
using WalletService.Models.Requests.WalletHistoryRequest;

namespace WalletService.Core.Services.IServices;

public interface IWalletHistoryService
{
    Task<IEnumerable<WalletHistoryDto>> GetAllWalletsHistoriesAsync();
    Task<WalletHistoryDto?> GetWalletHistoriesByIdAsync(int id);
    Task<WalletHistoryDto?> CreateWalletHistoriesAsync(WalletHistoryRequest request);
    Task<WalletHistoryDto?> UpdateWalletHistoriesAsync(int id, WalletHistoryRequest request);
    Task<WalletHistoryDto?> DeleteWalletHistoriesAsync(int id);
}