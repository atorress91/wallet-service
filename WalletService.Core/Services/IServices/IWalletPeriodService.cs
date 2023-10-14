using WalletService.Data.Database.Models;
using WalletService.Models.DTO.WalletPeriodDto;
using WalletService.Models.Requests.WalletPeriodRequest;

namespace WalletService.Core.Services.IServices;

public interface IWalletPeriodService
{
    Task<IEnumerable<WalletPeriodDto>> GetAllWalletsPeriods();
    Task<WalletPeriodDto?> GetWalletPeriodById(int                                              id);
    Task<IEnumerable<WalletPeriodDto>> CreateWalletPeriodAsync(IEnumerable<WalletPeriodRequest> request);
    Task<WalletPeriodDto?> DeleteWalletPeriodAsync(int id);
}