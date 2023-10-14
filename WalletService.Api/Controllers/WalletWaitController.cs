using Microsoft.AspNetCore.Mvc;
using WalletService.Core.Services.IServices;
using WalletService.Models.Requests.WalletWaitRequest;

namespace WalletService.Api.Controllers;

[ApiController]
[ApiVersion("1")]
[Route("api/v{version:apiVersion}/WalletWait")]
public class WalletWaitController : BaseController
{
    private readonly IWalletWaitService _walletWaitService;

    public WalletWaitController(IWalletWaitService walletWaitService)
    {
        _walletWaitService = walletWaitService;
    }

    #region Wallet

    [HttpGet("GetAllWalletsWaits")]
    public async Task<IActionResult> GetAllWalletsWaits()
    {
        var result = await _walletWaitService.GetAllWalletsWaits();
        return Ok(Success(result));
    }
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetWalletById(int id)
    {
        var result = await _walletWaitService.GetWalletWaitById(id);
        return result is null ? Ok(Fail("The wallet wait wasn't found")) : Ok(Success(result));
    }
    [HttpPost]
    public async Task<IActionResult> CreateWalletWaitAsync([FromBody] WalletWaitRequest request)
    {
        var result = await _walletWaitService.CreateWalletWaitAsync(request);

        return result is null ? Ok(Fail("The wallet wait wasn't created")) : Ok(Success(result));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateWalletWaitAsync([FromRoute] int id, [FromBody] WalletWaitRequest request)
    {
        var result = await _walletWaitService.UpdateWalletWaitAsync(id, request);

        return result is null ? Ok(Fail("The wallet wait wasn't updated")) : Ok(Success(result));
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteWalletWaitAsync([FromRoute] int id)
    {
        var result = await _walletWaitService.DeleteWalletWaitAsync(id);

        return result is null ? Ok(Fail("The wallet wait wasn't deleted")) : Ok(Success(result));
    }

    #endregion
}