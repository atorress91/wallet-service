using Microsoft.AspNetCore.Mvc;
using WalletService.Core.Services.IServices;
using WalletService.Models.Requests.CoinPayRequest;

namespace WalletService.Api.Controllers;

[ApiController]
[ApiVersion("1")]
[Route("api/v{version:apiVersion}/CoinPay")]
public class CoinPayController : BaseController
{
    private readonly ICoinPayService _coinPayService;
    private readonly ILogger<CoinPayController> _logger;

    public CoinPayController(ICoinPayService coinPayService, ILogger<CoinPayController> logger)
    {
        _coinPayService = coinPayService;
        _logger = logger;
    }

    #region Coinpay

    [HttpPost("createTransaction")]
    public async Task<IActionResult> CreateTransaction([FromBody] CreateTransactionRequest request)
    {
        var result = await _coinPayService.CreateTransaction(request);
        return result?.Data is null ? Ok(Fail("Error")) : Ok(result);
    }

    [HttpPost("createChannel")]
    public async Task<IActionResult> CreateChannel([FromBody] CreateTransactionRequest request)
    {
        var result = await _coinPayService.CreateChannel(request);

        return result?.Data is null ? Ok(Fail("The channel could not be created.")) : Ok(Success(result));
    }

    [HttpGet("getNetworksByIdCurrency")]
    public async Task<IActionResult> GetNetworkByIdCurrency([FromQuery]int idCurrency)
    {
        var result = await _coinPayService.GetNetworksByIdCurrency(idCurrency);

        return result?.Data is null ? Ok(Fail("Network not found for the provided ID.")) : Ok(result);
    }

    [HttpPost("createAddress")]
    public async Task<IActionResult> CreateAddress([FromBody] CreateAddresRequest request)
    {
        var result = await _coinPayService.CreateAddress(request);

        return result?.Data is null ? Ok(Fail("The address could not be created.")) : Ok(result);
    }

    [HttpGet("getTransactionById")]
    public async Task<IActionResult> GetTransactionById(int idTransaction)
    {
        var result = await _coinPayService.GetTransactionById(idTransaction);

        return result?.Data is null ? Ok(Fail("Transaction not found.")) : Ok(Success(result));
    }

    [HttpPost("Webhook")]
    public async Task<IActionResult> Webhook()
    {
        Request.EnableBuffering();
        string requestBody;

        using (var reader = new StreamReader(Request.Body, leaveOpen: true))
        {
            requestBody = await reader.ReadToEndAsync();
        }

        Request.Body.Position = 0;

        try
        {
            var result = await _coinPayService.ReceiveCoinPayNotifications(requestBody);

            _logger.LogInformation("Processing result: {Result}", result);

            return result is false ? Ok(Fail("The notification could not be processed.")) : Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing the Webhook notification.");

            return StatusCode(500, "Internal Server Error: " + ex.Message);
        }
    }

    [HttpPost("sendFunds")]
    public async Task<IActionResult> SendFunds([FromBody] WithDrawalRequest[] request)
    {
        var result = await _coinPayService.SendFunds(request);

        return result is null ? Ok(Fail("The sendFunds could not be processed.")) : Ok(Success(result));
    }

    [HttpGet("getTransactionByReference")]
    public async Task<IActionResult> GetTransactionByReference([FromQuery] string reference)
    {
        var result = await _coinPayService.GetTransactionByReference(reference);

        return Ok(Success(result));
    }

    #endregion
}