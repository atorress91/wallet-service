using Microsoft.AspNetCore.Mvc;
using WalletService.Core.Services.IServices;
using WalletService.Models.Requests.WalletRequest;


namespace WalletService.Api.Controllers;

[ApiController]
[ApiVersion("1")]
[Route("api/v{version:apiVersion}/[controller]")]
public class WalletModel1BController : BaseController
{
    private readonly IWalletModel1BService _walletModel1BService;

    public WalletModel1BController(IWalletModel1BService walletModel1BService)
    {
        _walletModel1BService = walletModel1BService;
    }

    [HttpGet("GetBalanceInformationByAffiliateId/{affiliateId}")]
    public async Task<IActionResult> GetBalanceInformationByAffiliateId(int affiliateId)
    {
        var response = await _walletModel1BService
            .GetBalanceInformationByAffiliateId(affiliateId);

        return Ok(Success(response));
    }
    
    [HttpPost("payWithMyBalance1B")]
    public async Task<IActionResult> PayWithMyBalance([FromBody] WalletRequest request)
    {
        var response = await _walletModel1BService.PayWithMyBalance(request);

        return response is false ? Ok(Fail("The payment could not be processed")) : Ok(Success(response));
    }
    
    [HttpPost("payWithMyServiceBalance")]
    public async Task<IActionResult> PayWithMyServiceBalance([FromBody] WalletRequest request)
    {
        var response = await _walletModel1BService.PayWithMyServiceBalance(request);

        return response is false ? Ok(Fail("The payment could not be processed")) : Ok(Success(response));
    }
    
    [HttpPost("CreateServiceBalanceAdmin")]
    public async Task<IActionResult> CreateServiceBalanceAdmin([FromBody] CreditTransactionAdminRequest request)
    {
        var response = await _walletModel1BService.CreateServiceBalanceAdmin(request);

        return response is false ? Ok(Fail("The payment could not be processed")) : Ok(Success(response));
    }
}