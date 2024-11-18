﻿using Microsoft.EntityFrameworkCore;
using WalletService.Data.Database;
using WalletService.Data.Database.Models;
using WalletService.Data.Repositories.IRepositories;

namespace WalletService.Data.Repositories;

public class WalletWaitRepository:BaseRepository,IWalletWaitRepository
{
    public WalletWaitRepository(WalletServiceDbContext context) : base(context)
    {
    }
    public  Task<List<WalletsWait>> GetAllWalletsWaits()
        =>  Context.WalletsWaits.AsNoTracking().ToListAsync();
    
    public  Task<WalletsWait?> GetWalletWaitById(int id)
        =>  Context.WalletsWaits.FirstOrDefaultAsync(x => x.Id == id);

    public async Task<WalletsWait> CreateWalletWaitAsync(WalletsWait request)
    {
        var today = DateTime.Now;
        request.CreatedAt = today;
        request.UpdatedAt = today;
        
        await Context.AddAsync(request);
        await Context.SaveChangesAsync();

        return request;
    } 
    
    public async Task<WalletsWait> UpdateWalletWaitAsync(WalletsWait request)
    {
        var today = DateTime.Now;
        request.UpdatedAt = today;
        Context.WalletsWaits.Update(request);
        await Context.SaveChangesAsync();

        return request;
    }
    
    public async Task<WalletsWait> DeleteWalletWaitAsync(WalletsWait request)
    {
        request.DeletedAt = DateTime.Now;

        Context.WalletsWaits.Update(request);
        await Context.SaveChangesAsync();

        return request;
    }
}