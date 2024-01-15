using Microsoft.AspNetCore.Mvc;
using WalletService.Core.Services.IServices;

namespace WalletService.Api.Controllers;

[ApiController]
[ApiVersion("1")]
[Route("api/v{version:apiVersion}/[controller]")]
public class WalletModel1AController : BaseController
{
    private readonly IWalletModel1AService _walletModel1AService;

    public WalletModel1AController(IWalletModel1AService walletModel1AService)
    {
        _walletModel1AService = walletModel1AService;
    }

    [HttpGet("GetBalanceInformationByAffiliateId/{affiliateId}")]
    public async Task<IActionResult> GetBalanceInformationByAffiliateId(int affiliateId)
    {
        var response = await _walletModel1AService.GetBalanceInformationByAffiliateId(affiliateId);

        return Ok(Success(response));
    }
}