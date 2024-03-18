using Newtonsoft.Json;
using WalletService.Models.DTO.AffiliateBtc;
using WalletService.Models.DTO.AffiliateInformation;
using WalletService.Models.DTO.ProductWalletDto;

namespace WalletService.Models.Responses;

public class ServicesResponse
{
    [JsonProperty("success")] public bool Success { get; set; }

    [JsonProperty("data")] public object? Data { get; set; }

    [JsonProperty("message")] public string Message { get; set; } = string.Empty;

    [JsonProperty("code")] public int Code { get; set; }
}

public class ServicesValidCodeAccountResponse
{
    [JsonProperty("success")] public bool Success { get; set; }

    [JsonProperty("data")] public bool Data { get; set; }

    [JsonProperty("message")] public string Message { get; set; } = string.Empty;

    [JsonProperty("code")] public int Code { get; set; }
}

public class ProductsResponse
{
    [JsonProperty("success")] public bool Success { get; set; }
    [JsonProperty("data")] public List<ProductWalletDto> Data { get; set; }

    [JsonProperty("message")] public string Message { get; set; } = string.Empty;

    [JsonProperty("code")] public int Code { get; set; }
}

public class ProductResponse
{
    [JsonProperty("success")] public bool Success { get; set; }
    [JsonProperty("data")] public ProductWalletDto Data { get; set; }

    [JsonProperty("message")] public string Message { get; set; } = string.Empty;

    [JsonProperty("code")] public int Code { get; set; }
}

public class UserAffiliateResponse
{ 
    [JsonProperty("success")] public bool Success { get; set; }

    [JsonProperty("data")] public UserInfoResponse? Data { get; set; }

    [JsonProperty("message")] public string Message { get; set; } = string.Empty;

    [JsonProperty("code")] public int Code { get; set; }
    
}

public class UserAffiliatePointInformation
{ 
    [JsonProperty("success")] public bool Success { get; set; }

    [JsonProperty("data")] public ICollection<UserBinaryInformation> Data { get; set; }

    [JsonProperty("message")] public string Message { get; set; } = string.Empty;

    [JsonProperty("code")] public int Code { get; set; }
    
}

public class UserPersonalNetworkResponse
{ 
    [JsonProperty("success")] public bool Success { get; set; }

    [JsonProperty("data")] public List<PersonalNetwork> Data { get; set; }

    [JsonProperty("message")] public string Message { get; set; } = string.Empty;

    [JsonProperty("code")] public int Code { get; set; }
    
}

public class GetTotalActiveMembersResponse
{ 
    [JsonProperty("success")] public bool Success { get; set; }

    [JsonProperty("data")] public int Data { get; set; }

    [JsonProperty("message")] public string Message { get; set; } = string.Empty;

    [JsonProperty("code")] public int Code { get; set; }
    
}

public class AffiliateBtcResponse
{
    [JsonProperty("success")] public bool Success { get; set; }

    [JsonProperty("data")] public AffiliateBtcDto? Data { get; set; }

    [JsonProperty("message")] public string Message { get; set; } = string.Empty;

    [JsonProperty("code")] public int Code { get; set; }
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
