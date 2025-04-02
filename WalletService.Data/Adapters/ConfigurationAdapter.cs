using Microsoft.Extensions.Options;
using WalletService.Data.Adapters.IAdapters;
using WalletService.Models.Configuration;
using WalletService.Models.Responses.BaseResponses;

namespace WalletService.Data.Adapters;

public class ConfigurationAdapter : BaseAdapter, IConfigurationAdapter
{
    public ConfigurationAdapter(
        IOptions<ApplicationConfiguration> appSettings) : base(appSettings) { }

    protected override string? GetServiceUrl()
        => AppSettings.Endpoints!.SystemConfigurationURL;

    protected override string? GetTokenUrl()
        => AppSettings.EndpointTokens!.SystemConfigurationServiceToken;
    
    protected override string? GetWebToken(long brandId)
    {
        return brandId switch
        {
            1 => AppSettings.WebTokens!.EcosystemToken,
            2 => AppSettings.WebTokens!.RecyCoinToken,
            3 => AppSettings.WebTokens!.HouseCoinToken,
            4 => AppSettings.WebTokens!.ExitoJuntosToken,
            _ => null
        };
    }

    public Task<IRestResponse> GetGradings(long brandId)
        => Get($"/grading", new Dictionary<string, string>(), brandId);

    public Task<IRestResponse> GetConfigurationProduct(long brandId)
        => Get($"/configuration/get_product_configuration", new Dictionary<string, string>(),brandId);

    public Task<IRestResponse> GetPointsConfiguration(long brandId)
        => Get($"/configuration/get_points_configuration", new Dictionary<string, string>(),brandId);
    
    public Task<IRestResponse> GetMatrixConfiguration(long brandId, int matrixType)
        => Get($"/configuration/get_matrix_configuration/{matrixType}", new Dictionary<string, string>(),brandId);
}