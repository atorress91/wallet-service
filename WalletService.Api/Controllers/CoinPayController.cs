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

        return result?.Data is null ? Ok(Fail("The channel could not be created.")) : Ok(Success(result));
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
    
    [HttpGet("getTransactionById")]
    public async Task<IActionResult> GetTransactionById(int idTransaction)
    {
        var result = await _coinPayService.GetTransactionById(idTransaction);

        return result?.Data is null ? Ok(Fail("Transaction not found.")) : Ok(Success(result));
    }
    
    [HttpPost("CoinPayWebhook")]
    public async Task<IActionResult> CoinPayWebhook([FromBody] WebhookNotificationRequest request)
    {
        var result = await _coinPayService.ReceiveCoinPayNotifications(request);
        
        return result is false ? Ok(Fail("The notification could not be processed.")) : Ok();      
    }
    
    
    #endregion
}