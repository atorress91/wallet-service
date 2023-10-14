using System.Text.Json;
using Microsoft.Extensions.Options;
using WalletService.Data.Adapters.IAdapters;
using WalletService.Models.Configuration;
using WalletService.Models.DTO.AffiliateInformation;
using WalletService.Models.Requests.RequestValidationCode;
using WalletService.Models.Requests.TransferBalanceRequest;
using WalletService.Models.Responses;
using WalletService.Models.Responses.BaseResponses;
using WalletService.Utility.Extensions;

namespace WalletService.Data.Adapters;

public class AccountServiceAdapter : BaseAdapter, IAccountServiceAdapter
{
    public AccountServiceAdapter(
        IOptions<ApplicationConfiguration> appSettings,
        IHttpClientFactory                 httpClientFactory,
        HttpClient                         httpClient) : base(appSettings, httpClientFactory, httpClient) { }

    protected override string? GetServiceUrl()
        => AppSettings.Endpoints?.AccountServiceURL;

    protected override string? GetTokenUrl()
        => AppSettings.EndpointTokens?.AccountServiceToken;

    public Task<IRestResponse> VerificationCode(string code, string password, int userId)
    {
        var requestValidationCode = new RequestValidationCode
        {
            Code     = code,
            Password = password,
            UserId   = userId
        };

        return Post("/userAffiliateInfo/validationCode/", requestValidationCode.ToJsonString());
    }

    public Task<IRestResponse> GetAccountsToEcoPool(int[] id, int levels)
    {
        var json = new
        {
            AffiliatesIds = id,
            Levels        = levels
        }.ToJsonString();

        return Post($"/userAffiliateInfo/get_accounts_eco_pool/", json);
    }

    public Task<IRestResponse> UpdateActivationDate(int id)
    {
        return Put($"/userAffiliateInfo/update_activation_date/{id}/");
    }

    public Task<IRestResponse> RevertActivationUser(int id)
    {
        return Put($"/userAffiliateInfo/revert_activation/{id}/");
    }

    public async Task<ICollection<UserBinaryInformation>> CalculatePointPerUser(Dictionary<int, decimal> dictionary)
    {
        var userBinaryInformation = new List<UserBinaryInformation>();
        
        var data = new Dictionary<string, Dictionary<int, decimal>>()
        {
            { "Users", dictionary }
        }.ToJsonString();
        
        var response = await Post($"matrix/binary_complete/", data);
        if (!response.IsSuccessful)
            throw new Exception("Failed to retrieve user information");

        if (string.IsNullOrEmpty(response.Content))
            throw new Exception("User information content is empty");

        var result = JsonSerializer.Deserialize<UserAffiliatePointInformation>(response.Content);
        return result!.Success ? result.Data : userBinaryInformation;
    }

    public Task<IRestResponse> GetTotalActiveMembers()
    {
        return Get($"/userAffiliateInfo/getTotalActiveMembers/", new Dictionary<string, string>());
    }
    
    public async Task<IRestResponse> UpdateGradingByUser(int userId, int gradingId)
    {
        try
        {
            var response = await Put($"/userAffiliateInfo/update_grading/{userId}/{gradingId}");
            return response;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
    
    public async Task<UserInfoResponse?> GetUserInfo(int id)
    {
        var response = await Get($"/userAffiliateInfo/get_user_id/{id}/", new Dictionary<string, string>());
        if (!response.IsSuccessful)
            throw new Exception("Failed to retrieve user information");

        if (string.IsNullOrEmpty(response.Content))
            throw new Exception("User information content is empty");

        var userInfo = JsonSerializer.Deserialize<UserAffiliateResponse>(response.Content);

        if (userInfo?.Data is null)
            return null;

        var userInfoResponse = new UserInfoResponse
        {
            Id                  = userInfo.Data.Id,
            Name                = userInfo.Data?.Name,
            UserName            = userInfo.Data?.UserName,
            Country             = userInfo.Data?.Country,
            LastName            = userInfo.Data?.LastName,
            City                = userInfo.Data?.City,
            Phone               = userInfo.Data?.Phone,
            Email               = userInfo.Data?.Email,
            Address             = userInfo.Data?.Address,
            CardIdAuthorization = userInfo.Data?.CardIdAuthorization,
            Father              = userInfo.Data!.Father,
        };

        return userInfoResponse;
    }

    public Task<IRestResponse> GetAffiliateByUserName(string userName)
        => Get($"/userAffiliateInfo/get_user_username/{userName}/", new Dictionary<string, string>());

    public Task<IRestResponse> GetPersonalNetwork(int id)
        => Get($"/userAffiliateInfo/getPersonalNetwork/{id}/", new Dictionary<string, string>());
    
    public Task<IRestResponse> GetAffiliateBtcByAffiliateId(int affiliateId)
        => Get($"/AffiliateBtc/get_affiliate_btc_by_affiliate_id/{affiliateId.ToJsonString()}/", new Dictionary<string, string>());
}