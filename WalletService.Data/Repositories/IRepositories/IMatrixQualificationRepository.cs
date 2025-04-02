using WalletService.Data.Database.Models;

namespace WalletService.Data.Repositories.IRepositories;

public interface IMatrixQualificationRepository
{
    Task<MatrixQualification?> GetByUserAndMatrixTypeAsync(long userId, int matrixType);
    Task<MatrixQualification?> CreateAsync(MatrixQualification qualification);
    Task<MatrixQualification?> UpdateAsync(MatrixQualification qualification);
}