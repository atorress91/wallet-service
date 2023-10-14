using Microsoft.EntityFrameworkCore;
using WalletService.Data.Database;
using WalletService.Data.Database.Models;
using WalletService.Data.Repositories.IRepositories;

namespace WalletService.Data.Repositories;

public class InvoiceDetailRepository : BaseRepository, IInvoiceDetailRepository
{
    public InvoiceDetailRepository(WalletServiceDbContext context) : base(context) { }
    public Task<List<InvoicesDetails>> GetAllInvoiceDetailAsync()
        => Context.InvoicesDetails.Where(x => x.DeletedAt == null).AsNoTracking().ToListAsync();

    public Task<List<InvoicesDetails>> GetInvoiceDetailByInvoiceIdAsync(int invoiceId)
        => Context.InvoicesDetails.Where(x => x.InvoiceId == invoiceId).ToListAsync();

    public async Task<InvoicesDetails> CreateInvoiceDetailAsync(InvoicesDetails invoice)
    {
        var today = DateTime.Now;
        invoice.CreatedAt = today;
        invoice.UpdatedAt = today;

        await Context.AddAsync(invoice);
        await Context.SaveChangesAsync();

        return invoice;
    }

    public async Task<List<InvoicesDetails>> CreateBulkInvoiceDetailAsync(List<InvoicesDetails> requests)
    {
        const int take         = 1000;
        var       packageCount = requests.Count / take;

        for (var i = 0; i < packageCount; i++)
        {
            var packageList = requests.Skip(i * take).Take(take).ToList();
            Context.InvoicesDetails.UpdateRange(packageList);
        }

        await Context.SaveChangesAsync();

        return requests;
    }
    public async Task<List<InvoicesDetails>> DeleteBulkInvoiceDetailAsync(List<InvoicesDetails> requests)
    {
        var today = DateTime.Now;
        foreach (var item in requests)
        {
            item.UpdatedAt = today;
            item.DeletedAt = today;
        }

        const int take         = 1000;
        var       packageCount = requests.Count / take;

        for (var i = 0; i < packageCount; i++)
        {
            var packageList = requests.Skip(i * take).Take(take).ToList();

            Context.InvoicesDetails.UpdateRange(packageList);
        }

        await Context.SaveChangesAsync();

        return requests;
    }

}