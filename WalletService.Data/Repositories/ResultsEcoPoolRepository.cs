using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using WalletService.Data.Database;
using WalletService.Data.Database.Models;
using WalletService.Data.Repositories.IRepositories;
using WalletService.Models.Configuration;


namespace WalletService.Data.Repositories;

public class ResultsEcoPoolRepository : BaseRepository, IResultsEcoPoolRepository
{
    private readonly ApplicationConfiguration _appSettings;
    public ResultsEcoPoolRepository(IOptions<ApplicationConfiguration> appSettings, WalletServiceDbContext context) : base(context)
        => _appSettings = appSettings.Value;

    public Task<List<ResultsEcoPool>> GetAllResultsEcoPool()
        => Context.ResultsEcoPool
            .Include(x => x.ResultEcoPoolLevels).AsNoTracking()
            .Where(x => x.CompletedAt == null).ToListAsync();

    public Task<List<ResultsEcoPool>> GetResultsEcoPoolToPayment()
    {
        return Context.ResultsEcoPool
            .Include(x => x.EcoPoolConfiguration)
            .Include(x => x.ResultEcoPoolLevels)
            .Where(x => x.EcoPoolConfiguration.CompletedAt == null).ToListAsync();
    }
    
    public Task<List<ResultsModelTwo>> GetResultsModelTwoToPayment()
    {
        return Context.ResultsModelTwo
            .Include(x => x.ResultsModelTwoLevels)
            .Where(x => x.CompletedAt == null).ToListAsync();
    }
}