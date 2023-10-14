using Microsoft.EntityFrameworkCore;
using WalletService.Data.Database;
using WalletService.Data.Database.Models;
using WalletService.Data.Repositories.IRepositories;

namespace WalletService.Data.Repositories;

public class EcoPoolConfigurationRepository : BaseRepository, IEcoPoolConfigurationRepository
{
    public EcoPoolConfigurationRepository(WalletServiceDbContext context) : base(context) { }

    public Task<EcoPoolConfiguration?> GetConfiguration()
        => Context.EcoPoolConfiguration.Include(x => x.Levels).FirstOrDefaultAsync(x => !x.CompletedAt.HasValue);

    public Task<EcoPoolConfiguration> GetProgressPercentage(int configurationId)
    {
        return Context.EcoPoolConfiguration
            .FirstAsync(x => x.Id == configurationId);
    }

    public async Task<EcoPoolConfiguration> CreateConfiguration(EcoPoolConfiguration poolConfiguration)
    {
        poolConfiguration.UpdatedAt = DateTime.Now;
        poolConfiguration.CreatedAt = DateTime.Now;

        await Context.EcoPoolConfiguration.AddAsync(poolConfiguration);
        await Context.SaveChangesAsync();

        return poolConfiguration;
    }

    public Task CreateConfigurationLevels(IEnumerable<EcoPoolLevels> levels)
    {
        Context.EcoPoolLevels.AddRange(levels);

        return Context.SaveChangesAsync();
    }

    public Task UpdateConfigurationLevels(IEnumerable<EcoPoolLevels> levels)
    {
        Context.EcoPoolLevels.UpdateRange(levels);

        return Context.SaveChangesAsync();
    }

    public async Task<EcoPoolConfiguration> UpdateConfiguration(EcoPoolConfiguration poolConfiguration)
    {
        poolConfiguration.UpdatedAt = DateTime.Now;

        Context.EcoPoolConfiguration.Update(poolConfiguration);
        await Context.SaveChangesAsync();

        return poolConfiguration;
    }

    public async Task DeleteAllLevelsConfiguration(int configurationId)
    {
        var entities = await Context.EcoPoolLevels.Where(x => x.EcoPoolConfigurationId == configurationId).ToListAsync();
        if (entities is { Count: > 0 })
            Context.EcoPoolLevels.RemoveRange(entities);
    }

}