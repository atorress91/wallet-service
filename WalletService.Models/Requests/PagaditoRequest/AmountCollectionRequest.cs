using System.Text.Json.Serialization;

namespace WalletService.Models.Requests.PagaditoRequest;

public class AmountCollectionRequest
{
    [JsonPropertyName("total")] 
    public string? Total { get; set; } 

    [JsonPropertyName("currency")]
    public string? Currency { get; set; } 
}