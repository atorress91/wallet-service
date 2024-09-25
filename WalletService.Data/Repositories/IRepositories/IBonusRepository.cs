using WalletService.Data.Database.Models;
using WalletService.Models.Requests.BonusRequest;

namespace WalletService.Data.Repositories.IRepositories;

public interface IBonusRepository
{
    Task<int> CreateBonus(BonusRequest request);
    Task<decimal> GetBonusAmountByAffiliateId(int affiliateId);
}