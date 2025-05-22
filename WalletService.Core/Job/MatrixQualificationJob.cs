using WalletService.Core.Services.IServices;

namespace WalletService.Core.Job;

public class MatrixQualificationJob(IMatrixService matrixService)
{
    public async Task ExecuteAsync()
    {
        await matrixService.ProcessAllUsersMatrixQualificationsAsync();
    }
}