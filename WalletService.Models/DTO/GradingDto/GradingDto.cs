using Newtonsoft.Json;

namespace WalletService.Models.DTO.GradingDto;

public class GradingDto
{
    [JsonProperty("id")] public int Id { get; set; }

    [JsonProperty("name")] public string Name { get; set; } = null!;

    [JsonProperty("description")] public string? Description { get; set; }

    [JsonProperty("scope_level")] public int ScopeLevel { get; set; }

    [JsonProperty("is_infinity")] public bool IsInfinity { get; set; }

    [JsonProperty("personal_purchases")]
    public decimal PersonalPurchases { get; set; }

    [JsonProperty("personal_purchases_exact")]
    public bool PersonalPurchasesExact { get; set; }

    [JsonProperty("purchases_network")]
    public decimal PurchasesNetwork { get; set; }

    [JsonProperty("binary_volume")] public decimal BinaryVolume { get; set; }

    [JsonProperty("volume_points")] public int VolumePoints { get; set; }

    [JsonProperty("volume_points_network")]
    public int VolumePointsNetwork { get; set; }

    [JsonProperty("children_left_leg")]
    public int ChildrenLeftLeg { get; set; }

    [JsonProperty("children_right_leg")]
    public int ChildrenRightLeg { get; set; }

    [JsonProperty("front_by_matrix")] public int FrontByMatrix { get; set; }

    [JsonProperty("front_qualif_1")] public int FrontQualif1 { get; set; }

    [JsonProperty("front_score_1")] public int FrontScore1 { get; set; }

    [JsonProperty("front_qualif_2")] public int FrontQualif2 { get; set; }

    [JsonProperty("front_score_2")] public int FrontScore2 { get; set; }

    [JsonProperty("front_qualif_3")] public int FrontQualif3 { get; set; }

    [JsonProperty("front_score_3")] public int FrontScore3 { get; set; }

    [JsonProperty("exact_front_ratings")]
    public bool ExactFrontRatings { get; set; }

    [JsonProperty("leader_by_matrix")] public int LeaderByMatrix { get; set; }

    [JsonProperty("network_leaders")] public int? NetworkLeaders { get; set; }

    [JsonProperty("network_leaders_qualifier")]
    public int? NetworkLeadersQualifier { get; set; }

    [JsonProperty("products")] public int? Products { get; set; }

    [JsonProperty("affiliations")] public int? Affiliations { get; set; }

    [JsonProperty("have_both")] public bool HaveBoth { get; set; }

    [JsonProperty("activate_user_by")] public int ActivateUserBy { get; set; }

    [JsonProperty("active")] public int Active { get; set; }

    [JsonProperty("status")] public bool Status { get; set; }

    [JsonProperty("network_scope_level")]
    public int NetworkScopeLevel { get; set; }

    [JsonProperty("full_period")] public bool FullPeriod { get; set; }
}