using Microsoft.AspNetCore.Mvc;
using WalletService.Core.Services.IServices;
using WalletService.Data.Database.Models;
using WalletService.Models.Requests.ConPaymentRequest;

namespace WalletService.Api.Controllers;

[ApiController]
[ApiVersion("1")]
[Route("api/v{version:apiVersion}/ConPayments")]
public class CoinPaymentsController : BaseController
{
    private readonly IConPaymentService _conPaymentService;


    public CoinPaymentsController(IConPaymentService conPaymentService)
    {
        _conPaymentService = conPaymentService;
    }


    [HttpGet("profile")]
    public async Task<IActionResult> GetProfile(string pbntag)
    {
        var response = await _conPaymentService.GetPayByNameProfile(pbntag);

        if (string.IsNullOrEmpty(response.Error) || response.Error.ToLower() == "ok")
        {
            return Ok(response.Result);
        }

        return Ok(Fail("Profile not found"));
    }

    [HttpGet("getDepositAddress")]
    public async Task<IActionResult> GetDepositAddress(string currency)
    {
        var response = await _conPaymentService.GetDepositAddress(currency);

        if (string.IsNullOrEmpty(response!.Error) || response.Error.ToLower() == "ok")
        {
            return Ok(response.Result);
        }

        return Ok(Fail("Address not found"));
    }

    [HttpGet("getCoinBalances")]
    public async Task<IActionResult> GetCoinBalances(bool includeZeroBalances)
    {
        var response = await _conPaymentService.GetCoinBalances(includeZeroBalances);

        return Ok(Success(response.Result));
    }

    [HttpPost("createPayment")]
    public async Task<IActionResult> CreatePayment(ConPaymentRequest request)
    {
        var response = await _conPaymentService.CreatePayment(request);

        if (string.IsNullOrEmpty(response!.Error) || response.Error.ToLower() == "ok")
        {
            return Ok(response.Result);
        }

        return Ok(Fail("Transaction could not be processed"));
    }

    [HttpGet("getTransactionInfo")]
    public async Task<IActionResult> GetTransactionInfo(string idTransaction, bool fullInfo)
    {
        var response = await _conPaymentService.GetTransactionInfo(idTransaction, fullInfo);

        return Ok(Success(response.Result));
    }

    [HttpPost("coinPaymentsIPN")]
    public async Task<IActionResult> CoinPaymentsIpn([FromForm] IpnRequest ipnRequest)
    {
        var response = await _conPaymentService.ProcessIpnAsync(ipnRequest, Request.Headers);

        if (response == "IPN OK")
            return Ok(response);
        else
            return BadRequest(response);
    }
    
    [HttpPost("createMassWithdrawal")]
    public async Task<IActionResult> CreateMassWithdrawal([FromBody] WalletsRequests[] requests)
    {
        var response = await _conPaymentService.CreateMassWithdrawal(requests);
        return response is null ? Ok(Fail("The withdrawal could not be created correctly")) : Ok(response);
    }
}