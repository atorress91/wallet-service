using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using WalletService.Core.Services.IServices;
using WalletService.Models.Requests.TransferBalanceRequest;
using WalletService.Models.Requests.WalletRequest;

namespace WalletService.Api.Controllers;

[ApiController]
[ApiVersion("1")]
[Route("api/v{version:apiVersion}/Wallet")]
public class WalletController : BaseController
{
    private readonly IWalletService _walletService;

    public WalletController(IWalletService walletService)
    {
        _walletService = walletService;
    }


    #region Wallet

    [HttpGet("GetAllWallets")]
    public async Task<IActionResult> GetAllWallets()
    {
        var result = await _walletService.GetAllWallets();
        return Ok(Success(result));
    }

    [HttpGet("GetWalletsRequest")]
    public async Task<IActionResult> GetWalletsRequest([FromQuery] int userId)
    {
        var result = await _walletService.GetWalletsRequest(userId);
        return Ok(Success(result));
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetWalletById(int id)
    {
        var result = await _walletService.GetWalletById(id);
        return result is null ? Ok(Fail("The wallet wasn't found")) : Ok(Success(result));
    }

    [HttpGet("GetWalletByAffiliateId/{id:int}")]
    public async Task<IActionResult> GetWalletByAffiliateId(int id)
    {
        var result = await _walletService.GetWalletByAffiliateId(id);
        return result.IsNullOrEmpty() ? Ok(Fail("The wallet wasn't found")) : Ok(Success(result));
    }

    [HttpGet("GetWalletByUserId/{id:int}")]
    public async Task<IActionResult> GetWalletByUserId(int id)
    {
        var result = await _walletService.GetWalletByUserId(id);
        return result.IsNullOrEmpty() ? Ok(Fail("The wallet wasn't found")) : Ok(Success(result));
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteWalletAsync([FromRoute] int id)
    {
        var result = await _walletService.DeleteWalletAsync(id);

        return result is null ? BadRequest("The wallet wasn't delete") : Ok(Success(result));
    }

    [HttpGet("GetBalanceInformationByAffiliateId/{id:int}")]
    public async Task<IActionResult> GetBalanceInformationByAffiliateId(int id)
    {
        var result = await _walletService.GetBalanceInformationByAffiliateId(id);
        return Ok(Success(result));
    }

    [HttpGet("GetBalanceInformationAdmin")]
    public async Task<IActionResult> GetBalanceInformationAdmin()
    {
        var result = await _walletService.GetBalanceInformationAdmin();
        return Ok(Success(result));
    }


    [HttpPost("payWithMyBalance")]
    public async Task<IActionResult> PayWithMyBalance([FromBody] WalletRequest request)
    {
        var response = await _walletService.PaymentHandler(request);

        return response is false ? Ok(Fail("The payment could not be processed")) : Ok(Success(response));
    }

    [HttpPost("payWithMyBalanceAdmin")]
    public async Task<IActionResult> PayWithMyBalanceAdmin([FromBody] WalletRequest request)
    {
        var response = await _walletService.AdminPaymentHandler(request);

        return response is false ? Ok(Fail("The payment could not be processed")) : Ok(Success(response));
    }

    [HttpPut("payWithMyBalance")]
    public async Task<IActionResult> PayWithMyBalanceMobile([FromBody] WalletRequest request)
    {
        var response = await _walletService.PaymentHandler(request);

        return response is false ? Ok(Fail("The payment could not be processed")) : Ok(Success(response));
    }

    [HttpPost("transferBalanceForNewAffiliates")]
    public async Task<IActionResult> TransferBalanceForNewAffiliates([FromBody] TransferBalanceRequest request)
    {
        var response = await _walletService.TransferBalanceForNewAffiliate(request);

        return Ok(response);
    }

    [HttpPut("transferBalanceForNewAffiliatesMobile")]
    public async Task<IActionResult> TransferBalanceForNewAffiliatesMobile([FromBody] TransferBalanceRequest request)
    {
        var response = await _walletService.TransferBalanceForNewAffiliate(request);

        return Ok(response);
    }

    [HttpPost("transferBalance")]
    public async Task<IActionResult> TransferBalance([FromBody] string encrypted)
    {
        var response = await _walletService.TransferBalance(encrypted);

        return Ok(response);
    }


    [HttpPost("rejectOrCancelRevertDebitTransaction")]
    public async Task<IActionResult> RejectOrCancelRevertDebitTransaction([FromQuery] int option, [FromBody] int id)
    {
        var response = await _walletService.HandleWalletRequestRevertTransactionAsync(option, id);

        return response is false ? Ok(Fail("The revert transaction could not be processed")) : Ok(Success(response));
    }

    [HttpGet("getPurchasesMadeInMyNetwork/{id:int}")]
    public async Task<IActionResult> GetPurchasesMadeInMyNetwork([FromRoute] int id)
    {
        var result = await _walletService.GetPurchasesMadeInMyNetwork(id);

        if (result == null)
        {
            return Ok(Fail("No purchases found on the network"));
        }
        else
        {
            var response = new
            {
                result.Value.CurrentYearPurchases,
                result.Value.PreviousYearPurchases
            };

            return Ok(Success(response));
        }
    }
    
    [HttpGet("GetAllAffiliatesWithPositiveBalance")]
    public async Task<IActionResult> GetAllAffiliatesWithPositiveBalance()
    {
        var result = await _walletService.GetAllAffiliatesWithPositiveBalance();
        return Ok(Success(result));
    }

    #endregion
}