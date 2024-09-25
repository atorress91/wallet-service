using Microsoft.EntityFrameworkCore;
using WalletService.Data.Database;
using WalletService.Data.Repositories.IRepositories;
using WalletService.Models.Requests.BonusRequest;

namespace WalletService.Data.Repositories;

public class BonusRepository : BaseRepository, IBonusRepository
{
    public BonusRepository(WalletServiceDbContext context) : base(context)
    {
    }

    public async Task<int> CreateBonus(BonusRequest request)
    {
        var result = await Context.Database.ExecuteSqlInterpolatedAsync(
            $"EXEC ManageBonus @InvoiceId = {request.InvoiceId}, @AffiliateId = {request.AffiliateId}, @Amount = {request.Amount}, @Comment = {request.Comment}");

        return result; 
    }
    
    public async Task<decimal> GetBonusAmountByAffiliateId(int affiliateId)
    {
        var amount = await Context.Bonuses
            .Where(x => x.AffiliateId == affiliateId)
            .Select(x => x.CurrentAmount)
            .FirstOrDefaultAsync();

        return amount;
    }

}