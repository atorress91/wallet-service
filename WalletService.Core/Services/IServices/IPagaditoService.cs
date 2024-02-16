using WalletService.Models.Requests.PagaditoRequest;

namespace WalletService.Core.Services.IServices;

public interface IPagaditoService
{
    Task<string?> CreateTransaction(CreatePagaditoTransactionRequest request);
}