using System.Net.Http.Headers;
using System.Text;
using WalletService.Models.Responses.BaseResponses;
using WalletService.Utility.Extensions;

namespace WalletService.Data.Adapters;

public abstract class CoinPayBaseAdapter
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly string _baseUrl;

    protected CoinPayBaseAdapter(IHttpClientFactory httpClientFactory, string baseUrl)
    {
        _httpClientFactory = httpClientFactory;
        _baseUrl = baseUrl;
    }

    protected abstract Task<string?> Authenticate();

    protected async Task<IRestResponse> Get(string path, Dictionary<string, string> queryParams)
    {
        var token = await Authenticate();
        var client = _httpClientFactory.CreateClient();

        if (string.IsNullOrEmpty(_baseUrl))
        {
            throw new InvalidOperationException("Base URL is not defined.");
        }

        client.BaseAddress = new Uri(_baseUrl);
        client.DefaultRequestHeaders.Accept.Clear();
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        if (queryParams.Count > 0)
        {
            path = $"{path}?{string.Join("&", queryParams.Select(kvp => $"{kvp.Key}={kvp.Value}"))}";
        }

        var response = await client.GetAsync(path);
        var content = await response.Content.ReadAsStringAsync();

        return new RestResponse
        {
            Content = content,
            StatusCode = response.StatusCode
        };
    }

    protected async Task<IRestResponse> Post(string path, object data)
    {
        var token = await Authenticate();
        var client = _httpClientFactory.CreateClient();

        if (string.IsNullOrEmpty(_baseUrl))
        {
            throw new InvalidOperationException("Base URL is not defined.");
        }

        client.BaseAddress = new Uri(_baseUrl);
        client.DefaultRequestHeaders.Accept.Clear();
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        var serializeObject = data.ToJsonString();
        var jsonContent     = new StringContent(serializeObject, Encoding.UTF8, "application/json");

        var response = await client.PostAsync(path, jsonContent);
        var content = await response.Content.ReadAsStringAsync();

        return new RestResponse
        {
            Content = content,
            StatusCode = response.StatusCode
        };
    }
}