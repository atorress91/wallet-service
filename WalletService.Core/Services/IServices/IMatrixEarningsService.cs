using WalletService.Models.DTO.MatrixEarningDto;
using WalletService.Models.Requests.MatrixEarningRequest;

namespace WalletService.Core.Services.IServices;

public interface IMatrixEarningsService
{
    Task<MatrixEarningDto> CreateAsync(MatrixEarningRequest request);
}