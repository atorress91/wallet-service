using Microsoft.Extensions.Options;
using WalletService.Data.Database;
using WalletService.Data.Database.Models;
using WalletService.Data.Repositories.IRepositories;
using WalletService.Models.Configuration;

namespace WalletService.Data.Repositories;

public class CreditRepository(IOptions<ApplicationConfiguration> appSettings, WalletServiceDbContext context) : BaseRepository(context), ICreditRepository
{
    
    public async Task<Credit> CreateCredit(Credit credit)
    {
        var today = DateTime.Now;
        credit.CreatedAt = today;
        credit.UpdatedAt = today;

        await Context.Credits.AddAsync(credit);

        return credit;
    }
}