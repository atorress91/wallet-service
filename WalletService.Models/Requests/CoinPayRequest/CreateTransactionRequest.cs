using Newtonsoft.Json;
using WalletService.Models.Requests.ConPaymentRequest;

namespace WalletService.Models.Requests.CoinPayRequest;

public class CreateTransactionRequest
{
    [JsonProperty("affiliateId")] public int AffiliateId { get; set; }
    [JsonProperty("amount")] public int Amount { get; set; }
    [JsonProperty("products")] public List<ProductRequest>? Products { get; set; }
}