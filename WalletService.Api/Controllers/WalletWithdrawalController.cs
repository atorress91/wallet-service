using Microsoft.AspNetCore.Mvc;
using WalletService.Core.Services.IServices;
using WalletService.Models.Requests.WalletWithDrawalRequest;

namespace WalletService.Api.Controllers;

[ApiController]
[ApiVersion("1")]
[Route("api/v{version:apiVersion}/WalletWithDrawal")]
public class WalletWithdrawalController : BaseController
{
    private readonly IWalletWithdrawalService _walletWithdrawalService;

    public WalletWithdrawalController(IWalletWithdrawalService walletWithdrawalService)
    {
        _walletWithdrawalService = walletWithdrawalService;
    }

    #region Wallet

    [HttpGet("GetAllWalletsWithdrawals")]
    public async Task<IActionResult> GetAllWalletsWithdrawals()
    {
        var result = await _walletWithdrawalService.GetAllWalletsWithdrawals();
        return Ok(Success(result));
    }
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetWalletWithdrawalById(int id)
    {
        var result = await _walletWithdrawalService.GetWalletWithdrawalById(id);

        return result is null ? Ok(Fail("The wallet with drawal wasn't found")) : Ok(Success(result));

    }
    [HttpPost]
    public async Task<IActionResult> CreateWalletWithdrawalAsync([FromBody] WalletWithDrawalRequest request)
    {
        var result = await _walletWithdrawalService.CreateWalletWithdrawalAsync(request);


        return result is null ? Ok(Fail("The wallet with drawal wasn't created")) : Ok(Success(result));

    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateWalletWithdrawalAsync([FromRoute] int id, [FromBody] WalletWithDrawalRequest request)
    {
        var result = await _walletWithdrawalService.UpdateWalletWithdrawalAsync(id, request);


        return result is null ? Ok(Fail("The wallet with drawal wasn't updated")) : Ok(Success(result));
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteWalletWithdrawalAsync([FromRoute] int id)
    {
        var result = await _walletWithdrawalService.DeleteWalletWithdrawalAsync(id);

        return result is null ? Ok(Fail("The wallet with drawal wasn't deleted")) : Ok(Success(result));
    }

    #endregion
}