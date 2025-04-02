using WalletService.Models.Configuration;
using Microsoft.Extensions.Options;
using RestSharp;
using baseResponse = WalletService.Models.Responses.BaseResponses;

namespace WalletService.Data.Adapters;

public abstract class BaseAdapter
{
    protected readonly ApplicationConfiguration AppSettings;

    protected BaseAdapter(IOptions<ApplicationConfiguration> appSettings)
    {
        AppSettings       = appSettings.Value;
    }
    
    protected abstract string? GetServiceUrl();

    protected abstract string? GetTokenUrl();
    
    protected abstract string? GetWebToken(long brandId);

    protected async Task<baseResponse.IRestResponse> Get(string path, Dictionary<string, string>? queryParams, long brandId)
    {
        var client  = new RestClient(GetServiceUrl()!);
        var request = new RestRequest(path);
        request.AddHeader("Accept", "application/json");
        request.AddHeader("Authorization", GetTokenUrl()!);
        request.AddHeader("X-Client-Id",GetWebToken(brandId)!);
        var response = await client.ExecuteAsync(request);

        return new baseResponse.RestResponse
        {
            Content    = response.Content,
            StatusCode = response.StatusCode
        };
    }

    protected async Task<baseResponse.IRestResponse> Post(string path, string data, long brandId)
    {
        var client  = new RestClient(GetServiceUrl()!);
        var request = new RestRequest(path, Method.Post);
        request.AddHeader("Accept", "application/json");
        request.AddHeader("Content-Type", "application/json");
        request.AddHeader("Authorization", GetTokenUrl()!);
        request.AddHeader("X-Client-Id",GetWebToken(brandId)!);
        request.AddParameter("application/json", data, ParameterType.RequestBody);
        var response = await client.ExecuteAsync(request);

        return new baseResponse.RestResponse
        {
            Content    = response.Content,
            StatusCode = response.StatusCode
        };
    }

    protected async Task<baseResponse.IRestResponse> Put(string path,long brandId)
    {
        var client  = new RestClient(GetServiceUrl()!);
        var request = new RestRequest(path, Method.Put);
        request.AddHeader("Accept", "application/json");
        request.AddHeader("Content-Type", "application/json");
        request.AddHeader("Authorization", GetTokenUrl()!);
        request.AddHeader("X-Client-Id",GetWebToken(brandId)!);
        var response = await client.ExecuteAsync(request);

        return new baseResponse.RestResponse
        {
            Content    = response.Content,
            StatusCode = response.StatusCode
        };
    }
}