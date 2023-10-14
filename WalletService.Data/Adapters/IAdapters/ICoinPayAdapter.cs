using WalletService.Models.Requests.CoinPayRequest;
using WalletService.Models.Responses.BaseResponses;

namespace WalletService.Data.Adapters.IAdapters;

public interface ICoinPayAdapter
{
    Task<IRestResponse> GetTransactionById(int          idTransaction);
    Task<IRestResponse> CreateTransaction(PaymentRequest request);
    Task<IRestResponse> CreateChannel(CreateChannelRequest request);
    Task<IRestResponse> SendFunds(SendFundRequest request);
    Task<IRestResponse> GetNetworksByIdCurrency(int idNetwork);
    Task<IRestResponse> CreateAddress(int idWallet, CreateAddresRequest request);
}