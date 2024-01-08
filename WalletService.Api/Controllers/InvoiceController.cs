using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using WalletService.Core.Services.IServices;

namespace WalletService.Api.Controllers;

[ApiController]
[ApiVersion("1")]
[Route("api/v{version:apiVersion}/Invoice")]
public class InvoiceController : BaseController
{
    private readonly IInvoiceService _invoiceService;

    public InvoiceController(IInvoiceService invoiceService)
    {
        _invoiceService = invoiceService;
    }

    [HttpGet("GetAllInvoicesByUserId")]
    public async Task<IActionResult> GetAllInvoicesByUserId([FromQuery] int id)
    {
        var result = await _invoiceService.GetAllInvoiceUserAsync(id);
        return Ok(result);
    }

    [HttpGet("GetAllInvoices")]
    public async Task<IActionResult> GetAllInvoices()
    {
        var result = await _invoiceService.GetAllInvoices();

        return Ok(result);
    }

    [HttpPost("RevertCoinPaymentTransactions")]
    public async Task<IActionResult> RevertCoinPaymentTransactions()
    {
        var result = await _invoiceService.RevertUnconfirmedOrUnpaidTransactions();

        return Ok(result);
    }

    [HttpGet("GetAllInvoicesForTradingAcademyPurchases")]
    public async Task<IActionResult> GetAllInvoicesForTradingAcademyPurchases()
    {
        var result = await _invoiceService.GetAllInvoicesForTradingAcademyPurchases();

        return result.IsNullOrEmpty() ? Ok(Fail("The invoice wasn't found")) : Ok(Success(result));
    }
    
    [HttpPost("SendInvitationsForUpcomingCourses")]
    public async Task<IActionResult> SendInvitationsForUpcomingCourses([FromQuery] string link, [FromQuery] string code)
    {
        var result = await _invoiceService.SendInvitationsForUpcomingCourses(link, code);

        return result.IsNullOrEmpty() ? Ok(Fail("The invoice wasn't found")) : Ok(Success(result));
    }
    
    [HttpGet("GetAllInvoicesForModelOneAndTwo")]
    public async Task<IActionResult>GetAllInvoicesForModelOneAndTwo()
    {
        var result = await _invoiceService.GetAllInvoicesModelOneAndTwo();

        return result.IsNullOrEmpty() ? Ok(Fail("The invoices wasn't found")) : Ok(Success(result));
    }
}