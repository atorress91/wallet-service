using WalletService.Data.Database.Models;

namespace WalletService.Data.Repositories.IRepositories;

public interface IBrandRepository
{
    Task<Brand?> GetBrandByIdAsync(string secretKey);
}