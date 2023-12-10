using WalletService.Data.Database.CustomModels;
using WalletService.Data.Database.Models;
using WalletService.Models.Requests.WalletRequest;

namespace WalletService.Data.Repositories.IRepositories;

public interface IInvoiceRepository
{
    Task<List<Invoices>>      GetAllInvoicesUser(int      id);
    Task<Invoices>            CreateInvoiceAsync(Invoices invoice);
    Task<List<Invoices>>      GetAllInvoices();
    Task<Invoices?>           GetInvoiceById(int                                      id);
    Task<Invoices>            DeleteInvoiceAsync(Invoices                             invoice);
    Task<InvoicesSpResponse?> HandleDebitTransaction(DebitTransactionRequest          request);
    Task<Invoices?>           GetInvoiceByReceiptNumber(string                        idTransaction);
    Task<List<Invoices>>      GetInvoicesByReceiptNumber(ICollection<string>          transactionIds);
    Task                      RevertCoinPaymentTransactions(List<InvoiceNumber>       invoiceNumbers);
    Task<bool>                InvoiceExistsByReceiptNumber(string                     idTransaction);
    Task<InvoicesSpResponse?> HandleDebitTransactionForCourse(DebitTransactionRequest request);
    Task<bool>                GetInvoicesForTradingAcademyPurchases(int               affiliateId);
}