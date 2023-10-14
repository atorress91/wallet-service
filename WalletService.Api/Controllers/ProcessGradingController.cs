using Microsoft.AspNetCore.Mvc;
using WalletService.Core.Services.IServices;
using WalletService.Models.Requests.EcoPoolConfigurationRequest;

namespace WalletService.Api.Controllers;

[ApiController]
[ApiVersion("1")]
[Route("api/v{version:apiVersion}/[controller]")]
public class ProcessGradingController : BaseController
{

    private readonly IProcessGradingService       _processGradingService;
    private readonly IEcoPoolConfigurationService _configurationService;

    public ProcessGradingController(
        IProcessGradingService       processGradingService,
        IEcoPoolConfigurationService configurationService)
    {
        _processGradingService = processGradingService;
        _configurationService  = configurationService;
    }

    [HttpPost("eco_pool_process")]
    public async Task<IActionResult> EcoPoolProcess()
    {
        await _processGradingService.EcoPoolProcess();

        return Ok(Success("ok"));
    }
    
    [HttpPost("payment")]
    public async Task<IActionResult> PaymentProcess()
    {
        await _processGradingService.PaymentProcess();

        return Ok(Success("ok"));
    }

    [HttpPost("eco_pool_configuration")]
    public async Task<IActionResult> EcoPoolConfiguration([FromBody] EcoPoolConfigurationRequest request)
    {
        await _configurationService.CreateOrUpdateEcoPoolConfiguration(request);

        return Ok(Success("ok"));
    }

    [HttpGet]
    public async Task<IActionResult> GetEcoPoolConfiguration()
    {
        var configuration = await _configurationService.GetEcoPoolDefaultConfiguration();

        if (configuration is null)
            return NotFound();

        return Ok(configuration);
    }

    [HttpGet("GetProgressPercentage/{configurationId:int}")]
    public async Task<IActionResult> GetProgressPercentage([FromRoute] int configurationId)
    {
        var result = await _configurationService.GetProgressPercentage(configurationId);

        return Ok(Success(result));
    }
}