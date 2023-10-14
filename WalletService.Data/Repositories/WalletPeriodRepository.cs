using Microsoft.EntityFrameworkCore;
using WalletService.Data.Database;
using WalletService.Data.Database.Models;
using WalletService.Data.Repositories.IRepositories;

namespace WalletService.Data.Repositories;

public class WalletPeriodRepository : BaseRepository, IWalletPeriodRepository
{
    public WalletPeriodRepository(WalletServiceDbContext context) : base(context) { }

    public Task<List<WalletsPeriods>> GetAllWalletsPeriods()
        => Context.WalletsPeriods.AsNoTracking().ToListAsync();

    public Task<WalletsPeriods?> GetWalletPeriodById(int id)
        => Context.WalletsPeriods.FirstOrDefaultAsync(x => x.Id == id);


    public async Task<IEnumerable<WalletsPeriods>> CreateWalletPeriodAsync(IEnumerable<WalletsPeriods> request)
    {
        var today          = DateTime.Now;
        var createdPeriods = new List<WalletsPeriods>();

        foreach (var period in request)
        {
            period.CreatedAt = today;
            period.UpdatedAt = today;

            await Context.AddAsync(period);
            createdPeriods.Add(period);
        }

        await Context.SaveChangesAsync();

        return createdPeriods;
    }

    public async Task<IEnumerable<WalletsPeriods>> UpdateWalletPeriodsAsync(IEnumerable<WalletsPeriods> request)
    {
        var today          = DateTime.Now;
        var updatedPeriods = new List<WalletsPeriods>();

        foreach (var period in request)
        {
            period.CreatedAt = today;
            period.UpdatedAt = today;

            Context.WalletsPeriods.Update(period);
            updatedPeriods.Add(period);
        }

        await Context.SaveChangesAsync();

        return updatedPeriods;
    }


    public async Task<WalletsPeriods> DeleteWalletPeriodAsync(WalletsPeriods request)
    {
        request.DeletedAt = DateTime.Now;

        Context.WalletsPeriods.Update(request);
        await Context.SaveChangesAsync();

        return request;
    }
}