using Microsoft.EntityFrameworkCore;
using WalletService.Data.Database;
using WalletService.Data.Database.Models;
using WalletService.Data.Repositories.IRepositories;

namespace WalletService.Data.Repositories;

public class WalletHistoryRepository : BaseRepository, IWalletHistoryRepository
{
    public WalletHistoryRepository(WalletServiceDbContext context) : base(context) { }
    public Task<List<WalletsHistories>> GetAllWalletsHistoriesAsync()
        => Context.WalletsHistories.AsNoTracking().ToListAsync();

    public Task<WalletsHistories?> GetWalletHistoriesByIdAsync(int id)
        => Context.WalletsHistories.FirstOrDefaultAsync(x => x.Id == id);

    public async Task<WalletsHistories> CreateWalletHistoriesAsync(WalletsHistories request)
    {
        var today = DateTime.Now;
        request.CreatedAt = today;
        request.UpdatedAt = today;

        await Context.AddAsync(request);
        await Context.SaveChangesAsync();

        return request;
    }

    public async Task<WalletsHistories> UpdateWalletHistoriesAsync(WalletsHistories request)
    {
        var today = DateTime.Now;
        request.UpdatedAt = today;
        Context.WalletsHistories.Update(request);
        await Context.SaveChangesAsync();

        return request;
    }

    public async Task<WalletsHistories> DeleteWalletHistoriesAsync(WalletsHistories request)
    {
        request.DeletedAt = DateTime.Now;

        Context.WalletsHistories.Update(request);
        await Context.SaveChangesAsync();

        return request;
    }
}