using Microsoft.EntityFrameworkCore;
using WalletService.Data.Database;
using WalletService.Data.Database.Models;
using WalletService.Data.Repositories.IRepositories;

namespace WalletService.Data.Repositories;

public class WalletWithDrawalRepository:BaseRepository,IWalletWithDrawalRepository
{
    public WalletWithDrawalRepository(WalletServiceDbContext context) : base(context)
    {
    }
    public Task<List<WalletsWithdrawals>> GetAllWalletsWithdrawals()
        =>  Context.WalletsWithdrawals.AsNoTracking().ToListAsync();
    
    public  Task<WalletsWithdrawals?> GetWalletWithdrawalById(int id)
        =>  Context.WalletsWithdrawals.FirstOrDefaultAsync(x => x.Id == id);

    public async Task<WalletsWithdrawals> CreateWalletWithdrawalAsync(WalletsWithdrawals request)
    {
        var today = DateTime.Now;
        request.CreatedAt = today;
        request.UpdatedAt = today;
        
        await Context.AddAsync(request);
        await Context.SaveChangesAsync();

        return request;
    } 
    public async Task<WalletsWithdrawals> UpdateWalletWithdrawalAsync(WalletsWithdrawals request)
    {
        var today = DateTime.Now;
        request.UpdatedAt = today;
        Context.WalletsWithdrawals.Update(request);
        await Context.SaveChangesAsync();

        return request;
    }
    public async Task<WalletsWithdrawals> DeleteWalletWithdrawalAsync(WalletsWithdrawals request)
    {
        request.DeletedAt = DateTime.Now;

        Context.WalletsWithdrawals.Update(request);
        await Context.SaveChangesAsync();

        return request;
    }
}