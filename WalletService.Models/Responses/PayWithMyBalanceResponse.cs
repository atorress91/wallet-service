using Newtonsoft.Json;

namespace WalletService.Models.Responses;

public class PayWithMyBalanceResponse
{
    [JsonProperty("success")]
    public bool Success { get; set; }

    [JsonProperty("data")]
    public List<object>? Data { get; set; }

    [JsonProperty("message")]
    public string Message { get; set; } = string.Empty;

    [JsonProperty("code")]
    public int Code { get; set; }
}

