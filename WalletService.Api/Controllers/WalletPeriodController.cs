using Microsoft.AspNetCore.Mvc;
using WalletService.Core.Services.IServices;
using WalletService.Models.Requests.WalletPeriodRequest;

namespace WalletService.Api.Controllers;

[ApiController]
[ApiVersion("1")]
[Route("api/v{version:apiVersion}/WalletPeriod")]
public class WalletPeriodController : BaseController
{
    private readonly IWalletPeriodService _walletPeriodService;

    public WalletPeriodController(IWalletPeriodService walletPeriodService)
    {
        _walletPeriodService = walletPeriodService;
    }
    #region Wallet

    [HttpGet("GetAllWalletsPeriods")]
    public async Task<IActionResult> GetAllWalletsPeriods()
    {
        var result = await _walletPeriodService.GetAllWalletsPeriods();

        return Ok(Success(result));
    }
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetWalletPeriodById(int id)
    {
        var result = await _walletPeriodService.GetWalletPeriodById(id);
        return result is null ? Ok(Fail("The wallet period wasn't found")) : Ok(Success(result));
    }

    [HttpPost]
    public async Task<IActionResult> CreateWalletPeriodAsync([FromBody] WalletPeriodRequest[] request)
    {
        await _walletPeriodService.CreateWalletPeriodAsync(request);

        return Ok(Success("Wallet periods created successfully"));
    }


    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteWalletPeriodAsync([FromRoute] int id)
    {
        var result = await _walletPeriodService.DeleteWalletPeriodAsync(id);

        return result is null ? Ok(Fail("The wallet period wasn't deleted")) : Ok(Success(result));
    }

    #endregion
}