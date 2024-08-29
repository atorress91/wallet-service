using WalletService.Models.DTO.AffiliateInformation;
using WalletService.Models.DTO.LeaderBoardDto;
using WalletService.Models.Responses;
using WalletService.Models.Responses.BaseResponses;

namespace WalletService.Data.Adapters.IAdapters;

public interface IAccountServiceAdapter
{
    Task<IRestResponse> GetAffiliateByUserName(string userName, int brandId);
    Task<IRestResponse> VerificationCode(string code, string password, int userId, int brandId);
    Task<IRestResponse> GetAccountsToEcoPool(int[] id, int levels, int brandId);
    Task<UserInfoResponse?> GetUserInfo(int id, int brandId);
    Task<IRestResponse> GetPersonalNetwork(int id, int brandId);
    Task<IRestResponse> UpdateActivationDate(int id, int brandId);
    Task<IRestResponse> RevertActivationUser(int id, int brandId);
    Task<IRestResponse> GetAffiliateBtcByAffiliateId(int affiliateId, int brandId);
    Task<IRestResponse> UpdateGradingByUser(int userId, int gradingId, int brandId);
    Task<IRestResponse> GetHave2Children(int[] users, int brandId);
    Task<NetworkDetailsResponse> NetworkDetails(int userId, int brandId);
    Task<IRestResponse> GetTreeModel4(Dictionary<int, decimal> dictionary, int brandId);
    Task<IRestResponse> AddTreeModel5(IEnumerable<LeaderBoardModel5> leaderBoard, int brandId);
    Task<IRestResponse> AddTreeModel6(IEnumerable<LeaderBoardModel6> leaderBoard, int brandId);
    Task<IRestResponse> DeleteTreeModel5(int brandId);
    Task<IRestResponse> DeleteTreeModel6(int brandId);
    Task<IRestResponse> GetTotalActiveMembers(int brandId);
}