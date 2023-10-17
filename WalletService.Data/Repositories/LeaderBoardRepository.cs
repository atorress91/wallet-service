using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore;
using WalletService.Data.Database;
using WalletService.Data.Database.Models;
using WalletService.Data.Repositories.IRepositories;

namespace WalletService.Data.Repositories;

public class LeaderBoardRepository : BaseRepository, ILeaderBoardRepository
{
    protected LeaderBoardRepository(WalletServiceDbContext context) : base(context)
    {
    }

    public Task<List<LeaderBoardModel5>> GetMatrixModel5()
        => Context.LeaderBoardModel5.OrderBy(x => x.GradingPosition).ToListAsync();
    

    public Task<List<LeaderBoardModel6>> GetMatrixModel6()
        => Context.LeaderBoardModel6.OrderBy(x => x.GradingPosition).ToListAsync();

    public Task AddCustomersToModel5(ICollection<LeaderBoardModel5> leaderBoardModel5)
    {
        Context.LeaderBoardModel5.AddRange(leaderBoardModel5);
        return Context.SaveChangesAsync();
    }

    public Task AddCustomersToModel6(ICollection<LeaderBoardModel6> leaderBoardModel6)
    {
        Context.LeaderBoardModel6.AddRange(leaderBoardModel6);
        return Context.SaveChangesAsync();
    }

    public Task CleanLeaderBoardModel5()
    {
        var sql = FormattableStringFactory.Create("TRUNCATE TABLE [LeaderBoardModel5]");
        Context.Database.ExecuteSqlInterpolated(sql);

        return Task.CompletedTask;
    }

    public Task CleanLeaderBoardModel6()
    {
        var sql = FormattableStringFactory.Create("TRUNCATE TABLE [LeaderBoardModel6]");
        Context.Database.ExecuteSqlInterpolated(sql);

        return Task.CompletedTask;
    }
}