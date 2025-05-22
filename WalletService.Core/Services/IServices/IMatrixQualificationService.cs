using WalletService.Models.DTO.MatrixQualificationDto;
using WalletService.Models.Requests.MatrixQualification;

namespace WalletService.Core.Services.IServices;

public interface IMatrixQualificationService
{
    Task<MatrixQualificationDto?> UpdateAsync(MatrixQualificationRequest request);
    Task<MatrixQualificationDto?> GetByUserAndMatrixTypeAsync(int userId, int matrixType);
}