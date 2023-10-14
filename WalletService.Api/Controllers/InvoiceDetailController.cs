using Microsoft.AspNetCore.Mvc;
using WalletService.Core.Services.IServices;

namespace WalletService.Api.Controllers;

[ApiController]
[ApiVersion("1")]
[Route("api/v{version:apiVersion}/InvoiceDetail")]
public class InvoiceDetailController : BaseController
{
    private readonly IInvoiceDetailService _invoiceDetailService;

    public InvoiceDetailController(IInvoiceDetailService invoiceService)
    {
        _invoiceDetailService = invoiceService;
    }

    [HttpGet("GetAllInvoicesDetails")]
    public async Task<IActionResult> GetAllInvoiceDetailAsync()
    {
        var result = await _invoiceDetailService.GetAllInvoiceDetailAsync();
        return Ok(result);
    }
}