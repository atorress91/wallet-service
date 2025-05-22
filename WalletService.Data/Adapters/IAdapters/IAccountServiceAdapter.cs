using WalletService.Models.DTO.LeaderBoardDto;
using WalletService.Models.Requests.MatrixRequest;
using WalletService.Models.Responses;
using WalletService.Models.Responses.BaseResponses;

namespace WalletService.Data.Adapters.IAdapters;

public interface IAccountServiceAdapter
{
    Task<IRestResponse> GetAffiliateByUserName(string userName, long brandId);
    Task<IRestResponse> VerificationCode(string code, string password, int userId, long brandId);
    Task<IRestResponse> GetAccountsToEcoPool(int[] id, int levels, long brandId);
    Task<UserInfoResponse?> GetUserInfo(int id, long brandId);
    Task<IRestResponse> GetPersonalNetwork(int id, long brandId);
    Task<IRestResponse> UpdateActivationDate(int id, long brandId);
    Task<IRestResponse> RevertActivationUser(int id, long brandId);
    Task<IRestResponse> GetAffiliateBtcByAffiliateId(int affiliateId, long brandId);
    Task<IRestResponse> UpdateGradingByUser(int userId, int gradingId, long brandId);
    Task<IRestResponse> GetHave2Children(int[] users, long brandId);
    Task<NetworkDetailsResponse> NetworkDetails(int userId, long brandId);
    Task<IRestResponse> GetTreeModel4(Dictionary<int, decimal> dictionary, long brandId);
    Task<IRestResponse> AddTreeModel5(IEnumerable<LeaderBoardModel5> leaderBoard, long brandId);
    Task<IRestResponse> AddTreeModel6(IEnumerable<LeaderBoardModel6> leaderBoard, long brandId);
    Task<IRestResponse> DeleteTreeModel5(long brandId);
    Task<IRestResponse> DeleteTreeModel6(long brandId);
    Task<IRestResponse> GetTotalActiveMembers(long brandId);
    Task<IRestResponse> PlaceUserInMatrix(MatrixRequest request, long brandId);
    Task<IRestResponse> IsActiveInMatrix(MatrixRequest request, long brandId);
    Task<IRestResponse> GetUplinePositionsAsync(MatrixRequest request, long brandId);
}