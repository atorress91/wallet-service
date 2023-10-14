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

    public CoinPayController(ICoinPayService coinPayService)
    {
        _coinPayService = coinPayService;
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

        return result?.Data is null ? Ok(Fail("The channel could not be created.")) : Ok(result);
    }

    [HttpGet("getNetworksByIdCurrency")]
    public async Task<IActionResult> GetNetworkByIdCurrency(int idCurrency)
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
    
    [HttpPost("webhookNotification")]
    public async Task<IActionResult> WebhookNotification([FromBody] TransactionNotificationRequest transaction)
    {
        var signature = Request.Headers["x-signature"].ToString();
        var dynamicKey = Request.Headers["x-dynamic-key"].ToString();

        if (!await _coinPayService.IsValidSignature(transaction.IdUser, transaction.IdTransaction, dynamicKey, signature))
        {
            return BadRequest("Invalid signature.");
        }

        await _coinPayService.ProcessTransactionAsync(transaction);

        return Ok();
    }
    
    #endregion
}