using Microsoft.Extensions.Options;
using WalletService.Data.Adapters.IAdapters;
using WalletService.Models.Configuration;
using WalletService.Models.Requests.CoinPayRequest;
using WalletService.Models.Responses.BaseResponses;

namespace WalletService.Data.Adapters;

public class CoinPayAdapter : CoinPayBaseAdapter, ICoinPayAdapter
{
    public CoinPayAdapter(IOptions<ApplicationConfiguration> appSettings, IHttpClientFactory httpClientFactory)
        : base(httpClientFactory, appSettings)
    {
    }

    public async Task<IRestResponse> GetTransactionById(int idTransaction)
    {
        return await Get($"/api/integration/transaction/getTransaction/{idTransaction}/v1",
            new Dictionary<string, string>());
    }

    public async Task<IRestResponse> CreateTransaction(PaymentRequest request)
        => await Post($"/api/integration/cpay/requestPayment/v1", request);

    public async Task<IRestResponse> SendFunds(SendFundRequest request)
        => await Post($"/api/integration/wallet/sendFunds/v1", request);

    public async Task<IRestResponse> CreateChannel(CreateChannelRequest request)
        => await Post("/api/integration/wallet/createChannel/v1", request);

    public async Task<IRestResponse> GetNetworksByIdCurrency(int idCurrency)
        => await Get($"api/integration/wallet/getNetwords/{idCurrency}/v1", new Dictionary<string, string>());

    public async Task<IRestResponse> CreateAddress(int idWallet, CreateAddresRequest request)
        => await Post($"api/integration/wallet/createAddress/{idWallet}/v1", request);
}