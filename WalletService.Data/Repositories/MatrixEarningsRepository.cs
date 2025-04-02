using Microsoft.EntityFrameworkCore;
using WalletService.Data.Database;
using WalletService.Data.Repositories.IRepositories;

namespace WalletService.Data.Repositories;

public class MatrixEarningsRepository(WalletServiceDbContext context) : BaseRepository(context), IMatrixEarningsRepository
{
    public async Task<decimal> GetTotalEarningsAsync(long userId, int matrixType)
        => await Context.MatrixEarnings.Where(x => x.UserId == userId && x.MatrixType == matrixType)
            .SumAsync(x => x.Amount);
}