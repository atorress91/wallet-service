using WalletService.Data.Database.Models;


namespace WalletService.Data.Repositories.IRepositories;

public interface IResultsEcoPoolRepository
{
    Task<List<ResultsModel2>> GetAllResultsEcoPool();
    Task<List<ResultsModel2>> GetResultsModel2ToPayment();
    Task<List<ResultsModel1A>> GetResultsModel1AToPayment();
    Task<List<ResultsModel1B>> GetResultsModel1BToPayment();
    Task<List<ResultsModel3>> GetResultsMode3ToPayment();
}