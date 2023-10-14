using Microsoft.EntityFrameworkCore;
using WalletService.Data.Database;
using WalletService.Data.Database.Models;

namespace WalletService.Data.Repositories;

public class WalletRetentionConfigRepository : BaseRepository, IWalletRetentionConfigRepository
{
    public WalletRetentionConfigRepository(WalletServiceDbContext context) : base(context) { }

    public Task<List<WalletsRetentionsConfigs>> GetAllWalletsRetentionConfig()
        => Context.WalletsRetentionsConfigs.AsNoTracking().ToListAsync();

    public Task<WalletsRetentionsConfigs?> GetWalletRetentionConfigById(int id)
        => Context.WalletsRetentionsConfigs.FirstOrDefaultAsync(x => x.Id == id);

    public async Task<IEnumerable<WalletsRetentionsConfigs>> CreateWalletRetentionConfigAsync(IEnumerable<WalletsRetentionsConfigs> request)
    {
        var today                  = DateTime.Now;
        var createRetentionConfigs = new List<WalletsRetentionsConfigs>();

        foreach (var periodRetention in request)
        {
            periodRetention.CreatedAt = today;
            periodRetention.UpdatedAt = today;

            await Context.AddAsync(periodRetention);
            createRetentionConfigs.Add(periodRetention);
        }

        await Context.SaveChangesAsync();

        return createRetentionConfigs;
    }

    public async Task<IEnumerable<WalletsRetentionsConfigs>> UpdateWalletRetentionConfigAsync(IEnumerable<WalletsRetentionsConfigs> request)
    {
        var today                   = DateTime.Now;
        var updatedRetentionConfigs = new List<WalletsRetentionsConfigs>();

        foreach (var periodRetention in request)
        {
            periodRetention.CreatedAt = today;
            periodRetention.UpdatedAt = today;

            Context.WalletsRetentionsConfigs.Update(periodRetention);
            updatedRetentionConfigs.Add(periodRetention);
        }

        await Context.SaveChangesAsync();

        return updatedRetentionConfigs;
    }


    public async Task<WalletsRetentionsConfigs> DeleteWalletRetentionConfigAsync(WalletsRetentionsConfigs request)
    {
        request.DeletedAt = DateTime.Now;

        Context.WalletsRetentionsConfigs.Update(request);
        await Context.SaveChangesAsync();

        return request;
    }
}