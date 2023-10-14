using Microsoft.AspNetCore.Http;
using WalletService.Data.Database.Models;
using WalletService.Models.Requests.ConPaymentRequest;
using WalletService.Models.Responses;

namespace WalletService.Core.Services.IServices;

public interface IConPaymentService
{
    Task<GetPayByNameProfileResponse> GetPayByNameProfile(string                pbntag);
    Task<GetDepositAddressResponse?> GetDepositAddress(string                   currency);
    Task<GetCoinBalancesResponse> GetCoinBalances(bool                          includeZeroBalances = false);
    Task<CreateConPaymentsTransactionResponse?> CreatePayment(ConPaymentRequest request);
    Task<GetTransactionInfoResponse> GetTransactionInfo(string                  txid, bool full = false);

    Task<string> ProcessIpnAsync(IpnRequest ipnRequest, IHeaderDictionary headers);
    Task<CoinPaymentWithdrawalResponse?> CreateMassWithdrawal(WalletsRequests[] requests);
}