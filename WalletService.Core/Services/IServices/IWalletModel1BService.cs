using WalletService.Models.DTO.WalletModel1BDto;
using WalletService.Models.Requests.WalletRequest;

namespace WalletService.Core.Services.IServices;

public interface IWalletModel1BService
{
    Task<BalanceInformationModel1BDto> GetBalanceInformationByAffiliateId(int affiliateId);
    Task<bool>                         PayWithMyBalance(WalletRequest         request);
}