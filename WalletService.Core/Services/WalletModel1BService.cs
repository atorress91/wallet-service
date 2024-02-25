using AutoMapper;
using WalletService.Core.PaymentStrategies.IPaymentStrategies;
using WalletService.Core.Services.IServices;
using WalletService.Data.Repositories.IRepositories;
using WalletService.Models.DTO.WalletModel1BDto;
using WalletService.Models.Requests.WalletRequest;

namespace WalletService.Core.Services;

public class WalletModel1BService : BaseService, IWalletModel1BService
{
    private readonly IWalletModel1BRepository _walletModel1BRepository;
    private readonly IBalancePaymentStrategyModel1B _balancePaymentStrategyModel1B;

    public WalletModel1BService(IMapper mapper, IWalletModel1BRepository walletModel1BRepository,
        IBalancePaymentStrategyModel1B balancePaymentStrategyModel1B) : base(mapper)
    {
        _walletModel1BRepository = walletModel1BRepository;
        _balancePaymentStrategyModel1B = balancePaymentStrategyModel1B;
    }

    public async Task<BalanceInformationModel1BDto> GetBalanceInformationByAffiliateId(int affiliateId)
    {
        var availableBalance     = await _walletModel1BRepository.GetAvailableBalanceByAffiliateId(affiliateId);
        var totalAcquisitions    = await _walletModel1BRepository.GetTotalAcquisitionsByAffiliateId(affiliateId);
        var reverseBalance       = await _walletModel1BRepository.GetReverseBalanceByAffiliateId(affiliateId);
        var serviceBalance       = await _walletModel1BRepository.GetTotalServiceBalance(affiliateId);
        var totalCommissionsPaid = await _walletModel1BRepository.GetTotalCommissionsPaidBalance(affiliateId);

        var response = new BalanceInformationModel1BDto
        {
            AvailableBalance     = availableBalance,
            ReverseBalance       = reverseBalance ?? 0,
            TotalAcquisitions    = totalAcquisitions ?? 0,
            TotalCommissionsPaid = totalCommissionsPaid ?? 0,
            ServiceBalance       = serviceBalance ?? 0,
        };

        if (response.ReverseBalance == 0m) return response;

        response.AvailableBalance -= response.ReverseBalance;

        return response;
    }

    public async Task<bool> PayWithMyBalance(WalletRequest request)
    {
        if (request.ProductsList.Count == 0)
            return false;

        var response = await _balancePaymentStrategyModel1B.ExecuteEcoPoolPayment(request);

        return response;
    }

    public async Task<bool> PayWithMyServiceBalance(WalletRequest request)
    {
        if (request.ProductsList.Count == 0)
            return false;

        var response = await _balancePaymentStrategyModel1B.ExecuteEcoPoolPaymentWithServiceBalance(request);
        
        return response;
    }
}