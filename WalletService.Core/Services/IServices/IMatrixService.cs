using Microsoft.AspNetCore.Http;
using WalletService.Models.Requests.ConPaymentRequest;
using WalletService.Models.Requests.MatrixRequest;
using WalletService.Models.Responses;

namespace WalletService.Core.Services.IServices;

public interface IMatrixService
{
    Task<bool> CheckQualificationAsync(long userId, int matrixType);
    Task<(bool anyQualified, List<int> qualifiedMatrixTypes)> ProcessAllMatrixQualificationsAsync(int userId);
    Task<bool> ProcessAdminMatrixPlacementAsync(int userId, int matrixType);
    Task<bool> ProcessDirectPaymentMatrixActivationAsync(MatrixRequest request);
    Task FixInconsistentQualificationRecordsAsync();
    Task<BatchProcessingResult> ProcessAllUsersMatrixQualificationsAsync(int[]? userIds = null);
    Task<bool> CoinPaymentsMatrixActivationConfirmation(IpnRequest request,IHeaderDictionary headers);
    Task<bool> HasReachedWithdrawalLimitAsync(int userId);
}