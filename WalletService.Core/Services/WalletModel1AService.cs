using AutoMapper;
using WalletService.Core.Caching;
using WalletService.Core.PaymentStrategies.IPaymentStrategies;
using WalletService.Core.Services.IServices;
using WalletService.Data.Adapters.IAdapters;
using WalletService.Data.Repositories.IRepositories;
using WalletService.Models.Constants;
using WalletService.Models.DTO.WalletModel1ADto;
using WalletService.Models.Requests.WalletRequest;

namespace WalletService.Core.Services;

public class WalletModel1AService : BaseService, IWalletModel1AService
{
    private readonly IWalletModel1ARepository       _walletModel1ARepository;
    private readonly IBalancePaymentStrategyModel1A _balancePaymentStrategyModel1A;
    private readonly RedisCache                    _redisCache;
    private readonly IAccountServiceAdapter        _accountServiceAdapter;
    private readonly IWalletModel1ARepository      _walletRepositoryModel1A;

    public WalletModel1AService(
        IMapper                         mapper, 
        IWalletModel1ARepository        walletModel1ARepository,
        RedisCache                      redisCache,
        IBalancePaymentStrategyModel1A  balancePaymentStrategyModel1A,
        IAccountServiceAdapter accountServiceAdapter,
        IWalletModel1ARepository walletRepositoryModel1A) : base(mapper)
    {
        _walletModel1ARepository       = walletModel1ARepository;
        _balancePaymentStrategyModel1A = balancePaymentStrategyModel1A;
        _redisCache                    = redisCache;
        _accountServiceAdapter         = accountServiceAdapter;
        _walletRepositoryModel1A       = walletRepositoryModel1A;
    }

    public async Task<BalanceInformationModel1ADto> GetBalanceInformationByAffiliateId(int affiliateId)
    {
        var                          key       = string.Format(CacheKeys.BalanceInformationModel1A, affiliateId);
        var                          existsKey =  await _redisCache.KeyExists(key);
        BalanceInformationModel1ADto response;
        if (!existsKey)
        {
            var availableBalance     = await _walletModel1ARepository.GetAvailableBalanceByAffiliateId(affiliateId);
            var totalAcquisitions    = await _walletModel1ARepository.GetTotalAcquisitionsByAffiliateId(affiliateId);
            var reverseBalance       = await _walletModel1ARepository.GetReverseBalanceByAffiliateId(affiliateId);
            var serviceBalance       = await _walletModel1ARepository.GetTotalServiceBalance(affiliateId);
            var totalCommissionsPaid = await _walletModel1ARepository.GetTotalCommissionsPaidBalance(affiliateId);

            response = new BalanceInformationModel1ADto
            {
                AvailableBalance     = availableBalance,
                ReverseBalance       = reverseBalance ?? 0,
                TotalAcquisitions    = totalAcquisitions ?? 0,
                TotalCommissionsPaid = totalCommissionsPaid ?? 0,
                ServiceBalance       = serviceBalance ?? 0
            };

            if (response.ReverseBalance != 0m)
                response.AvailableBalance -= response.ReverseBalance;

            await _redisCache.Set(key, response);
            return response;
        }
        
        response = await _redisCache.Get<BalanceInformationModel1ADto>(key) ?? new BalanceInformationModel1ADto();
        return response;
    }

    public async Task<bool> PayWithMyBalance(WalletRequest request)
    {
        if(request.ProductsList.Count == 0)
            return false;
        
        var response = await _balancePaymentStrategyModel1A.ExecuteEcoPoolPayment(request);

        if (response)
            await RemoveCacheKey(request.AffiliateId);

        return response;
    }

    public async Task<bool> PayWithMyServiceBalance(WalletRequest request)
    {
        if (request.ProductsList.Count == 0)
            return false;

        var response = await _balancePaymentStrategyModel1A.ExecuteEcoPoolPaymentWithServiceBalance(request);

        if (response)
            await RemoveCacheKey(request.AffiliateId);
        
        return response;
    }
    
    private async Task RemoveCacheKey(int affiliateId)
    {
        var key       = string.Format(CacheKeys.BalanceInformationModel1A, affiliateId);
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

        var result = await _walletRepositoryModel1A.CreditServiceBalanceTransaction(credit);
        if (!result)
            return false;

        await RemoveCacheKey(user.Id);
        return true;
    }
}