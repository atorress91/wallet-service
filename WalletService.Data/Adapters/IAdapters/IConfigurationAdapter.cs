using WalletService.Models.Responses.BaseResponses;

namespace WalletService.Data.Adapters.IAdapters;

public interface IConfigurationAdapter
{
    Task<IRestResponse> GetGradings();
    Task<IRestResponse> GetConfigurationProduct();
    Task<IRestResponse> GetPointsConfiguration();

}