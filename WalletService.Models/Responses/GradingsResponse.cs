using Newtonsoft.Json;
using WalletService.Models.DTO.WalletDto;

namespace WalletService.Models.Responses;

public class GradingsResponse
{
    public class ProductsResponse
    {
        [JsonProperty("success")] public bool Success { get; set; }
        [JsonProperty("data")] public ICollection<WalletDto> Data { get; set; } = new List<WalletDto>();

        [JsonProperty("message")] public string Message { get; set; } = string.Empty;

        [JsonProperty("code")] public int Code { get; set; }
    }
}