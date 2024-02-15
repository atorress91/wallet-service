using WalletService.Core.Services.IServices;
using WalletService.Data.Adapters.IAdapters;
using WalletService.Models.Requests.PagaditoRequest;

namespace WalletService.Core.Services;

public class PagaditoService : IPagaditoService
{
    private readonly IPagaditoAdapter _pagaditoAdapter;

    public PagaditoService(IPagaditoAdapter pagaditoAdapter)
    {
        _pagaditoAdapter = pagaditoAdapter;
    }

    public async Task<string?> CreateTransaction(TransactionRequest request)
    {
        var connectResponse = await _pagaditoAdapter.ConnectAsync();
        
        if (connectResponse == null || string.IsNullOrEmpty(connectResponse.Value))
            return "This connect is not valid";

        request.Token = connectResponse.Value;

        var executeTransaction = await _pagaditoAdapter.ExecuteTransaction(request);
        
        if (executeTransaction == null || string.IsNullOrEmpty(executeTransaction.Value))
            return "This transaction is not valid";

        return executeTransaction.Value;
    }
}