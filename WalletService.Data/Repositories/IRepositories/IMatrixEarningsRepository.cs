namespace WalletService.Data.Repositories.IRepositories;

public interface IMatrixEarningsRepository
{
    Task<decimal> GetTotalEarningsAsync(long userId, int matrixType);
}