using Microsoft.Extensions.Options;
using WalletService.Data.Adapters.IAdapters;
using WalletService.Models.Configuration;
using WalletService.Models.Responses.BaseResponses;
using WalletService.Utility.Extensions;

namespace WalletService.Data.Adapters;

public class InventoryServiceAdapter : BaseAdapter, IInventoryServiceAdapter
{
    public InventoryServiceAdapter(
        IOptions<ApplicationConfiguration> appSettings) : base(appSettings) { }

    protected override string? GetServiceUrl()
        => AppSettings.Endpoints!.InventoryServiceURL;

    protected override string? GetTokenUrl()
        => AppSettings.EndpointTokens!.InventoryServiceToken;

    protected override string? GetWebToken(int brandId)
    {
        return brandId switch
        {
            1 => AppSettings.WebTokens!.EcosystemToken,
            2 => AppSettings.WebTokens!.RecyCoinToken,
            _ => null
        };
    }

    public Task<IRestResponse> GetProductsIds(int[] ids, int brandId)
    {
        var requestObject = new Dictionary<string, object>
        {
            { "productIds", ids }
        };


        return Post("/product/get_products_by_ids/", requestObject.ToJsonString(), brandId);
    }

    public Task<IRestResponse> GetProductById(int id, int brandId)
    {
        return Post($"/product/get_product_by_id/", id.ToJsonString(), brandId);
    }



}