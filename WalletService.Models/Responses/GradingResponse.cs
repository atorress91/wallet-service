using Newtonsoft.Json;
using WalletService.Models.DTO.GradingDto;

namespace WalletService.Models.Responses;

public class GradingResponse
{
    [JsonProperty("success")] public bool Success { get; set; }
    [JsonProperty("data")] public List<GradingDto> Data { get; set; }

    [JsonProperty("message")] public string Message { get; set; } = string.Empty;

    [JsonProperty("code")] public int Code { get; set; }
}