using AutoMapper;
using WalletService.Core.PaymentStrategies.IPaymentStrategies;
using WalletService.Core.Services.IServices;
using WalletService.Data.Repositories.IRepositories;
using WalletService.Models.DTO.WalletModel1ADto;
using WalletService.Models.Requests.WalletRequest;

namespace WalletService.Core.Services;

public class WalletModel1AService : BaseService, IWalletModel1AService
{
    private readonly IWalletModel1ARepository       _walletModel1ARepository;
    private readonly IBalancePaymentStrategyModel1A _balancePaymentStrategyModel1A;

    public WalletModel1AService(IMapper mapper, IWalletModel1ARepository walletModel1ARepository,
        IBalancePaymentStrategyModel1A  balancePaymentStrategyModel1A) : base(mapper)
    {
        _walletModel1ARepository       = walletModel1ARepository;
        _balancePaymentStrategyModel1A = balancePaymentStrategyModel1A;
    }

    public async Task<BalanceInformationModel1ADto> GetBalanceInformationByAffiliateId(int affiliateId)
    {
        var availableBalance  = await _walletModel1ARepository.GetAvailableBalanceByAffiliateId(affiliateId);
        var totalAcquisitions = await _walletModel1ARepository.GetTotalAcquisitionsByAffiliateId(affiliateId);
        var reverseBalance    = await _walletModel1ARepository.GetReverseBalanceByAffiliateId(affiliateId);
        var serviceBalance    = await _walletModel1ARepository.GetTotalServiceBalance(affiliateId);

        var response = new BalanceInformationModel1ADto
        {
            AvailableBalance     = availableBalance,
            ReverseBalance       = reverseBalance ?? 0,
            TotalAcquisitions    = totalAcquisitions ?? 0,
            TotalCommissionsPaid = 0,
            ServiceBalance       = Math.Round(serviceBalance ?? 0, 2)
        };

        if (response.ReverseBalance == 0m) return response;

        response.AvailableBalance -= response.ReverseBalance;

        return response;
    }

    public async Task<bool> PayWithMyBalance(WalletRequest request)
    {
        if(request.ProductsList.Count == 0)
            return false;
        
        var response = await _balancePaymentStrategyModel1A.ExecuteEcoPoolPayment(request);

        return response;
    }
}