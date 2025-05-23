using Microsoft.Extensions.Logging;
using WalletService.Core.Services.IServices;

namespace WalletService.Core.Job;

public class MatrixQualificationJob
{
    private readonly IMatrixService _matrixService;
    private readonly ILogger<MatrixQualificationJob> _logger;

    public MatrixQualificationJob(IMatrixService matrixService, ILogger<MatrixQualificationJob> logger)
    {
        _matrixService = matrixService;
        _logger = logger;
    }

    public async Task ExecuteAsync()
    {
        _logger.LogInformation("Iniciando tarea MatrixQualificationJob...");
        try
        {
            await _matrixService.ProcessAllUsersMatrixQualificationsAsync();
            _logger.LogInformation("Tarea MatrixQualificationJob completada exitosamente.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al ejecutar MatrixQualificationJob.");
            throw; 
        }
    }
}