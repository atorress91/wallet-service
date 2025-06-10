using Microsoft.AspNetCore.Mvc;
using WalletService.Core.Services.IServices;
using WalletService.Data.Database.Models;
using WalletService.Models.Requests.WalletRequestRequest;

namespace WalletService.Api.Controllers;

[ApiController]
[ApiVersion("1")]
[Route("api/v{version:apiVersion}/WalletRequest")]
public class WalletRequestController : BaseController
{
    private readonly IWalletRequestService _walletRequestService;

    public WalletRequestController(IWalletRequestService walletRequestService)
    {
        _walletRequestService = walletRequestService;
    }
    #region Wallet

    [HttpGet("GetAllWalletsRequests")]
    public async Task<IActionResult> GetAllWalletsRequests()
    {
        var result = await _walletRequestService.GetAllWalletsRequests();
        return Ok(Success(result));
    }
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetAllWalletRequestByAffiliateId(int id)
    {
        var result = await _walletRequestService.GetWalletRequestById(id);
        return result is null ? Ok(Fail("The wallet request wasn't found")) : Ok(Success(result));
    }

    [HttpPost]
    public async Task<IActionResult> CreateAsync([FromBody] WalletRequestRequest request)
    {
        var result = await _walletRequestService.CreateWalletRequestAsync(request);

        if (result.IsSuccess)
            return Ok(Success(result.Value!));
        
        return BadRequest(Fail(result.Error!));
    }

    [HttpPost("processOption")]
    public async Task<IActionResult> ProcessOptionRequest([FromQuery] int option, [FromBody] List<long> ids)
    {
        var result = await _walletRequestService.ProcessOption(option, ids);

        return result is null ? Ok(Fail("The process option wasn't updated")) : Ok(Success(result));
    }

    [HttpPost("createWalletRequestRevert")]
    public async Task<IActionResult> CreateWalletRequestRevert([FromBody] WalletRequestRevertTransaction request)
    {
        var result = await _walletRequestService.CreateWalletRequestRevert(request);

        return result is null ? Ok(Fail("The wallet revert wasn't created")) : Ok(Success(result));
    }

    [HttpPut("createWalletRequestRevert")]
    public async Task<IActionResult> CreateWalletRequestRevertMobile([FromBody] WalletRequestRevertTransaction request)
    {
        var result = await _walletRequestService.CreateWalletRequestRevert(request);

        return result is null ? Ok(Fail("The wallet revert wasn't created")) : Ok(Success(result));

    }

    [HttpGet("getAllWalletRequestRevertTransaction")]
    public async Task<IActionResult> GetAllWalletRequestRevertTransaction()
    {
        var result = await _walletRequestService.GetAllWalletRequestRevertTransaction();

        return result is null ? Ok(Fail("The wallet request revert transaction wasn't found")) : Ok(Success(result));
    }

    [HttpPost("administrativePaymentAsync")]
    public async Task<IActionResult> AdministrativePaymentAsync([FromBody] WalletsRequest[] requests)
    {
        var result = await _walletRequestService.AdministrativePaymentAsync(requests);

        return result is false ? Ok(Fail("The administrative payment wasn't created")) : Ok(Success(result));

    }

    #endregion
}