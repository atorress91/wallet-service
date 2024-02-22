using Microsoft.AspNetCore.Http;
using WalletService.Models.Requests.PagaditoRequest;

namespace WalletService.Core.Services.IServices;

public interface IPagaditoService
{
    Task<string?> CreateTransaction(CreatePagaditoTransactionRequest request);
    Task<bool> VerifySignature(IHeaderDictionary headers, string requestBody);
    Task<bool> ProcessPurchase(WebHookRequest? request);
}