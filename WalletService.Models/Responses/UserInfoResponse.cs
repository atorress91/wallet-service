using System.Text.Json.Serialization;

namespace WalletService.Models.Responses;

public class UserInfoResponse
{
    [JsonPropertyName("name")] public string? Name { get; set; }
    [JsonPropertyName("id")] public int Id { get; set; }
    [JsonPropertyName("status")] public byte? Status { get; set; }
    
    [JsonPropertyName("activation_date")]public DateTime? ActivationDate { get; set; }
    [JsonPropertyName("user_name")] public string? UserName { get; set; }
    [JsonPropertyName("last_name")] public string? LastName { get; set; }
    [JsonPropertyName("city")] public string? City { get; set; }
    [JsonPropertyName("phone")] public string? Phone { get; set; }
    [JsonPropertyName("email")] public string? Email { get; set; }
    [JsonPropertyName("address")] public string? Address { get; set; }
    [JsonPropertyName("affiliate_type")] public string? AffiliateType { get; set; }

    [JsonPropertyName("card_id_authorization")] public bool? CardIdAuthorization { get; set; }
    [JsonPropertyName("country_information")] public CountryInformation? Country { get; set; }
    [JsonPropertyName("father")] public int Father { get; set; }
    [JsonPropertyName("verification_code")] public string? VerificationCode { get; set; }
}

public class CountryInformation
{
    [JsonPropertyName("name")] public string? Name { get; set; }
}