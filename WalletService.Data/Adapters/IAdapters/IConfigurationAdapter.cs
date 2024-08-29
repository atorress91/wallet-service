using WalletService.Models.Responses.BaseResponses;

namespace WalletService.Data.Adapters.IAdapters;

public interface IConfigurationAdapter
{
    Task<IRestResponse> GetGradings(int brandId);
    Task<IRestResponse> GetConfigurationProduct(int brandId);
    Task<IRestResponse> GetPointsConfiguration(int brandId);

}