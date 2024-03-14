using Newtonsoft.Json;

namespace WalletService.Models.Responses;

public class CoinPaymentWithdrawalResponse
{
    [JsonProperty("error")] public string? Error { get; set; }
    [JsonProperty("result")] public Dictionary<string, WithdrawalInfo>? Result { get; set; }
}

public class WithdrawalInfo
{
    [JsonProperty("error")] public string? Error { get; set; }
    [JsonProperty("id")] public string? Id { get; set; }
    [JsonProperty("status")] public int Status { get; set; }
    [JsonProperty("amount")] public string? Amount { get; set; }
}