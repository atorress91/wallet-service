using WalletService.Models.Responses.BaseResponses;

namespace WalletService.Data.Adapters.IAdapters;

public interface IInventoryServiceAdapter
{
    Task<IRestResponse> GetProductsIds(int[] ids);
    Task<IRestResponse> GetProductById(int   id);
}