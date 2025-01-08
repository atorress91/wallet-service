using Microsoft.Extensions.Options;
using WalletService.Data.Adapters.IAdapters;
using WalletService.Models.Configuration;
using WalletService.Models.DTO.LeaderBoardDto;
using WalletService.Models.Requests.RequestValidationCode;
using WalletService.Models.Responses;
using WalletService.Models.Responses.BaseResponses;
using WalletService.Utility.Extensions;
using NewtonsoftJson = Newtonsoft.Json;

namespace WalletService.Data.Adapters;

public class AccountServiceAdapter : BaseAdapter, IAccountServiceAdapter
{
    public AccountServiceAdapter(
        IOptions<ApplicationConfiguration> appSettings) : base(appSettings) { }

    protected override string? GetServiceUrl()
        => AppSettings.Endpoints?.AccountServiceURL;

    protected override string? GetTokenUrl()
        => AppSettings.EndpointTokens?.AccountServiceToken;
    
    protected override string? GetWebToken(long brandId)
    {
        return brandId switch
        {
            1 => AppSettings.WebTokens!.EcosystemToken,
            2 => AppSettings.WebTokens!.RecyCoinToken,
            3 => AppSettings.WebTokens!.HouseCoinToken,
            _ => null
        };
    }

    public Task<IRestResponse> VerificationCode(string code, string password, int userId, long brandId)
    {
        var requestValidationCode = new RequestValidationCode
        {
            Code     = code,
            Password = password,
            UserId   = userId
        };

        return Post("/userAffiliateInfo/validationCode/", requestValidationCode.ToJsonString(),brandId);
    }

    public Task<IRestResponse> GetAccountsToEcoPool(int[] id, int levels, long brandId)
    {
        var json = new
        {
            AffiliatesIds = id,
            Levels        = levels
        }.ToJsonString();

        return Post($"/userAffiliateInfo/get_accounts_eco_pool/", json, brandId);
    }

    public Task<IRestResponse> UpdateActivationDate(int id, long brandId)
    {
        return Put($"/userAffiliateInfo/update_activation_date/{id}/", brandId);
    }

    public Task<IRestResponse> RevertActivationUser(int id, long brandId)
    {
        return Put($"/userAffiliateInfo/revert_activation/{id}/", brandId);
    }
    public Task<IRestResponse> GetTotalActiveMembers(long brandId)
    {
        return Get($"/userAffiliateInfo/getTotalActiveMembers/", new Dictionary<string, string>(), brandId);
    }
    
    public async Task<IRestResponse> UpdateGradingByUser(int userId, int gradingId, long brandId)
    {
        try
        {
            var response = await Put($"/userAffiliateInfo/update_grading/{userId}/{gradingId}",brandId);
            return response;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public Task<IRestResponse> GetHave2Children(int[]                       users, long brandId)
    {
        var data = new Dictionary<string, object>
        {
            { "Users", users }
        }.ToJsonString();
        
        return Post($"/matrix/have_2_children/", data, brandId);
    }
    
    public Task<IRestResponse> GetTreeModel4(Dictionary<int, decimal>       dictionary, long brandId)
    {
        var json = dictionary.ToJsonString();
        return Post($"/leaderboard/model4/getResultTree/", json, brandId);
    }
    
    public Task<IRestResponse> AddTreeModel5(IEnumerable<LeaderBoardModel5> leaderBoard, long brandId)
    {
        return Post($"/leaderboard/model5/addTree/", leaderBoard.ToJsonString(), brandId);
    }
    
    public Task<IRestResponse> AddTreeModel6(IEnumerable<LeaderBoardModel6> leaderBoard , long brandId)
    {
        return Post($"/leaderboard/model6/addTree/", leaderBoard.ToJsonString(), brandId);
    }

    public Task<IRestResponse> DeleteTreeModel6(long brandId)
    {
        return Post($"/leaderboard/model6/deleteTree/", new Dictionary<string,string>().ToJsonString(), brandId);
    }

    public Task<IRestResponse> DeleteTreeModel5(long brandId)
    {
        return Post($"/leaderboard/model5/deleteTree/", new Dictionary<string,string>().ToJsonString(), brandId);
    }

    public async Task<UserInfoResponse?> GetUserInfo(int id, long brandId)
    {
        var response = await Get($"/userAffiliateInfo/get_user_id/{id}/", new Dictionary<string, string>(), brandId);
        if (!response.IsSuccessful)
            throw new Exception("Failed to retrieve user information");

        if (string.IsNullOrEmpty(response.Content))
            throw new Exception("User information content is empty");

        var userInfo = NewtonsoftJson.JsonConvert.DeserializeObject<UserAffiliateResponse>(response.Content);

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
    
    public async Task<NetworkDetailsResponse> NetworkDetails(int id, long brandId)
    {
        var response = await Get($"/userAffiliateInfo/getNetworkDetails/{id}", new Dictionary<string, string>(), brandId);
        if (!response.IsSuccessful)
            throw new Exception("Failed to retrieve user information");

        if (string.IsNullOrEmpty(response.Content))
            throw new Exception("Information content is empty");
        
        var networkDetails = response.Content.ToJsonObject<NetworkDetailsResponse>()!;

        return networkDetails;
    }
    
    public Task<IRestResponse> GetAffiliateByUserName(string userName, long brandId)
        => Get($"/userAffiliateInfo/get_user_username/{userName}/", new Dictionary<string, string>(), brandId);

    public Task<IRestResponse> GetPersonalNetwork(int id, long brandId)
        => Get($"/userAffiliateInfo/getPersonalNetwork/{id}/", new Dictionary<string, string>(), brandId);
    
    public Task<IRestResponse> GetAffiliateBtcByAffiliateId(int affiliateId, long brandId)
        => Get($"/AffiliateBtc/get_affiliate_btc_by_affiliate_id/{affiliateId.ToJsonString()}/", new Dictionary<string, string>(), brandId);
}