using Microsoft.AspNetCore.Mvc;
using WalletService.Core.Services.IServices;
using WalletService.Models.Requests.MatrixEarningRequest;

namespace WalletService.Api.Controllers;

[ApiController]
[ApiVersion("1")]
[Route("api/v{version:apiVersion}/[controller]")]
public class MatrixEarningsController : BaseController
{
    private readonly IMatrixEarningsService _matrixEarningsService;

    public MatrixEarningsController(IMatrixEarningsService matrixEarningsService)
    {
        _matrixEarningsService = matrixEarningsService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateAsync([FromBody] MatrixEarningRequest request)
    {
        var result = await _matrixEarningsService.CreateAsync(request);
        return Ok(result);
    }
}