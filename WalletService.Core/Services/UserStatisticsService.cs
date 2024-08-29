using WalletService.Core.Caching;
using WalletService.Core.Services.IServices;
using WalletService.Data.Adapters.IAdapters;
using WalletService.Data.Repositories.IRepositories;
using WalletService.Models.Constants;
using WalletService.Models.DTO.AffiliateInformation;

namespace WalletService.Core.Services;

public class UserStatisticsService : IUserStatisticsService
{
    private readonly IInvoiceRepository _invoiceRepository;
    private readonly IResultsEcoPoolRepository _resultsEcoPoolRepository;
    private readonly IAccountServiceAdapter _accountServiceAdapter;
    private readonly RedisCache               _redisCache;
    private readonly IBrandService            _brandService;

    public UserStatisticsService(IInvoiceRepository invoiceRepository, 
        IAccountServiceAdapter accountServiceAdapter,
        IResultsEcoPoolRepository resultsEcoPoolRepository, RedisCache redisCache,IBrandService brandService)
    {
        _invoiceRepository = invoiceRepository;
        _accountServiceAdapter = accountServiceAdapter;
        _resultsEcoPoolRepository = resultsEcoPoolRepository;
        _redisCache = redisCache;
        _brandService = brandService;
    }
    public async Task<UserStatistics> GetUserStatisticsAsync(int userId)
    {
        var key = string.Format(CacheKeys.ModelInformation, userId);
        var isExists = await _redisCache.KeyExists(key);
        UserStatistics result;
        if (!isExists)
        {
            var pointsModel4 = await _invoiceRepository.Model4StatisticsByUser(userId);
            var amountModel1A = await _invoiceRepository.CountDetailsByPaymentGroup(7, userId,_brandService.BrandId);
            var amountModel1B = await _invoiceRepository.CountDetailsByPaymentGroup(8, userId,_brandService.BrandId);
            var amountModel2 = await _invoiceRepository.CountDetailsByPaymentGroup(2, userId,_brandService.BrandId);
            var amountModel3 = await _invoiceRepository.CountDetailsModel3ByPaymentGroup(userId, _brandService.BrandId);
            var accountInformation = await _accountServiceAdapter.NetworkDetails(userId,_brandService.BrandId);
            var residualModel1A= await _resultsEcoPoolRepository.SumResidualModel1AByUserId(userId);
            var passiveModel1A= await _resultsEcoPoolRepository.SumPassiveModel1AByUserId(userId);
            var residualModel1B= await _resultsEcoPoolRepository.SumResidualModel1BByUserId(userId);
            var passiveModel1B= await _resultsEcoPoolRepository.SumPassiveModel1BByUserId(userId);
            var residualModel2= await _resultsEcoPoolRepository.SumResidualModel2ByUserId(userId);
            var passiveModel2= await _resultsEcoPoolRepository.SumPassiveModel2ByUserId(userId);
            var residualModel3= await _resultsEcoPoolRepository.SumResidualModel3ByUserId(userId);
            var percentageModel3= await _resultsEcoPoolRepository.SumPercentageModel3ByUserId(userId);
            
            result = new UserStatistics
            {
                AmountPoolModel1A = amountModel1A,
                AmountPoolModel1B = amountModel1B,
                AmountPoolModel2 = amountModel2,
                AmountPoolModel3 = amountModel3,
                VolumeLeftModel4 = pointsModel4.Sum(s => s.CreditLeft - s.DebitLeft),
                VolumeRightModel4 = pointsModel4.Sum(s => s.CreditRight - s.DebitRight),
                AmountChildrenLeftModel4 = accountInformation.Model4.LeftCount,
                AmountChildrenRightModel4 = accountInformation.Model4.RightCount,
                AmountUsersDirectModel123 = accountInformation.Model123.DirectAffiliates,
                AmountUsersNetworkModel123 = accountInformation.Model123.IndirectAffiliates,
                AmountUsersDirectModel5 = accountInformation.Model5.DirectAffiliates,
                AmountUsersNetworkModel5 = accountInformation.Model5.IndirectAffiliates,
                AmountUsersDirectModel6 = accountInformation.Model6.DirectAffiliates,
                AmountUsersNetworkModel6 = accountInformation.Model6.IndirectAffiliates,
                AmountResidualModel1A = residualModel1A,
                AmountPassiveModel1A = passiveModel1A,
                AmountResidualModel1B = residualModel1B,
                AmountPassiveModel1B = passiveModel1B,
                AmountResidualModel2 = residualModel2,
                AmountPassiveModel2 = passiveModel2,
                AmountResidualModel3 = residualModel3,
                AmountPercentageModel3 = percentageModel3
            };
            await _redisCache.Set(key, result, TimeSpan.FromHours(1));
            return result;
        }
        result = await _redisCache.Get<UserStatistics>(key) ?? new UserStatistics();

        return result;
    }
}