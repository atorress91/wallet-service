using Microsoft.AspNetCore.Mvc;

namespace WalletService.Api.Controllers;


[ApiController]
[ApiVersion("1")]
[Route("api/v{version:apiVersion}/test")]
public class TestController : BaseController
{
    [HttpGet]
    public IActionResult TestAction()
    {
        return Ok();
    }
}