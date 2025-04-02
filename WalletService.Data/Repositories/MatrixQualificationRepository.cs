using Microsoft.EntityFrameworkCore;
using WalletService.Data.Database;
using WalletService.Data.Database.Models;
using WalletService.Data.Repositories.IRepositories;

namespace WalletService.Data.Repositories;

public class MatrixQualificationRepository(WalletServiceDbContext context) : BaseRepository(context), IMatrixQualificationRepository
{
    public async Task<MatrixQualification?> GetByUserAndMatrixTypeAsync(long userId, int matrixType)
        => await Context.MatrixQualifications.FirstOrDefaultAsync(e =>
            e.UserId == userId && e.MatrixType == matrixType);

    public async Task<MatrixQualification?> CreateAsync(MatrixQualification qualification)
    {
        var today = DateTime.Now;
        qualification.CreatedAt = today;
        qualification.UpdatedAt = today;

        await Context.MatrixQualifications.AddAsync(qualification);
        await Context.SaveChangesAsync();

        return qualification;
    }

    public async Task<MatrixQualification?> UpdateAsync(MatrixQualification qualification)
    {
        var today = DateTime.Now;
        qualification.UpdatedAt = today;

        Context.MatrixQualifications.Update(qualification);
        await Context.SaveChangesAsync();

        return qualification;
    }
}