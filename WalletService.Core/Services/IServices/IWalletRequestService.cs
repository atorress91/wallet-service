using WalletService.Data.Database.Models;
using WalletService.Models.DTO.WalletRequestDto;
using WalletService.Models.Requests.WalletRequestRequest;
using WalletService.Models.Responses;

namespace WalletService.Core.Services.IServices;

public interface IWalletRequestService
{
    Task<IEnumerable<WalletRequestDto>> GetAllWalletsRequests();
    Task<IEnumerable<WalletRequestDto>?> GetWalletRequestById(int                    id);
    Task<WalletRequestDto?> CreateWalletRequestAsync(WalletRequestRequest            request);
    Task CancelWalletRequestsAsync(List<int>                                         id);
    Task<ServicesResponse?> ProcessOption(int                                        option, List<int> ids);
    Task<WalletRequestDto?> CreateWalletRequestRevert(WalletRequestRevertTransaction request);
    Task<IEnumerable<WalletRequestDto>?> GetAllWalletRequestRevertTransaction();
    Task<bool> AdministrativePaymentAsync(WalletsRequests[] requests);

}