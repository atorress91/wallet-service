using WalletService.Models.DTO.AffiliateInformation;

namespace WalletService.Core.Services.IServices;

public interface IUserStatisticsService
{
    Task<UserStatistics> GetUserStatisticsAsync(int userId);
}