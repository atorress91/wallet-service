using WalletService.Models.DTO.WalletModel1ADto;

namespace WalletService.Core.Services.IServices;

public interface IWalletModel1AService
{
    Task<BalanceInformationModel1ADto> GetBalanceInformationByAffiliateId(int affiliateId);
}