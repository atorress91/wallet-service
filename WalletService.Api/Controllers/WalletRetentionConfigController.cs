using Microsoft.AspNetCore.Mvc;
using WalletService.Core.Services.IServices;
using WalletService.Models.Requests.WalletHistoryRequest;
using WalletService.Models.Requests.WalletRetentionConfigRequest;

namespace WalletService.Api.Controllers;

[ApiController]
[ApiVersion("1")]
[Route("api/v{version:apiVersion}/WalletRetentionConfig")]
public class WalletRetentionConfigController : BaseController
{
    private readonly IWalletRetentionConfigService _walletRetentionConfigService;

    public WalletRetentionConfigController(IWalletRetentionConfigService walletRetentionConfigService)
    {
        _walletRetentionConfigService = walletRetentionConfigService;
    }

    #region Wallet

    [HttpGet("GetAllWalletsRetentionConfig")]
    public async Task<IActionResult> GetAllWalletsRetentionConfig()
    {
        var result = await _walletRetentionConfigService.GetAllWalletsRetentionConfig();
        return Ok(Success(result));
    }
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetWalletRetentionConfigById(int id)
    {
        var result = await _walletRetentionConfigService.GetWalletRetentionConfigById(id);
        return result is null ? Ok(Fail("The wallet retention wasn't found")) : Ok(Success(result));
    }

    [HttpPost]
    public async Task<IActionResult> CreateWalletRetentionConfigAsync([FromBody] WalletRetentionConfigRequest[] request)
    {
        var result = await _walletRetentionConfigService.CreateWalletRetentionConfigAsync(request);

        return Ok(Success(result));
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteWalletRetentionConfigAsync([FromRoute] int id)
    {
        var result = await _walletRetentionConfigService.DeleteWalletRetentionConfigAsync(id);

        return result is null ? Ok(Fail("The wallet retention wasn't deleted")) : Ok(Success(result));
    }

    #endregion
}