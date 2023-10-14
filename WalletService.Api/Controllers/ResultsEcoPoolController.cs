using Microsoft.AspNetCore.Mvc;
using WalletService.Core.Services.IServices;

namespace WalletService.Api.Controllers;

[ApiController]
[ApiVersion("1")]
[Route("api/v{version:apiVersion}/ResultsEcoPool")]
public class ResultsEcoPoolController : BaseController
{
    private readonly IResultsEcoPoolService _resultsEcoPoolService;

    public ResultsEcoPoolController(IResultsEcoPoolService resultsEcoPoolService)
    {
        _resultsEcoPoolService = resultsEcoPoolService;
    }

    [HttpGet("GetAllResultsEcoPool")]
    public async Task<IActionResult> GetAllResultsEcoPool()
    {
        var result = await _resultsEcoPoolService.GetAllResultsEcoPoolAsync();
        return Ok(Success(result));
    }

}