using Microsoft.EntityFrameworkCore;
using WalletService.Data.Database;
using WalletService.Data.Database.Models;
using WalletService.Data.Repositories.IRepositories;
using WalletService.Models.Enums;

namespace WalletService.Data.Repositories;

public class WalletRequestRepository : BaseRepository, IWalletRequestRepository
{
    public WalletRequestRepository(WalletServiceDbContext context) : base(context) { }
    public Task<List<WalletsRequests>> GetAllWalletsRequests()
        => Context.WalletsRequests.Where(x => x.Type == WalletRequestType.withdrawal_request.ToString() && x.Status == 0).AsNoTracking().ToListAsync();

    public Task<List<WalletsRequests>> GetAllWalletRequestRevertTransaction()
        => Context.WalletsRequests.Where(x => x.Type == WalletRequestType.revert_invoice_request.ToString()).AsNoTracking().ToListAsync();

    public Task<List<WalletsRequests>> GetAllWalletRequestByAffiliateId(int id)
        => Context.WalletsRequests.Where(x => x.AffiliateId == id).AsNoTracking().ToListAsync();

    public async Task<decimal> GetTotalWalletRequestAmountByAffiliateId(int id)
    {
        var walletRequests = await Context.WalletsRequests
            .Where(x => x.AffiliateId == id && x.Status == 0 && x.Type == WalletRequestType.withdrawal_request.ToString())
            .ToListAsync();

        decimal totalAmount = walletRequests.Sum(x => x.Amount);
        return totalAmount;
    }

	public async Task<decimal> GetTotalWalletRequestAmount()
	{
		var walletRequests = await Context.WalletsRequests
			.Where(x => x.Status == 0 && x.Type == WalletRequestType.withdrawal_request.ToString())
			.ToListAsync();

		decimal totalAmount = walletRequests.Sum(x => x.Amount);
		return totalAmount;
	}


	public Task<List<WalletsRequests>> GetWalletRequestsByIds(List<int> ids)
        => Context.WalletsRequests.Where(x => ids.Contains(x.Id)).ToListAsync();

    public Task<WalletsRequests?> GetWalletRequestsByInvoiceNumber(int id)
        => Context.WalletsRequests.FirstOrDefaultAsync(x => x.InvoiceNumber == id);

    public async Task<WalletsRequests?> CreateWalletRequestAsync(WalletsRequests request)
    {
        var today = DateTime.Now;
        request.CreatedAt = today;
        request.UpdatedAt = today;

        await Context.AddAsync(request);
        await Context.SaveChangesAsync();

        return request;
    }

    public async Task UpdateBulkWalletRequestsAsync(List<WalletsRequests> requests)
    {
        const int take         = 1000;
        var       packageCount = requests.Count / take;

        for (var i = 0; i <= packageCount; i++)
        {
            var packageList = requests.Skip(i * take).Take(take).ToList();
            Context.WalletsRequests.UpdateRange(packageList);
        }

        await Context.SaveChangesAsync();
    }

    public async Task<WalletsRequests> UpdateWalletRequestsAsync(WalletsRequests requests)
    {
        var today = DateTime.Now;
        requests.UpdatedAt = today;
        Context.WalletsRequests.Update(requests);
        await Context.SaveChangesAsync();

        return requests;
    }


    public async Task<WalletsRequests> DeleteWalletRequestAsync(WalletsRequests request)
    {
        request.DeletedAt = DateTime.Now;

        Context.WalletsRequests.Update(request);
        await Context.SaveChangesAsync();

        return request;
    }
}