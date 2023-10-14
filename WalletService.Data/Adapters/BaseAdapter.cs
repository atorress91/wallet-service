using WalletService.Models.Configuration;
using Microsoft.Extensions.Options;
using RestSharp;
using baseResponse = WalletService.Models.Responses.BaseResponses;

namespace WalletService.Data.Adapters;

public abstract class BaseAdapter
{
    protected readonly ApplicationConfiguration AppSettings;
    private readonly   IHttpClientFactory       HttpClientFactory;

    protected BaseAdapter(
        IOptions<ApplicationConfiguration> appSettings,
        IHttpClientFactory                 httpClientFactory, HttpClient httpClient)
    {
        AppSettings       = appSettings.Value;
        HttpClientFactory = httpClientFactory;
    }
    protected BaseAdapter()
    {
        throw new NotImplementedException();
    }

    protected abstract string? GetServiceUrl();

    protected abstract string? GetTokenUrl();

    protected async Task<baseResponse.IRestResponse> Get(string path, Dictionary<string, string>? queryParams)
    {
        var client  = new RestClient(GetServiceUrl()!);
        var request = new RestRequest(path, Method.Get);
        request.AddHeader("Accept", "application/json");
        request.AddHeader("Authorization", GetTokenUrl()!);
        var response = await client.ExecuteAsync(request);

        return new baseResponse.RestResponse
        {
            Content    = response.Content,
            StatusCode = response.StatusCode
        };
    }

    protected async Task<baseResponse.IRestResponse> Post(string path, string data)
    {
        var client  = new RestClient(GetServiceUrl()!);
        var request = new RestRequest(path, Method.Post);
        request.AddHeader("Accept", "application/json");
        request.AddHeader("Content-Type", "application/json");
        request.AddHeader("Authorization", GetTokenUrl()!);
        request.AddParameter("application/json", data, ParameterType.RequestBody);
        var response = await client.ExecuteAsync(request);

        return new baseResponse.RestResponse
        {
            Content    = response.Content,
            StatusCode = response.StatusCode
        };
    }

    protected async Task<baseResponse.IRestResponse> Put(string path)
    {
        var client  = new RestClient(GetServiceUrl()!);
        var request = new RestRequest(path, Method.Put);
        request.AddHeader("Accept", "application/json");
        request.AddHeader("Content-Type", "application/json");
        request.AddHeader("Authorization", GetTokenUrl()!);
        var response = await client.ExecuteAsync(request);

        return new baseResponse.RestResponse
        {
            Content    = response.Content,
            StatusCode = response.StatusCode
        };
    }


}