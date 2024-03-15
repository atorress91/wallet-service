using System.Text.Json.Serialization;
using WalletService.Models.DTO.GradingDto;

namespace WalletService.Models.Responses;

public class GradingResponse
{
    [JsonPropertyName("success")] public bool Success { get; set; }
    [JsonPropertyName("data")] public List<GradingDto> Data { get; set; }

    [JsonPropertyName("message")] public string Message { get; set; } = string.Empty;

    [JsonPropertyName("code")] public int Code { get; set; }
}