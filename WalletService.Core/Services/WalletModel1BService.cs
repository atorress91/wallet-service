using AutoMapper;
using WalletService.Core.Caching;
using WalletService.Core.PaymentStrategies.IPaymentStrategies;
using WalletService.Core.Services.IServices;
using WalletService.Data.Adapters.IAdapters;
using WalletService.Data.Repositories.IRepositories;
using WalletService.Models.Constants;
using WalletService.Models.DTO.WalletModel1BDto;
using WalletService.Models.Requests.WalletRequest;

namespace WalletService.Core.Services;

public class WalletModel1BService : BaseService, IWalletModel1BService
{
    private readonly IWalletModel1BRepository       _walletModel1BRepository;
    private readonly IBalancePaymentStrategyModel1B _balancePaymentStrategyModel1B;
    private readonly RedisCache                     _redisCache;
    private readonly IAccountServiceAdapter        _accountServiceAdapter;

    public WalletModel1BService(
        IMapper mapper, 
        IWalletModel1BRepository walletModel1BRepository,
        IBalancePaymentStrategyModel1B balancePaymentStrategyModel1B,
        RedisCache redisCache,IAccountServiceAdapter accountServiceAdapter) : base(mapper)
    {
        _walletModel1BRepository       = walletModel1BRepository;
        _balancePaymentStrategyModel1B = balancePaymentStrategyModel1B;
        _redisCache                    = redisCache;
        _accountServiceAdapter         = accountServiceAdapter;
    }

    public async Task<BalanceInformationModel1BDto> GetBalanceInformationByAffiliateId(int affiliateId)
    {
        var                          key       = string.Format(CacheKeys.BalanceInformationModel1B, affiliateId);
        var                          existsKey =  await _redisCache.KeyExists(key);
        BalanceInformationModel1BDto response;
        if (!existsKey)
        {
            var availableBalance     = await _walletModel1BRepository.GetAvailableBalanceByAffiliateId(affiliateId);
            var totalAcquisitions    = await _walletModel1BRepository.GetTotalAcquisitionsByAffiliateId(affiliateId);
            var reverseBalance       = await _walletModel1BRepository.GetReverseBalanceByAffiliateId(affiliateId);
            var serviceBalance       = await _walletModel1BRepository.GetTotalServiceBalance(affiliateId);
            var totalCommissionsPaid = await _walletModel1BRepository.GetTotalCommissionsPaidBalance(affiliateId);

            response = new BalanceInformationModel1BDto
            {
                AvailableBalance     = availableBalance,
                ReverseBalance       = reverseBalance ?? 0,
                TotalAcquisitions    = totalAcquisitions ?? 0,
                TotalCommissionsPaid = totalCommissionsPaid ?? 0,
                ServiceBalance       = serviceBalance ?? 0,
            };

            if (response.ReverseBalance != 0m)
                response.AvailableBalance -= response.ReverseBalance;
            
            await _redisCache.Set(key, response, TimeSpan.FromHours(1));
            return response;        
        }
        
        response = await _redisCache.Get<BalanceInformationModel1BDto>(key) ?? new BalanceInformationModel1BDto();
        return response;
    }

    public async Task<bool> PayWithMyBalance(WalletRequest request)
    {
        if (request.ProductsList.Count == 0)
            return false;

        var response = await _balancePaymentStrategyModel1B.ExecuteEcoPoolPayment(request);

        if (response)
            await RemoveCacheKey(request.AffiliateId);
        
        return response;
    }

    public async Task<bool> PayWithMyServiceBalance(WalletRequest request)
    {
        if (request.ProductsList.Count == 0)
            return false;

        var response = await _balancePaymentStrategyModel1B.ExecuteEcoPoolPaymentWithServiceBalance(request);
        
        if (response)
            await RemoveCacheKey(request.AffiliateId);
        
        return response;
    }
    
    private async Task RemoveCacheKey(int affiliateId)
    {
        var key       = string.Format(CacheKeys.BalanceInformationModel1B, affiliateId);
        var existsKey = await _redisCache.KeyExists(key);

        if (existsKey)
            await _redisCache.Delete(key);
    }
    
    public async Task<bool> CreateServiceBalanceAdmin(CreditTransactionAdminRequest request)
    {
        if (request.Amount == 0)
            return false;

        var user = await _accountServiceAdapter.GetUserInfo(request.AffiliateId);

        if (user is null)
            return false;

        var credit = new CreditTransactionRequest
        {
            AdminUserName     = Constants.AdminEcosystemUserName,
            AffiliateId       = user.Id,
            Concept           = Constants.AdminCredit,
            Credit            = request.Amount,
            AffiliateUserName = user.UserName,
            ConceptType       = Constants.AdminCredit,
            UserId            = Constants.AdminUserId
        };

        var result = await _walletModel1BRepository.CreditServiceBalanceTransaction(credit);
        if (!result)
            return false;

        await RemoveCacheKey(user.Id);
        return true;
    }
}