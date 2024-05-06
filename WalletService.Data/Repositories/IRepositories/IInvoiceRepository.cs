using WalletService.Data.Database.CustomModels;
using WalletService.Data.Database.Models;
using WalletService.Models.Requests.WalletRequest;

namespace WalletService.Data.Repositories.IRepositories;

public interface IInvoiceRepository
{
    Task<List<Invoices>>                        GetAllInvoices();
    Task<int>                                   CountDetailsByPaymentGroup(int                          paymentGroupId,int userId);
    Task<List<ModelFourStatistics>>             Model4StatisticsByUser(int                              userId);
    Task<List<Invoices>>                        GetAllInvoicesUser(int                                  id);
    Task<Invoices>                              CreateInvoiceAsync(Invoices                             invoice);
    Task<Invoices?>                             GetInvoiceById(int                                      id);
    Task<Invoices>                              DeleteInvoiceAsync(Invoices                             invoice);
    Task<InvoicesSpResponse?>                   HandleDebitTransaction(DebitTransactionRequest          request);
    Task<Invoices?>                             GetInvoiceByReceiptNumber(string                        receiptNumber);
    Task<List<Invoices>>                        GetInvoicesByReceiptNumber(ICollection<string>          transactionIds);
    Task                                        RevertCoinPaymentTransactions(List<InvoiceNumber>       invoiceNumbers);
    Task<bool>                                  InvoiceExistsByReceiptNumber(string                     idTransaction);
    Task<InvoicesSpResponse?>                   HandleDebitTransactionForCourse(DebitTransactionRequest request);
    Task<bool>                                  GetInvoicesForTradingAcademyPurchases(int               affiliateId);
    Task<List<Invoices>>                        DeleteMultipleInvoicesAndDetailsAsync(int[]             invoiceIds);
    Task<List<InvoicesTradingAcademyResponse>?> GetAllInvoicesForTradingAcademyPurchases();
    Task<List<InvoiceModelOneAndTwoResponse>?>  GetAllInvoicesModelOneAndTwo();
    Task<int> CountDetailsModel3ByPaymentGroup(int userId);
}