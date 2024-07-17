using Newtonsoft.Json;

namespace WalletService.Models.Requests.CoinPayRequest;

public class WithDrawalRequest
{
    [JsonProperty("id")] 
    public int Id { get; set; }
    
    [JsonProperty("affiliateId")] 
    public int AffiliateId { get; set; }
    
    [JsonProperty("amount")] 
    public int Amount { get; set; }
}