using WalletService.Models.DTO.ResultsEcoPoolDto;

namespace WalletService.Core.Services.IServices;

public interface IResultsEcoPoolService
{
    Task<IEnumerable<ResultsEcoPoolDto>> GetAllResultsEcoPoolAsync();
}