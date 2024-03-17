using Newtonsoft.Json;

namespace WalletService.Models.Responses;

public class GetCoinBalancesResponse
{
    public string? Error { get; set; }
    public Dictionary<string, CoinInfo> Result { get; set; }

}

public class CoinInfo
{
    [JsonProperty("balance")]
    public long Balance { get; set; }

    [JsonProperty("balancef")]
    public string BalanceF { get; set; } = string.Empty;

    [JsonProperty("status")]
    public string Status { get; set; } = string.Empty;

    [JsonProperty("coin_status")]
    public string CoinStatus { get; set; } = string.Empty;
}