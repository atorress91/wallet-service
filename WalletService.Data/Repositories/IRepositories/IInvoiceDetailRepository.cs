using WalletService.Data.Database.Models;

namespace WalletService.Data.Repositories.IRepositories;

public interface IInvoiceDetailRepository
{
    Task<List<InvoicesDetails>> GetAllInvoiceDetailAsync();
    Task<InvoicesDetails> CreateInvoiceDetailAsync(InvoicesDetails                 invoice);
    Task<List<InvoicesDetails>> CreateBulkInvoiceDetailAsync(List<InvoicesDetails> requests);
    Task<List<InvoicesDetails>> GetInvoiceDetailByInvoiceIdAsync(int               invoiceId);

    Task<List<InvoicesDetails>> DeleteBulkInvoiceDetailAsync(List<InvoicesDetails> requests);

}