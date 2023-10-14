using System.Text.Json.Serialization;

namespace WalletService.Models.Requests.CoinPayRequest;

public class PaymentRequest
{
    [JsonPropertyName("idCurrency")] public int IdCurrency { get; set; }
    [JsonPropertyName("amount")] public double Amount { get; set; }
    [JsonPropertyName("details")] public string? Details { get; set; }
}