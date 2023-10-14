using WalletService.Models.DTO.AffiliateInformation;
using WalletService.Models.Responses;
using WalletService.Models.Responses.BaseResponses;

namespace WalletService.Data.Adapters.IAdapters;

public interface IAccountServiceAdapter
{
    Task<IRestResponse> GetAffiliateByUserName(string userName);
    Task<IRestResponse> VerificationCode(string                       code, string password, int userId);
    Task<IRestResponse> GetAccountsToEcoPool(int[]                    id,   int    levels);
    Task<UserInfoResponse?> GetUserInfo(int                               id);
    Task<IRestResponse> GetPersonalNetwork(int                        id);
    Task<IRestResponse> UpdateActivationDate(int                      id);
    Task<IRestResponse> GetTotalActiveMembers();
    Task<IRestResponse> RevertActivationUser(int id);
    Task<IRestResponse> GetAffiliateBtcByAffiliateId(int affiliateId);
  
    Task<ICollection<UserBinaryInformation>> CalculatePointPerUser(Dictionary<int, decimal> dictionary);
    Task<IRestResponse> UpdateGradingByUser(int                                             userId, int gradingId);


}