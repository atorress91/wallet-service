using Microsoft.AspNetCore.Mvc;
using WalletService.Core.Services.IServices;
using WalletService.Models.Requests.PaymentTransaction;

namespace WalletService.Api.Controllers;

[ApiController]
[ApiVersion("1")]
[Route("api/v{version:apiVersion}/PaymentTransaction")]
public class PaymentTransactionController : BaseController
{
    private readonly IPaymentTransactionService _paymentTransactionService;
    
    public PaymentTransactionController(IPaymentTransactionService paymentTransactionService)
    {
        _paymentTransactionService = paymentTransactionService;
    }

    #region Transaction

    [HttpPost]
    public async Task<IActionResult> CreatePaymentTransactionAsync([FromBody] PaymentTransactionRequest request)
    {
        var result = await _paymentTransactionService.CreatePaymentTransactionAsync(request);
        return result is null ? Ok(Fail("The transaction wasn't created")) : Ok(Success(result));
    }
    
    [HttpGet("getAllWireTransfer")]
    public async Task<IActionResult> GetAllWireTransfer()
    {
        var result = await _paymentTransactionService.GetAllWireTransfer();
        return Ok(Success(result));
    }
    
    [HttpPost("confirmPayment")]
    public async Task<IActionResult> ConfirmPayment([FromBody] ConfirmPaymentTransactionRequest request)
    {
        var result = await _paymentTransactionService.ConfirmPayment(request);
        return result ? Ok(Success("The transaction was confirmed")) : Ok(Fail("The transaction wasn't confirmed"));
    }

    #endregion
    
}