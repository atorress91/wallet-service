using Newtonsoft.Json;

namespace WalletService.Models.DTO.AffiliateBtc;

public class AffiliateBtcDto
{
    [JsonProperty("id")] public int Id { get; set; }
    [JsonProperty("affiliateId")] public int AffiliateId { get; set; }
    [JsonProperty("address")] public string Address { get; set; } = String.Empty;
    [JsonProperty("status")] public byte Status { get; set; }
    [JsonProperty("createdAt")] public DateTime CreatedAt { get; set; }
    [JsonProperty("updatedAt")] public DateTime? UpdatedAt { get; set; }
    [JsonProperty("deletedAt")] public DateTime? DeletedAt { get; set; }
}