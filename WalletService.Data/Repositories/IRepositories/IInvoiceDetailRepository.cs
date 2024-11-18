using WalletService.Data.Database.Models;

namespace WalletService.Data.Repositories.IRepositories;

public interface IInvoiceDetailRepository
{
    Task<List<InvoicesDetail>> GetAllInvoiceDetailAsync();
    Task<InvoicesDetail> CreateInvoiceDetailAsync(InvoicesDetail                 invoice);
    Task<List<InvoicesDetail>> CreateBulkInvoiceDetailAsync(List<InvoicesDetail> requests);
    Task<List<InvoicesDetail>> GetInvoiceDetailByInvoiceIdAsync(long               invoiceId);

    Task<List<InvoicesDetail>> DeleteBulkInvoiceDetailAsync(List<InvoicesDetail> requests);

}