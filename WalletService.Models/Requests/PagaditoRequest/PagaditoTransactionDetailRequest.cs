using System.Text.Json.Serialization;

namespace WalletService.Models.Requests.PagaditoRequest;

public class PagaditoTransactionDetailRequest
{
    [JsonPropertyName("quantity")] 
    public string? Quantity { get; set; }
    
    [JsonPropertyName("description")]
    public string? Description { get; set; } 
    
    [JsonPropertyName("price")] 
    public string? Price { get; set; }
    
    [JsonPropertyName("url_product")] 
    public string? UrlProduct { get; set; }
}