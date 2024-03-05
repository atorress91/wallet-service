using System.Globalization;
using Microsoft.Extensions.Options;
using System.Text.Json;
using WalletService.Models.Configuration;

using WalletService.Data.Adapters.IAdapters;
using WalletService.Data.Database.CustomModels;
using WalletService.Models.Constants;
using WalletService.Models.Responses.BaseResponses;


namespace WalletService.Data.Adapters;

public class PagaditoAdapter : IPagaditoAdapter
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ApplicationConfiguration _appSettings;

    public PagaditoAdapter(IOptions<ApplicationConfiguration> appSettings, IHttpClientFactory httpClientFactory)
    {
        _appSettings = appSettings.Value;
        _httpClientFactory = httpClientFactory;
    }

    public async Task<PagaditoResponse?> ConnectAsync()
    {
        try
        {
            var url = _appSettings.Pagadito!.Url;
            var parameters = new Dictionary<string, string>
            {
                { "operation", Constants.PagaditoConnectKey },
                { "uid", _appSettings.Pagadito.Uid! },
                { "wsk", _appSettings.Pagadito.Wsk! },
                { "format_return", "json" }
            };

            using var client = _httpClientFactory.CreateClient();
            var content = new FormUrlEncodedContent(parameters);
            var response = await client.PostAsync(url, content);

            var responseString = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
            {
                
                var connectResponse = JsonSerializer.Deserialize<PagaditoResponse>(responseString);
                if (connectResponse != null && connectResponse.Code == "PG1001")
                {
                    return connectResponse;
                }

                throw new Exception("Pagadito response does not contain expected data.");
            }
            else
            {
                throw new Exception($"Error connecting to Pagadito: {response.StatusCode}, {responseString}");
            }
        }
        catch (Exception ex)
        {
            throw new Exception($"Error connecting to Pagadito: {ex.Message}", ex);
        }
    }

    public async Task<PagaditoResponse?> ExecuteTransaction(CreatePagaditoTransaction request)
    {
        var url = _appSettings.Pagadito!.Url;

        string jsonDetails = JsonSerializer.Serialize(request.Details);
        string jsonCustomParams = JsonSerializer.Serialize(request.CustomParams);

        var parameters = new Dictionary<string, string>
        {
            { "operation", Constants.PagaditoExecuteKey },
            { "token", request.Token },
            { "ern", request.Ern },
            { "amount", request.Amount.ToString(CultureInfo.InvariantCulture) },
            { "details", jsonDetails },
            { "custom_params", jsonCustomParams },
            { "currency", Constants.PagaditoCurrency },
            { "format_return", Constants.PagaditoFormatReturn }
        };

        using var client = new HttpClient();
        var content = new FormUrlEncodedContent(parameters);
        var response = await client.PostAsync(url, content);

        var responseContent = await response.Content.ReadAsStringAsync();
        var execTransResponse = JsonSerializer.Deserialize<PagaditoResponse>(responseContent);

        if (execTransResponse != null && execTransResponse.Code == "PG1002")
        {
            return execTransResponse;
        }
        else
        {
            throw new Exception("Error executing transaction on Pagadito: " + execTransResponse?.Message);
        }
    }
}