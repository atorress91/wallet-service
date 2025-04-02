namespace WalletService.Core.Services.IServices;

public interface IMatrixService
{
    Task<bool> CheckQualificationAsync(long brandId, int matrixType);
}