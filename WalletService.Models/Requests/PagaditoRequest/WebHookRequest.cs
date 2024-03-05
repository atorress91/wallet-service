using System.Text.Json.Serialization;

namespace WalletService.Models.Requests.PagaditoRequest;

public class WebHookRequest
{
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    [JsonPropertyName("event_create_timestamp")]
    public string? EventCreateTimestamp { get; set; }

    [JsonPropertyName("event_type")] 
    public string? EventType { get; set; } 

    [JsonPropertyName("resource")]
    public ResourceCollectionRequest? Resource { get; set; }
}