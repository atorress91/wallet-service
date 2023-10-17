using WalletService.Data.Database.Models;

namespace WalletService.Data.Repositories.IRepositories;

public interface ILeaderBoardRepository
{
    Task<List<LeaderBoardModel5>> GetMatrixModel5();
    Task<List<LeaderBoardModel6>> GetMatrixModel6();
    Task AddCustomersToModel5(ICollection<LeaderBoardModel5> leaderBoardModel5);
    Task AddCustomersToModel6(ICollection<LeaderBoardModel6> leaderBoardModel6);

    Task CleanLeaderBoardModel5();
    Task CleanLeaderBoardModel6();
}