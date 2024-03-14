using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using WalletService.Data.Adapters.IAdapters;
using WalletService.Models.Configuration;
using WalletService.Models.Requests.CoinPayRequest;
using WalletService.Models.Responses.BaseResponses;
using WalletService.Utility.Extensions;


namespace WalletService.Data.Adapters;

public class CoinPayAdapter : CoinPayBaseAdapter, ICoinPayAdapter
{
    private ApplicationConfiguration _appSettings;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly string _baseUrl;

    public CoinPayAdapter(IOptions<ApplicationConfiguration> appSettings, IHttpClientFactory httpClientFactory)
        : base(httpClientFactory, appSettings.Value.Endpoints?.CoinPayURL!)
    {
        _appSettings = appSettings.Value;
        _httpClientFactory = httpClientFactory;
        _baseUrl = _appSettings.Endpoints?.CoinPayURL!;
    }

    protected override async Task<string?> Authenticate()
    {
        var initialToken = _appSettings.CoinPay?.InitialToken;
        var secretKey = _appSettings.CoinPay?.SecretKey;

        var idRequest = DateTime.UtcNow.ToString("yyyyMMddHHmmss");

        using var sha256 = SHA256.Create();
        var checksumData = Encoding.UTF8.GetBytes(idRequest + secretKey);
        var checksumHash = sha256.ComputeHash(checksumData);
        var checksum = BitConverter.ToString(checksumHash).Replace("-", "").ToLower();

        var client = _httpClientFactory.CreateClient();
        client.BaseAddress = new Uri(_baseUrl);

        var requestBody = new
        {
            IdRequest = idRequest,
            Token = initialToken,
            Checksum = checksum
        };

        var response = await client.PostAsync("/api/auth/integration/createToken/v1",
            new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json"));

        if (response.IsSuccessStatusCode)
        {
            var jsonString   = await response.Content.ReadAsStringAsync();
            var responseData = jsonString.ToJsonObject<CoinPayResponse>();
            
            return responseData?.Data.Token;
        }

        return null;
    }

    public async Task<IRestResponse> GetTransactionById(int idTransaction)
    {
        return await Get($"/api/integration/transaction/getTransaction/{idTransaction}/v1",
            new Dictionary<string, string>());
    }

    public async Task<IRestResponse> CreateTransaction(PaymentRequest request)
        => await Post($"/api/integration/cpay/requestPayment/v1", request);

    public async Task<IRestResponse> SendFunds(SendFundRequest request)
        => await Post($"/api/integration/wallet/sendFunds/v1",request);
    
    public async Task<IRestResponse> CreateChannel(CreateChannelRequest request)
        => await Post("/api/integration/wallet/createChannel/v1",request);

    public async Task<IRestResponse> GetNetworksByIdCurrency(int idCurrency)
        => await Get($"api/integration/wallet/getNetwords/{idCurrency}/v1",new Dictionary<string,string>());

    public async Task<IRestResponse> CreateAddress(int idWallet,CreateAddresRequest request)
        => await Post($"api/integration/wallet/createAddress/{idWallet}/v1",request);


}