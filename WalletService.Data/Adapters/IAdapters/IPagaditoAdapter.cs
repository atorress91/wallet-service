using WalletService.Data.Database.CustomModels;
using WalletService.Models.Responses.BaseResponses;

namespace WalletService.Data.Adapters.IAdapters;

public interface IPagaditoAdapter
{
    Task<PagaditoResponse?> ConnectAsync();
    Task<PagaditoResponse?> ExecuteTransaction(CreatePagaditoTransaction request);
}