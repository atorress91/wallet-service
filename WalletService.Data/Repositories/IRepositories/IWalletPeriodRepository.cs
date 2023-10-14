using WalletService.Data.Database.Models;

namespace WalletService.Data.Repositories.IRepositories;

public interface IWalletPeriodRepository
{
    Task<List<WalletsPeriods>> GetAllWalletsPeriods();
    Task<WalletsPeriods?> GetWalletPeriodById(int                                          id);
    Task<IEnumerable<WalletsPeriods>> CreateWalletPeriodAsync(IEnumerable<WalletsPeriods>  request);
    Task<IEnumerable<WalletsPeriods>> UpdateWalletPeriodsAsync(IEnumerable<WalletsPeriods> request);
    Task<WalletsPeriods> DeleteWalletPeriodAsync(WalletsPeriods                            request);
}