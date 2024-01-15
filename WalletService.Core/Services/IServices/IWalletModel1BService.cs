using WalletService.Models.DTO.WalletModel1BDto;

namespace WalletService.Core.Services.IServices;

public interface IWalletModel1BService
{
    Task<BalanceInformationModel1BDto> GetBalanceInformationByAffiliateId(int affiliateId);
}