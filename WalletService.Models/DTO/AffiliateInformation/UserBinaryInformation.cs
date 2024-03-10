using Newtonsoft.Json;

namespace WalletService.Models.DTO.AffiliateInformation;

public class UserBinaryInformation
{
    [JsonProperty("affiliateId")]
    public int UserId { get; set; }

    [JsonProperty("leftVolume")] public decimal PointsLeft { get; set; }

    [JsonProperty("rightVolume")] public decimal PointsRight { get; set; } 
}

public class UserBinaryResponse
{
    [JsonProperty("success")]
    public bool Success { get; set; }
    
    [JsonProperty("data")]
    public List<UserBinaryInformation> Data { get; set; }
    
    [JsonProperty("message")]
    public string Message { get; set; }
    
    [JsonProperty("code")]
    public int Code { get; set; }
}
