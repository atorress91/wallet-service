using Newtonsoft.Json;

namespace WalletService.Models.Requests.ConPaymentRequest;

public class ConPaymentRequest
{
    [JsonProperty("amount")]
    public decimal Amount { get; set; }

    [JsonProperty("currency1")]
    public string Currency1 { get; set; } = string.Empty;

    [JsonProperty("currency2")]
    public string Currency2 { get; set; } = string.Empty;

    [JsonProperty("buyer_email")]
    public string BuyerEmail { get; set; } = string.Empty;

    [JsonProperty("address")]
    public string Address { get; set; } = string.Empty;

    [JsonProperty("buyer_name")]
    public string? BuyerName { get; set; }

    [JsonProperty("item_name")]
    public string? ItemName { get; set; }

    [JsonProperty("item_number")]
    public string? ItemNumber { get; set; }

    [JsonProperty("invoice")]
    public string? Invoice { get; set; }

    [JsonProperty("custom")]
    public string? Custom { get; set; }

    [JsonProperty("ipn_url")]
    public string? IpnUrl { get; set; }

    [JsonProperty("success_url")]
    public string? SuccessUrl { get; set; }

    [JsonProperty("cancel_url")]
    public string? CancelUrl { get; set; }

    [JsonProperty("products")]
    public List<ProductRequest> Products { get; set; }
}