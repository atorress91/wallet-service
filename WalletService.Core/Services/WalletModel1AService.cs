using AutoMapper;
using WalletService.Core.Services.IServices;
using WalletService.Data.Repositories.IRepositories;
using WalletService.Models.DTO.WalletModel1ADto;

namespace WalletService.Core.Services;

public class WalletModel1AService : BaseService, IWalletModel1AService
{
    private readonly IWalletModel1ARepository _walletModel1ARepository;

    public WalletModel1AService(IMapper mapper, IWalletModel1ARepository walletModel1ARepository) : base(mapper)
    {
        _walletModel1ARepository = walletModel1ARepository;
    }
    
    public async Task<BalanceInformationModel1ADto> GetBalanceInformationByAffiliateId(int affiliateId)
    {
        var availableBalance = await _walletModel1ARepository.GetAvailableBalanceByAffiliateId(affiliateId);

        var response = new BalanceInformationModel1ADto
        {
            AvailableBalance     = availableBalance,
            ReverseBalance       = 0,
            TotalAcquisitions    = 0,
            TotalCommissionsPaid = 0
        };
        
        return response;
    }
}