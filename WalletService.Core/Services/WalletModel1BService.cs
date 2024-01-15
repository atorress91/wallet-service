using AutoMapper;
using WalletService.Core.Services.IServices;
using WalletService.Data.Repositories.IRepositories;
using WalletService.Models.DTO.WalletModel1BDto;

namespace WalletService.Core.Services;

public class WalletModel1BService : BaseService, IWalletModel1BService
{
    private readonly IWalletModel1BRepository _walletModel1BRepository;

    public WalletModel1BService(IMapper mapper, IWalletModel1BRepository walletModel1BRepository) : base(mapper)
    {
        _walletModel1BRepository = walletModel1BRepository;
    }

    public async Task<BalanceInformationModel1BDto> GetBalanceInformationByAffiliateId(int affiliateId)
    {
        var availableBalance = await _walletModel1BRepository.GetAvailableBalanceByAffiliateId(affiliateId);

        var response = new BalanceInformationModel1BDto
        {
            AvailableBalance     = availableBalance,
            ReverseBalance       = 0,
            TotalAcquisitions    = 0,
            TotalCommissionsPaid = 0
        };

        return response;
    }
}