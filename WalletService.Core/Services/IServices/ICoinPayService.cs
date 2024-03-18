using WalletService.Models.Requests.CoinPayRequest;
using WalletService.Models.Responses;
using WalletService.Models.Responses.BaseResponses;

namespace WalletService.Core.Services.IServices;

public interface ICoinPayService
{
    #region coinPay
    Task<CreateTransactionResponse?> CreateTransaction(CreateTransactionRequest request);
    Task<CreateChannelResponse?>CreateChannel(CreateTransactionRequest request);
    Task<GetNetworkResponse?> GetNetworksByIdCurrency(int idCurrency);
    Task<CreateAddressResponse?> CreateAddress(CreateAddresRequest request);
    Task<bool> IsValidSignature(int idUser, int idTransaction, string dynamicKey, string incomingSignature);

    #endregion
}