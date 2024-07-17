using WalletService.Models.DTO.CoinPayDto;
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
    Task<GetTransactionByIdResponse?> GetTransactionById(int idTransaction);
    Task<bool> ReceiveCoinPayNotifications(WebhookNotificationRequest? request);
    Task<SendFundsDto?> SendFunds(WithDrawalRequest[] request);

    #endregion
}