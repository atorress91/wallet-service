using System.Text.Json.Serialization;

namespace WalletService.Models.Requests.PagaditoRequest;

public class CreatePagaditoTransactionRequest
{
    [JsonPropertyName("amount")] 
    public decimal Amount { get; set; }
    
    [JsonPropertyName("details")] 
    public List<PagaditoTransactionDetailRequest> Details { get; set; } = new List<PagaditoTransactionDetailRequest>();

    [JsonPropertyName("custom_params")]
    public Dictionary<string, string> CustomParams { get; set; } = new Dictionary<string, string>();
}