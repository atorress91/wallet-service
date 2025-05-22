using Microsoft.AspNetCore.Mvc;
using WalletService.Core.Services.IServices;
using WalletService.Models.Requests.MatrixQualification;
using WalletService.Models.Requests.MatrixRequest;

namespace WalletService.Api.Controllers;

[ApiController]
[ApiVersion("1")]
[Route("api/v{version:apiVersion}/[controller]")]
public class MatrixQualificationController : BaseController
{
    private readonly IMatrixQualificationService _matrixQualificationService;
    private readonly IMatrixService _matrixService;

    public MatrixQualificationController(IMatrixQualificationService matrixQualificationService,
        IMatrixService matrixService)
    {
        _matrixQualificationService = matrixQualificationService;
        _matrixService = matrixService;
    }

    [HttpPut]
    public async Task<IActionResult> UpdateQualification([FromBody] MatrixQualificationRequest request)
    {
        var result = await _matrixQualificationService.UpdateAsync(request);
        return result is null ? Ok(Fail("The qualification wasn't updated")) : Ok(Success(result));
    }

    [HttpGet("get_by_user_and_matrix_type")]
    public async Task<IActionResult> GetByUserAndMatrixType([FromQuery] MatrixRequest request)
    {
        var result = await _matrixQualificationService.GetByUserAndMatrixTypeAsync(request.UserId, request.MatrixType);
        return result is null ? Ok(Fail("Error, The position not found.")) : Ok(Success(result));
    }

    [HttpGet("process_qualification")]
    public async Task<IActionResult> ProcessQualification([FromQuery] int userId)
    {
        var result = await _matrixService.ProcessAllMatrixQualificationsAsync(userId);
        return result.anyQualified == false ? Ok(Fail("Error, The position not found.")) : Ok(Success(result));
    }

    [HttpPost("process_qualification_admin")]
    public async Task<IActionResult> ProcessQualificationAdmin([FromBody] MatrixRequest request)
    {
        var result = await _matrixService.ProcessAdminMatrixPlacementAsync(request.UserId, request.MatrixType);
        return result is false ? Ok(Fail("Error, The user is active in the matrix.")) : Ok(Success(result));
    }

    [HttpPost("process_direct_payment_matrix_activation_async")]
    public async Task<IActionResult> ProcessDirectPaymentMatrixActivationAsync([FromBody] MatrixRequest request)
    {
        var result = await _matrixService.ProcessDirectPaymentMatrixActivationAsync(request);
        return result is false ? Ok(Fail("Error, The position not found.")) : Ok(Success(result));
    }
    
    [HttpPost("check_qualification")]
    public async Task<IActionResult> CheckQualification([FromBody] MatrixRequest request)
    {
        var result = await _matrixService.CheckQualificationAsync(request.UserId, request.MatrixType);
        return result is false ? Ok(Fail("Error, The position not found.")) : Ok(Success(result));
    }
    
    [HttpPost("all_inconsistent_records")]
    public async Task<IActionResult> FixInconsistentQualificationRecordsAsync()
    {
        await _matrixService.FixInconsistentQualificationRecordsAsync();
        return Ok(Success("Operation completed successfully.")); 
    }
    
    [HttpPost("process_all_qualifications")]
    public async Task<IActionResult> ProcessAllQualifications([FromBody] int[]? userIds = null)
    {
        var result = await _matrixService.ProcessAllUsersMatrixQualificationsAsync(userIds);
        return Ok(Success(result));
    }
}