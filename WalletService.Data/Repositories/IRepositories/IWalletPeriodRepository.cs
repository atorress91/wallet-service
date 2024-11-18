using WalletService.Data.Database.Models;

namespace WalletService.Data.Repositories.IRepositories;

public interface IWalletPeriodRepository
{
    Task<List<WalletsPeriod>> GetAllWalletsPeriods();
    Task<WalletsPeriod?> GetWalletPeriodById(int                                          id);
    Task<IEnumerable<WalletsPeriod>> CreateWalletPeriodAsync(IEnumerable<WalletsPeriod>  request);
    Task<IEnumerable<WalletsPeriod>> UpdateWalletPeriodsAsync(IEnumerable<WalletsPeriod> request);
    Task<WalletsPeriod> DeleteWalletPeriodAsync(WalletsPeriod                            request);
}