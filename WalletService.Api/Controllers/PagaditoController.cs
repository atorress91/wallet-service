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

    [HttpPost("createTransaction")]
    public async Task<IActionResult> CreateTransaction([FromBody] TransactionRequest request)
    {
        var result = await _pagaditoService.CreateTransaction(request);
        return result is null ? Ok(Fail("Error")) : Ok(result);
    }
}