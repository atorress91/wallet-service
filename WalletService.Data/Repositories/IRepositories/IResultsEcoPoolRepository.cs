using WalletService.Data.Database.Models;


namespace WalletService.Data.Repositories.IRepositories;

public interface IResultsEcoPoolRepository
{
    Task<List<ResultsEcoPool>> GetAllResultsEcoPool();
    Task<List<ResultsEcoPool>> GetResultsEcoPoolToPayment();
}