using System.Text.Json.Serialization;

namespace WalletService.Models.DTO.AffiliateInformation;

public class UserBinaryInformation
{
    [JsonPropertyName("affiliateId")]
    public int UserId { get; set; }

    [JsonPropertyName("leftVolume")] public decimal PointsLeft { get; set; }

    [JsonPropertyName("rightVolume")] public decimal PointsRight { get; set; } 
}

public class UserBinaryResponse
{
    [JsonPropertyName("success")]
    public bool Success { get; set; }
    
    [JsonPropertyName("data")]
    public List<UserBinaryInformation> Data { get; set; }
    
    [JsonPropertyName("message")]
    public string Message { get; set; }
    
    [JsonPropertyName("code")]
    public int Code { get; set; }
}
