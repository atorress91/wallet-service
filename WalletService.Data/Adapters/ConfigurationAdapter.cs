using Microsoft.Extensions.Options;
using WalletService.Data.Adapters.IAdapters;
using WalletService.Models.Configuration;
using WalletService.Models.Responses.BaseResponses;

namespace WalletService.Data.Adapters;

public class ConfigurationAdapter : BaseAdapter, IConfigurationAdapter
{
    public ConfigurationAdapter(
        IOptions<ApplicationConfiguration> appSettings,
        IHttpClientFactory                 httpClientFactory,
        HttpClient                         httpClient) : base(appSettings, httpClientFactory, httpClient) { }

    protected override string? GetServiceUrl()
        => AppSettings.Endpoints!.SystemConfigurationURL;

    protected override string? GetTokenUrl()
        => AppSettings.EndpointTokens!.SystemConfigurationServiceToken;
    
    protected override string? GetWebToken(int brandId)
    {
        return brandId switch
        {
            1 => AppSettings.WebTokens!.EcosystemToken,
            2 => AppSettings.WebTokens!.RecyCoinToken,
            _ => null
        };
    }

    public Task<IRestResponse> GetGradings(int brandId)
        => Get($"/grading", new Dictionary<string, string>(), brandId);

    public Task<IRestResponse> GetConfigurationProduct(int brandId)
        => Get($"/configuration/get_product_configuration", new Dictionary<string, string>(),brandId);

    public Task<IRestResponse> GetPointsConfiguration(int brandId)
        => Get($"/configuration/get_points_configuration", new Dictionary<string, string>(),brandId);
    
}