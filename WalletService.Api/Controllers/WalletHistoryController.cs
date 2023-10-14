using Microsoft.AspNetCore.Mvc;
using WalletService.Core.Services.IServices;
using WalletService.Models.Requests.WalletHistoryRequest;
using WalletService.Models.Requests.WalletRequest;

namespace WalletService.Api.Controllers;

[ApiController]
[ApiVersion("1")]
[Route("api/v{version:apiVersion}/WalletHistory")]
public class WalletHistoryController : BaseController
{
    private readonly IWalletHistoryService _walletHistoryService;

    public WalletHistoryController(IWalletHistoryService walletHistoryService)
    {
        _walletHistoryService = walletHistoryService;
    }
    #region Wallet

    [HttpGet("GetAllWalletsHistories")]
    public async Task<IActionResult> GetAllWalletsHistoriesAsync()
    {
        var result = await _walletHistoryService.GetAllWalletsHistoriesAsync();
        return Ok(Success(result));
    }
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetWalletHistoriesByIdAsync(int id)
    {
        var result = await _walletHistoryService.GetWalletHistoriesByIdAsync(id);
        return result is null ? Ok(Fail("The wallet history wasn't found")) : Ok(Success(result));
    }

    [HttpPost]
    public async Task<IActionResult> CreateWalletHistoriesAsync([FromBody] WalletHistoryRequest request)
    {
        var result = await _walletHistoryService.CreateWalletHistoriesAsync(request);

        return result is null ? Ok(Fail("The wallet history wasn't created")) : Ok(Success(result));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateWalletHistoriesAsync([FromRoute] int id, [FromBody] WalletHistoryRequest request)
    {
        var result = await _walletHistoryService.UpdateWalletHistoriesAsync(id, request);

        return result is null ? Ok(Fail("The wallet history wasn't updated")) : Ok(Success(result));
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteWalletHistoriesAsync([FromRoute] int id)
    {
        var result = await _walletHistoryService.DeleteWalletHistoriesAsync(id);

        return result is null ? Ok(Fail("The wallet history wasn't deleted")) : Ok(Success(result));
    }

    #endregion
}