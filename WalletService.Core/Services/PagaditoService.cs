using AutoMapper;
using WalletService.Core.Services.IServices;
using WalletService.Data.Adapters.IAdapters;
using WalletService.Data.Database.CustomModels;
using WalletService.Models.Requests.PagaditoRequest;

namespace WalletService.Core.Services;

public class PagaditoService : BaseService, IPagaditoService
{
    private readonly IPagaditoAdapter _pagaditoAdapter;

    public PagaditoService(IMapper mapper, IPagaditoAdapter pagaditoAdapter) : base(mapper)
    {
        _pagaditoAdapter = pagaditoAdapter;
    }

    public async Task<string?> CreateTransaction(CreatePagaditoTransactionRequest request)
    {
        var connectResponse = await _pagaditoAdapter.ConnectAsync();

        if (connectResponse == null || string.IsNullOrEmpty(connectResponse.Value))
            return "This connect is not valid";

        var pagaditoTransaction = Mapper.Map<CreatePagaditoTransaction>(request);
        pagaditoTransaction.Token = connectResponse.Value;
        pagaditoTransaction.Ern   = Guid.NewGuid().ToString();

        var executeTransaction = await _pagaditoAdapter.ExecuteTransaction(pagaditoTransaction);

        if (executeTransaction == null || string.IsNullOrEmpty(executeTransaction.Value))
            return "This transaction is not valid";

        return executeTransaction.Value;
    }
}