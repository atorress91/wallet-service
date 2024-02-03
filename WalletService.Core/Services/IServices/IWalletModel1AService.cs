using WalletService.Models.DTO.WalletModel1ADto;
using WalletService.Models.Requests.WalletRequest;

namespace WalletService.Core.Services.IServices;

public interface IWalletModel1AService
{
    Task<BalanceInformationModel1ADto> GetBalanceInformationByAffiliateId(int affiliateId);
    Task<bool>                         PayWithMyBalance(WalletRequest         request);
    Task<bool> PayWithMyServiceBalance(WalletRequest request);
}