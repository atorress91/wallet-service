using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;
using WalletService.Models.Configuration;
using WalletService.Models.Constants;
using WalletService.Models.Responses.BaseResponses;
using WalletService.Utility.Extensions;

namespace WalletService.Data.Adapters;

public abstract class CoinPayBaseAdapter
{
    private ApplicationConfiguration _appSettings;
    private readonly HttpClient _client;

    protected CoinPayBaseAdapter(IHttpClientFactory httpClientFactory, IOptions<ApplicationConfiguration> appSettings)
    {
        _client = httpClientFactory.CreateClient();
        var baseUrl = appSettings.Value.Endpoints!.CoinPayURL!;
        _appSettings = appSettings.Value;

        _client.BaseAddress = new Uri(baseUrl);
        _client.DefaultRequestHeaders.Accept.Clear();
        _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    }

    private async Task<string?> Authenticate()
    {
        var initialToken = _appSettings.CoinPay?.InitialToken ??
                           throw new InvalidOperationException("Initial token is not set.");
        var secretKey = _appSettings.CoinPay?.SecretKey ??
                        throw new InvalidOperationException("Secret key is not set.");

        var idRequest = DateTime.UtcNow.ToString("yyyyMMddHHmmss");

        using var sha256 = SHA256.Create();
        var checksumData = Encoding.UTF8.GetBytes(idRequest + secretKey);
        var checksumHash = sha256.ComputeHash(checksumData);
        var checksum = BitConverter.ToString(checksumHash).Replace("-", "").ToLower();

        var requestBody = new
        {
            IdRequest = idRequest,
            Token = initialToken,
            Checksum = checksum
        };

        var response = await _client.PostAsync("/api/auth/integration/createToken/v1",
            new StringContent(requestBody.ToJsonString(), Encoding.UTF8, "application/json"));

        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException($"Request to authenticate failed with status code: {response.StatusCode}");
        }

        var responseJson = await response.Content.ReadAsStringAsync();
        var responseData = responseJson.ToJsonObject<CoinPayResponse>();

        if (responseData?.StatusCode != Constants.SuccessStatusCode)
        {
            throw new InvalidOperationException($"Authentication failed with status code: {responseData?.StatusCode}");
        }

        return responseData.Data.Token;
    }

    protected async Task<IRestResponse> Get(string path, Dictionary<string, string> queryParams)
    {
        var token = await Authenticate();

        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        if (queryParams.Count > 0)
        {
            path = $"{path}?{string.Join("&", queryParams.Select(kvp => $"{kvp.Key}={kvp.Value}"))}";
        }

        var response = await _client.GetAsync(path);
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

        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var serializeObject = data.ToJsonString();
        var jsonContent = new StringContent(serializeObject, Encoding.UTF8, "application/json");

        var response = await _client.PostAsync(path, jsonContent);
        var content = await response.Content.ReadAsStringAsync();

        return new RestResponse
        {
            Content = content,
            StatusCode = response.StatusCode
        };
    }
}