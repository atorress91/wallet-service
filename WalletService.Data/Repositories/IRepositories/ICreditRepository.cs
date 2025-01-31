using WalletService.Data.Database.Models;

namespace WalletService.Data.Repositories.IRepositories;

public interface ICreditRepository
{
    Task<Credit> CreateCredit(Credit credit);
}