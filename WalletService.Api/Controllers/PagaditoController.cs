using Microsoft.AspNetCore.Mvc;
using WalletService.Core.Services.IServices;
using WalletService.Models.Requests.PagaditoRequest;

namespace WalletService.Api.Controllers;

[ApiController]
[ApiVersion("1")]
[Route("api/v{version:apiVersion}/Pagadito")]
public class PagaditoController : BaseController
{
    private readonly IPagaditoService _pagaditoService;
    
    public PagaditoController(IPagaditoService pagaditoService)
    {
        _pagaditoService = pagaditoService;
    }

    [HttpPost("create_transaction")]
    public async Task<IActionResult> CreateTransaction([FromBody] CreatePagaditoTransactionRequest request)
    {
        var result = await _pagaditoService.CreateTransaction(request);
        return result is null ? Ok(Fail("Error")) : Ok(Success(result));
    }
    
    [HttpPost("webhook")]
    public async Task<IActionResult> HandleWebhook()
    {
        var headers = Request.Headers;
        
        Request.EnableBuffering();
        string requestBody;
        using (var reader = new StreamReader(Request.Body, leaveOpen: true))
        {
            requestBody = await reader.ReadToEndAsync();
        }
        Request.Body.Position = 0;
        
        var isSignatureValid = await _pagaditoService.VerifySignature(headers, requestBody);

        if (!isSignatureValid)
        {
            return BadRequest("The request is not valid.");
        }
        
        var request = Newtonsoft.Json.JsonConvert.DeserializeObject<WebHookRequest>(requestBody);

        var isPurchaseProcessed = await _pagaditoService.ProcessPurchase(request);

        return isPurchaseProcessed ? Ok() : BadRequest("The purchase could not be processed.");
    }
}