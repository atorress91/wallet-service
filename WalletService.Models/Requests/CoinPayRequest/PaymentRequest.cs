using Newtonsoft.Json;

namespace WalletService.Models.Requests.CoinPayRequest;

public class PaymentRequest
{
    [JsonProperty("idCurrency")] public int IdCurrency { get; set; }
    [JsonProperty("amount")] public double Amount { get; set; }
    [JsonProperty("details")] public string? Details { get; set; }
}