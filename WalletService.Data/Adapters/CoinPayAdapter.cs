using Microsoft.Extensions.Options;
using WalletService.Data.Adapters.IAdapters;

using WalletService.Models.Constants;
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
    public async Task<bool> VerifyTransactionSignature(SignatureParamsRequest request)
    {
        return await IsValidSignature(request);
    }
    
    public async Task<IRestResponse> GetTransactionById(int idTransaction)
        => await Get(string.Format(CoinPayRoutes.GetTransactionRoute, idTransaction), new Dictionary<string, string>());
    
    public async Task<IRestResponse> CreateTransaction(PaymentRequest request)
        => await Post(CoinPayRoutes.CreateTransactionRoute, request);

    public async Task<IRestResponse> SendFunds(SendFundRequest request)
        => await Post(CoinPayRoutes.SendFundsRoute, request);

    public async Task<IRestResponse> CreateChannel(CreateChannelRequest request)
        => await Post(CoinPayRoutes.CreateChannelRoute, request);

    public async Task<IRestResponse> GetNetworksByIdCurrency(int idCurrency)
        => await Get(string.Format(CoinPayRoutes.GetNetworksByIdCurrencyRoute, idCurrency),
            new Dictionary<string, string>());

    public async Task<IRestResponse> CreateAddress(int idWallet, CreateAddresRequest request)
        => await Post(string.Format(CoinPayRoutes.CreateAddressRoute, idWallet), request);
}