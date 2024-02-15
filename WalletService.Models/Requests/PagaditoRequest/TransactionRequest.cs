using System.Text.Json.Serialization;

namespace WalletService.Models.Requests.PagaditoRequest;

public class TransactionRequest
{
    [JsonPropertyName("token")] public string Token { get; set; } = string.Empty;
    [JsonPropertyName("ern")] public string Ern { get; set; } = string.Empty;
    [JsonPropertyName("amount")] public decimal Amount { get; set; }
    [JsonPropertyName("details")] public List<TransactionDetail> Details { get; set; }
    [JsonPropertyName("format_return")] public string FormatReturn { get; set; } = "json";
    [JsonPropertyName("currency")] public string Currency { get; set; } = "USD";

    [JsonPropertyName("custom_params")] public Dictionary<string, string> CustomParams { get; set; }
}