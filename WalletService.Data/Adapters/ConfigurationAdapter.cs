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

    public Task<IRestResponse> GetGradings()
        => Get($"/grading", new Dictionary<string, string>());

    public Task<IRestResponse> GetConfigurationProduct()
        => Get($"/configuration/get_product_configuration", new Dictionary<string, string>());

    public Task<IRestResponse> GetPointsConfiguration()
        => Get($"/configuration/get_points_configuration", new Dictionary<string, string>());
    
}