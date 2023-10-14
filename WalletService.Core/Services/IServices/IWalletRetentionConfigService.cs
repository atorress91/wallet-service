using WalletService.Data.Database.Models;
using WalletService.Models.DTO.WalletRetentionConfigDto;
using WalletService.Models.Requests.WalletRetentionConfigRequest;

namespace WalletService.Core.Services.IServices;

public interface IWalletRetentionConfigService
{
    Task<IEnumerable<WalletRetentionConfigDto>> GetAllWalletsRetentionConfig();
    Task<WalletRetentionConfigDto?> GetWalletRetentionConfigById(int                                                       id);
    Task<IEnumerable<WalletRetentionConfigDto>> CreateWalletRetentionConfigAsync(IEnumerable<WalletRetentionConfigRequest> request);
    Task<WalletRetentionConfigDto?> DeleteWalletRetentionConfigAsync(int id);
}