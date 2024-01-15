using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver.Core.Events;
using WalletService.Core.Services.IServices;


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
        var response = await _walletModel1BService.GetBalanceInformationByAffiliateId(affiliateId);

        return Ok(Success(response));
    }
}