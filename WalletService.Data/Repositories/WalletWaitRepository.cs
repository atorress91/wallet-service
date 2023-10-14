using Microsoft.EntityFrameworkCore;
using WalletService.Data.Database;
using WalletService.Data.Database.Models;
using WalletService.Data.Repositories.IRepositories;

namespace WalletService.Data.Repositories;

public class WalletWaitRepository:BaseRepository,IWalletWaitRepository
{
    public WalletWaitRepository(WalletServiceDbContext context) : base(context)
    {
    }
    public  Task<List<WalletsWaits>> GetAllWalletsWaits()
        =>  Context.WalletsWaits.AsNoTracking().ToListAsync();
    
    public  Task<WalletsWaits?> GetWalletWaitById(int id)
        =>  Context.WalletsWaits.FirstOrDefaultAsync(x => x.Id == id);

    public async Task<WalletsWaits> CreateWalletWaitAsync(WalletsWaits request)
    {
        var today = DateTime.Now;
        request.CreatedAt = today;
        request.UpdatedAt = today;
        
        await Context.AddAsync(request);
        await Context.SaveChangesAsync();

        return request;
    } 
    
    public async Task<WalletsWaits> UpdateWalletWaitAsync(WalletsWaits request)
    {
        var today = DateTime.Now;
        request.UpdatedAt = today;
        Context.WalletsWaits.Update(request);
        await Context.SaveChangesAsync();

        return request;
    }
    
    public async Task<WalletsWaits> DeleteWalletWaitAsync(WalletsWaits request)
    {
        request.DeletedAt = DateTime.Now;

        Context.WalletsWaits.Update(request);
        await Context.SaveChangesAsync();

        return request;
    }
}