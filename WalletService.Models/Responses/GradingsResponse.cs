using System.Text.Json.Serialization;

using WalletService.Models.DTO.WalletDto;

namespace WalletService.Models.Responses;

public class GradingsResponse
{
    public class ProductsResponse
    {
        [JsonPropertyName("success")] public bool Success { get; set; }
        [JsonPropertyName("data")] public ICollection<WalletDto> Data { get; set; } = new List<WalletDto>();

        [JsonPropertyName("message")] public string Message { get; set; } = string.Empty;

        [JsonPropertyName("code")] public int Code { get; set; }
    }
}