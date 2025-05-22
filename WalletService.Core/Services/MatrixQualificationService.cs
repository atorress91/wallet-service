using AutoMapper;
using WalletService.Core.Services.IServices;
using WalletService.Data.Repositories.IRepositories;
using WalletService.Models.DTO.MatrixQualificationDto;
using WalletService.Models.Requests.MatrixQualification;

namespace WalletService.Core.Services;

public class MatrixQualificationService : BaseService, IMatrixQualificationService
{
    private readonly IMatrixQualificationRepository _matrixQualificationRepository;

    public MatrixQualificationService(IMapper mapper, IMatrixQualificationRepository matrixQualificationRepository) :
        base(mapper)
    {
        _matrixQualificationRepository = matrixQualificationRepository;
    }

    public async Task<MatrixQualificationDto?> UpdateAsync(MatrixQualificationRequest request)
    {
        var qualification = await _matrixQualificationRepository.GetQualificationById(request.QualificationId);

        if (qualification is null)
            throw new ApplicationException($"Qualification with ID {request.QualificationId} not found.");

        var today = DateTime.UtcNow;
        qualification.UserId = request.UserId;
        qualification.MatrixType = request.MatrixType;
        qualification.TotalEarnings = request.TotalEarnings;
        qualification.WithdrawnAmount = request.WithdrawnAmount;
        qualification.AvailableBalance = request.AvailableBalance;
        qualification.IsQualified = request.IsQualified;
        qualification.UpdatedAt = today;

        var updatedQualification = await _matrixQualificationRepository.UpdateAsync(qualification);

        return Mapper.Map<MatrixQualificationDto>(updatedQualification);
    }

    public async Task<MatrixQualificationDto?> GetByUserAndMatrixTypeAsync(int userId, int matrixType)
    {
        var qualification = await _matrixQualificationRepository.GetByUserAndMatrixTypeAsync(userId, matrixType);

        if (qualification is null)
            throw new ApplicationException($"Qualification for user {userId} and matrix type {matrixType} not found.");

        return Mapper.Map<MatrixQualificationDto>(qualification);
    }
}