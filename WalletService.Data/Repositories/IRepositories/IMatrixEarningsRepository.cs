using Microsoft.EntityFrameworkCore.Storage;
using WalletService.Data.Database.Models;

namespace WalletService.Data.Repositories.IRepositories;

public interface IMatrixEarningsRepository
{
    Task<decimal> GetTotalEarningsAsync(long userId, int matrixType);
    Task<MatrixEarning?> CreateAsync(MatrixEarning matrixEarning,int qualificationCount = 0);
    Task<IDbContextTransaction> BeginTransactionAsync();
}