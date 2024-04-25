using Microsoft.AspNetCore.Http;
using WalletService.Models.Requests.ConPaymentRequest;
using WalletService.Models.Requests.PagaditoRequest;
using WalletService.Models.Requests.WalletRequest;

namespace WalletService.Core.Services.IServices;

public interface IPagaditoService
{
    Task<string?> CreateTransaction(CreatePagaditoTransactionRequest request);
    Task<bool> VerifySignature(IHeaderDictionary headers, string requestBody);
    Task<bool> UpdateTransactionStatus(WebHookRequest? request);
}