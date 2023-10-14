using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
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
}