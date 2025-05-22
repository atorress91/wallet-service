using WalletService.Models.Responses.BaseResponses;

namespace WalletService.Data.Adapters.IAdapters;

public interface IConfigurationAdapter
{
    Task<IRestResponse> GetGradings(long brandId);
    Task<IRestResponse> GetConfigurationProduct(long brandId);
    Task<IRestResponse> GetPointsConfiguration(long brandId);
    Task<IRestResponse> GetMatrixConfiguration(long brandId, int matrixType);
    Task<IRestResponse> GetAllMatrixConfigurations(long brandId);
}