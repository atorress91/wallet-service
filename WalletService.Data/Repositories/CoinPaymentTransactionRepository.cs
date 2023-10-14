using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using WalletService.Data.Database;
using WalletService.Data.Database.Models;
using WalletService.Data.Repositories.IRepositories;
using WalletService.Models.Configuration;

namespace WalletService.Data.Repositories;

public class CoinPaymentTransactionRepository : BaseRepository, ICoinPaymentTransactionRepository
{
    private readonly ApplicationConfiguration _appSettings;

    public CoinPaymentTransactionRepository(IOptions<ApplicationConfiguration> appSettings, WalletServiceDbContext context) : base(context)
        => _appSettings = appSettings.Value;


    public Task<List<PaymentTransaction>> GetAllCoinPaymentTransaction()
        => Context.PaymentTransaction.AsNoTracking().ToListAsync();

    public Task<PaymentTransaction?> GetCoinPaymentTransactionByIdTransaction(string idTransaction)
        => Context.PaymentTransaction.FirstOrDefaultAsync(e => e.IdTransaction == idTransaction);

    public async Task<PaymentTransaction> CreateCoinPaymentTransaction(PaymentTransaction request)
    {
        var today = DateTime.Now;
        request.CreatedAt = today;
        request.UpdatedAt = today;

        await Context.AddAsync(request);
        await Context.SaveChangesAsync();

        return request;
    }

    public async Task<PaymentTransaction> UpdateCoinPaymentTransactionAsync(PaymentTransaction request)
    {
        var today = DateTime.Now;
        request.UpdatedAt = today;
        Context.PaymentTransaction.Update(request);
        await Context.SaveChangesAsync();

        return request;
    }

    public Task<List<PaymentTransaction>> GetAllUnconfirmedOrUnpaidTransactions()
    {
        return Context.PaymentTransaction
            .Where(e => e.Status != 100 && DateTime.Now > e.CreatedAt.AddHours(3)).AsNoTracking()
            .ToListAsync();
    }
}