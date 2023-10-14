using Microsoft.Extensions.Options;
using WalletService.Data.Adapters.IAdapters;
using WalletService.Models.Configuration;
using WalletService.Models.Responses.BaseResponses;
using WalletService.Utility.Extensions;

namespace WalletService.Data.Adapters;

public class InventoryServiceAdapter : BaseAdapter, IInventoryServiceAdapter
{
    public InventoryServiceAdapter(
        IOptions<ApplicationConfiguration> appSettings,
        IHttpClientFactory                 httpClientFactory,
        HttpClient                         httpClient) : base(appSettings, httpClientFactory, httpClient) { }

    protected override string? GetServiceUrl()
        => AppSettings.Endpoints!.InventoryServiceURL;

    protected override string? GetTokenUrl()
        => AppSettings.EndpointTokens!.InventoryServiceToken;

    public Task<IRestResponse> GetProductsIds(int[] ids)
    {
        var requestObject = new Dictionary<string, object>
        {
            { "productIds", ids }
        };


        return Post("/product/get_products_by_ids/", requestObject.ToJsonString());
    }

    public Task<IRestResponse> GetProductById(int id)
    {
        return Post($"/product/get_product_by_id/", id.ToJsonString());
    }



}