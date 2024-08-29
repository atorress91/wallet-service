using Microsoft.EntityFrameworkCore;
using WalletService.Data.Database;
using WalletService.Data.Database.Models;
using WalletService.Data.Repositories.IRepositories;

namespace WalletService.Data.Repositories;

public class BrandRepository : BaseRepository, IBrandRepository
{
    public BrandRepository(WalletServiceDbContext context) : base(context)
    {
    }

    public Task<Brand?> GetBrandByIdAsync(string secretKey)
        => Context.Brand.FirstOrDefaultAsync(x => x.SecretKey == secretKey);
}