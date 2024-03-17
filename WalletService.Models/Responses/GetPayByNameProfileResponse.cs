using Newtonsoft.Json;

namespace WalletService.Models.Responses;

public class GetPayByNameProfileResponse
{

    public string Error { get; set; } = string.Empty;
    public ProfileResult? Result { get; set; }
}

public class ProfileResult
{
    [JsonProperty("pbntag")]
    public string? Pbntag { get; set; }

    [JsonProperty("merchant")]
    public string? Merchant { get; set; } 

    [JsonProperty("profile_name")]
    public string? ProfileName { get; set; } 

    [JsonProperty("profile_url")]
    public string? ProfileUrl { get; set; } 

    [JsonProperty("profile_email")]
    public string? ProfileEmail { get; set; }

    [JsonProperty("profile_image")]
    public string? ProfileImage { get; set; } 

    [JsonProperty("member_since")]
    public long MemberSince { get; set; }

    [JsonProperty("feedback")]
    public Feedback? Feedback { get; set; }

}

public class Feedback
{
    [JsonProperty("pos")]
    public int Pos { get; set; }

    [JsonProperty("neg")]
    public int Neg { get; set; }

    [JsonProperty("neut")]
    public int Neut { get; set; }

    [JsonProperty("total")]
    public int Total { get; set; }

    [JsonProperty("percent")]
    public string Percent { get; set; } = string.Empty;

    [JsonProperty("percent_str")]
    public string PercentStr { get; set; } = string.Empty;
}