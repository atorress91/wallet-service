using WalletService.Models.DTO.AffiliateInformation;
using WalletService.Models.DTO.LeaderBoardDto;
using WalletService.Models.Responses;
using WalletService.Models.Responses.BaseResponses;

namespace WalletService.Data.Adapters.IAdapters;

public interface IAccountServiceAdapter
{
    Task<IRestResponse> GetAffiliateByUserName(string                userName);
    Task<IRestResponse> VerificationCode(string                      code, string password, int userId);
    Task<IRestResponse> GetAccountsToEcoPool(int[]                   id,   int    levels);
    Task<UserInfoResponse?> GetUserInfo(int                          id);
    Task<IRestResponse> GetPersonalNetwork(int                       id);
    Task<IRestResponse> UpdateActivationDate(int                     id);
    Task<IRestResponse> RevertActivationUser(int                     id);
    Task<IRestResponse> GetAffiliateBtcByAffiliateId(int             affiliateId);
    Task<IRestResponse> UpdateGradingByUser(int                      userId, int gradingId);
    Task<IRestResponse> GetHave2Children(int[]                       users);
    Task<NetworkDetailsResponse> NetworkDetails(int                           userId);
    Task<IRestResponse> GetTreeModel4(Dictionary<int, decimal>       dictionary);
    Task<IRestResponse> AddTreeModel5(IEnumerable<LeaderBoardModel5> leaderBoard);
    Task<IRestResponse> AddTreeModel6(IEnumerable<LeaderBoardModel6> leaderBoard);
    Task<IRestResponse> DeleteTreeModel5();
    Task<IRestResponse> DeleteTreeModel6();
    Task<IRestResponse> GetTotalActiveMembers();

}