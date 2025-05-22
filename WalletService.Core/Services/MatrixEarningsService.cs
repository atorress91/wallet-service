using AutoMapper;
using WalletService.Core.Services.IServices;
using WalletService.Data.Database.Models;
using WalletService.Data.Repositories.IRepositories;
using WalletService.Models.DTO.MatrixEarningDto;
using WalletService.Models.Requests.MatrixEarningRequest;

namespace WalletService.Core.Services;

public class MatrixEarningsService : BaseService, IMatrixEarningsService
{
    private readonly IMatrixEarningsRepository _matrixEarningsRepository;

    public MatrixEarningsService(IMapper mapper, IMatrixEarningsRepository matrixEarningsRepository) : base(mapper)
    {
        _matrixEarningsRepository = matrixEarningsRepository;
    }

    public async Task<MatrixEarningDto> CreateAsync(MatrixEarningRequest request)
    {
        var matrixEarning = Mapper.Map<MatrixEarning>(request);

        var createdMatrixEarning = await _matrixEarningsRepository.CreateAsync(matrixEarning);
        return Mapper.Map<MatrixEarningDto>(createdMatrixEarning);
    }
}