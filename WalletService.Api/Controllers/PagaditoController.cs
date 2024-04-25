using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using WalletService.Core.Services.IServices;
using WalletService.Models.Requests.ConPaymentRequest;
using WalletService.Models.Requests.PagaditoRequest;
using WalletService.Models.Requests.WalletRequest;

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

        return isSignatureValid ? Ok() : BadRequest("The request is not valid or the purchase could not be processed.");
    }
}