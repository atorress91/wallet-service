using Microsoft.AspNetCore.Mvc;
using WalletService.Core.Services.IServices;
using WalletService.Models.Responses;

namespace WalletService.Api.Controllers;


[ApiController]
[ApiVersion("1")]
[Route("api/v{version:apiVersion}/[controller]")]
public class UserStatisticsController : BaseController
{
    private readonly IUserStatisticsService _userStatisticsService;

    public UserStatisticsController(IUserStatisticsService userStatisticsService)
    {
        _userStatisticsService = userStatisticsService;
    }

    [HttpGet("{userId}")]
    public async Task<ServicesResponse> GetUserStatistics(int userId)
    {
        var data = await _userStatisticsService.GetUserStatisticsAsync(userId);
        return Success(data);
    }
}