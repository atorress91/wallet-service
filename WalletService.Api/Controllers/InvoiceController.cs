using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using WalletService.Core.Services.IServices;
using WalletService.Models.Requests.InvoiceRequest;
using WalletService.Models.Requests.WalletRequest;

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
    
    [HttpPost("ProcessAndReturnBalancesForModels1A1B2")]
    public async Task<IActionResult> ProcessAndReturnBalancesForModels1A1B2([FromBody] ModelBalancesAndInvoicesRequest request)
    {
        var result = await _invoiceService.ProcessAndReturnBalancesForModels1A1B2(request);

        return result is null ? Ok(Fail("Invoice returns could not be processed.")) : Ok(Success(result));
    }
    
    [HttpGet("create_invoice")]
    public async Task<IActionResult> CreateInvoice([FromQuery] int invoiceId)
    {
        var result = await _invoiceService.CreateInvoice(invoiceId);
        if (result.Length == 0)
        {
            return NotFound("The requested invoice could not be generated or does not exist.");
        }
        
        Response.ContentType = "application/pdf";
        Response.Headers.ContentDisposition = $"attachment; filename=invoice_{invoiceId}.pdf";

        return File(result, "application/pdf");
    }
    
    [HttpGet("create_invoice_by_reference")]
    public async Task<IActionResult> CreateInvoiceByReference([FromQuery] string reference)
    {
        var result = await _invoiceService.CreateInvoiceByReference(reference);
        if (result == null)
            return NotFound("The requested invoice could not be generated or does not exist.");
        
        Response.ContentType = "application/pdf";
        Response.Headers.ContentDisposition = $"attachment; filename=invoice_{reference}.pdf";
        Response.Headers.Add("X-Brand-Id", result.BrandId.ToString());

        return File(result.PdfContent ?? throw new InvalidOperationException(), "application/pdf");
    }
    
    [HttpPost("HandleDebitTransaction")]
    public async Task<IActionResult> HandleDebitTransaction([FromBody] DebitTransactionRequest debitRequest)
    {
        try 
        {
            var result = await _invoiceService.HandleDebitTransaction(debitRequest);
        
            if (result == null)
                return BadRequest("La transacción no pudo ser procesada");
            
            return Ok(Success(result));
        }
        catch (Exception ex)
        {
            return BadRequest($"Error al procesar la transacción: {ex.Message}");
        }
    }
}