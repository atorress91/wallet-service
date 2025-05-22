using WalletService.Data.Database.Models;

namespace WalletService.Data.Repositories.IRepositories;

public interface IMatrixQualificationRepository
{
    Task<MatrixQualification?> GetByUserAndMatrixTypeAsync(long userId, int matrixType);
    Task<MatrixQualification?> GetQualificationById(int qualificationId);
    Task<MatrixQualification?> CreateAsync(MatrixQualification qualification);
    Task<bool> UpdateAsync(MatrixQualification qualification);
    Task<IEnumerable<MatrixQualification>> GetAllByUserIdAsync(long userId);
    Task<IEnumerable<MatrixQualification>> GetAllInconsistentRecordsAsync();
}