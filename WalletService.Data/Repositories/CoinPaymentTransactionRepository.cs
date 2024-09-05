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

    public CoinPaymentTransactionRepository(IOptions<ApplicationConfiguration> appSettings,
        WalletServiceDbContext context) : base(context)
        => _appSettings = appSettings.Value;

    public Task<List<PaymentTransaction>> GetAllCoinPaymentTransaction(int brandId)
        => Context.PaymentTransaction.Where(x => x.BrandId == brandId).AsNoTracking().ToListAsync();

    public Task<PaymentTransaction?> GetCoinPaymentTransactionByIdTransaction(string idTransaction, int brandId)
        => Context.PaymentTransaction.FirstOrDefaultAsync(e =>
            e.IdTransaction == idTransaction && e.BrandId == brandId);

    public Task<PaymentTransaction?> GetTransactionByTxnId(string idTransaction)
        => Context.PaymentTransaction.FirstOrDefaultAsync(e =>
            e.IdTransaction == idTransaction);
    public async Task<PaymentTransaction?> CreateCoinPaymentTransaction(PaymentTransaction request)
    {
        var today = DateTime.Now;
        request.CreatedAt = today;
        request.UpdatedAt = today;

        await Context.AddAsync(request);
        await Context.SaveChangesAsync();

        return request;
    }

    public async Task<int> GetLastTransactionId(int brandId)
    {
        var lastTransaction = await Context.PaymentTransaction
            .Where(x => x.PaymentMethod == "CoinPay" && x.BrandId == brandId)
            .OrderByDescending(t => t.Id).FirstOrDefaultAsync();
        return lastTransaction != null ? int.Parse(lastTransaction.IdTransaction) : 0;
    }

    public async Task<PaymentTransaction?> UpdateCoinPaymentTransactionAsync(PaymentTransaction request)
    {
        var today = DateTime.Now;
        request.UpdatedAt = today;
        Context.PaymentTransaction.Update(request);
        await Context.SaveChangesAsync();

        return request;
    }

    public Task<List<PaymentTransaction>> GetAllUnconfirmedOrUnpaidTransactions(int brandId)
    {
        return Context.PaymentTransaction
            .Where(e => e.Status != 100 && DateTime.Now > e.CreatedAt.AddHours(3) && e.BrandId == brandId)
            .AsNoTracking()
            .ToListAsync();
    }

    public Task<List<PaymentTransaction>> GetAllWireTransfer(int brandId)
        => Context.PaymentTransaction.Where(x => x.PaymentMethod == "wire_transfer" && x.BrandId == brandId)
            .AsNoTracking().ToListAsync();

    public Task<PaymentTransaction?> GetPaymentTransactionById(int id, int brandId)
        => Context.PaymentTransaction.FirstOrDefaultAsync(x => x.Id == id && x.BrandId == brandId);

    public Task<PaymentTransaction?> GetTransactionByReference(string reference)
        => Context.PaymentTransaction.FirstOrDefaultAsync(e => e.Reference == reference);
}