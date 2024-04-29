using System.Text.Json.Serialization;
using WalletService.Models.DTO.AffiliateBtc;
using WalletService.Models.DTO.AffiliateInformation;
using WalletService.Models.DTO.ProductWalletDto;
using NewtonsoftJson = Newtonsoft.Json;

namespace WalletService.Models.Responses;

public class ServicesResponse
{
    [JsonPropertyName("success")] public bool Success { get; set; }

    [JsonPropertyName("data")] public object? Data { get; set; }

    [JsonPropertyName("message")] public string Message { get; set; } = string.Empty;

    [JsonPropertyName("code")] public int Code { get; set; }
}

public class ServicesValidCodeAccountResponse
{
    [JsonPropertyName("success")] public bool Success { get; set; }

    [JsonPropertyName("data")] public bool Data { get; set; }

    [JsonPropertyName("message")] public string Message { get; set; } = string.Empty;

    [JsonPropertyName("code")] public int Code { get; set; }
}

public class ProductsResponse
{
    [JsonPropertyName("success")] public bool Success { get; set; }
    [JsonPropertyName("data")] public List<ProductWalletDto> Data { get; set; }

    [JsonPropertyName("message")] public string Message { get; set; } = string.Empty;

    [JsonPropertyName("code")] public int Code { get; set; }
}

public class ProductResponse
{
    [JsonPropertyName("success")] public bool Success { get; set; }
    [JsonPropertyName("data")] public ProductWalletDto Data { get; set; }

    [JsonPropertyName("message")] public string Message { get; set; } = string.Empty;

    [JsonPropertyName("code")] public int Code { get; set; }
}

public class UserAffiliateResponse
{ 
    [NewtonsoftJson.JsonProperty] public bool Success { get; set; }

    [NewtonsoftJson.JsonProperty("data")] public UserInfoResponse? Data { get; set; }

    [NewtonsoftJson.JsonProperty("message")] public string Message { get; set; } = string.Empty;

    [NewtonsoftJson.JsonProperty("code")] public int Code { get; set; }
    
}

public class UserAffiliatePointInformation
{ 
    [JsonPropertyName("success")] public bool Success { get; set; }

    [JsonPropertyName("data")] public ICollection<UserBinaryInformation> Data { get; set; }

    [JsonPropertyName("message")] public string Message { get; set; } = string.Empty;

    [JsonPropertyName("code")] public int Code { get; set; }
    
}

public class UserPersonalNetworkResponse
{ 
    [JsonPropertyName("success")] public bool Success { get; set; }

    [JsonPropertyName("data")] public List<PersonalNetwork> Data { get; set; }

    [JsonPropertyName("message")] public string Message { get; set; } = string.Empty;

    [JsonPropertyName("code")] public int Code { get; set; }
    
}

public class GetTotalActiveMembersResponse
{ 
    [JsonPropertyName("success")] public bool Success { get; set; }

    [JsonPropertyName("data")] public int Data { get; set; }

    [JsonPropertyName("message")] public string Message { get; set; } = string.Empty;

    [JsonPropertyName("code")] public int Code { get; set; }
    
}

public class AffiliateBtcResponse
{
    [JsonPropertyName("success")] public bool Success { get; set; }

    [JsonPropertyName("data")] public AffiliateBtcDto? Data { get; set; }

    [JsonPropertyName("message")] public string Message { get; set; } = string.Empty;

    [JsonPropertyName("code")] public int Code { get; set; }
}

public class PersonalNetwork
{

    public int id { get; set; }
    public string fullName { get; set; }
    public string email { get; set; }
    public string userName { get; set; }
    public int externalGradingId { get; set; }
    public byte status { get; set; }
    public decimal latitude { get; set; }
    public decimal longitude { get; set; }
    public string countryName { get; set; }

}
