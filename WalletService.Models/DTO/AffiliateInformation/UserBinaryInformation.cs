using System.Text.Json.Serialization;

namespace WalletService.Models.DTO.AffiliateInformation;

public class UserBinaryInformation
{
    [JsonPropertyName("userId")]
    public int UserId { get; set; }
    
    [JsonPropertyName("pointsLeft")]
    public decimal PointsLeft { get; set; }
    
    [JsonPropertyName("pointsRight")]
    public decimal PointsRight { get; set; }
}