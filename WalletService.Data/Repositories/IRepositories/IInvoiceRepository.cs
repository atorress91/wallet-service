using WalletService.Data.Database.CustomModels;
using WalletService.Data.Database.Models;
using WalletService.Models.Requests.WalletRequest;

namespace WalletService.Data.Repositories.IRepositories;

public interface IInvoiceRepository
{
    Task<List<Invoice>> GetAllInvoices(long brandId, DateTime? startDate = null, DateTime? endDate = null);
    Task<int>                                   CountDetailsByPaymentGroup(int                          paymentGroupId,int userId,long brandId);
    Task<List<ModelFourStatistic>>             Model4StatisticsByUser(int                              userId);
    Task<List<Invoice>>                        GetAllInvoicesUser(int                                  id, long brandId);
    Task<Invoice>                              CreateInvoiceAsync(Invoice                             invoice);
    Task<Invoice?>                             GetInvoiceById(long                                      id, long brandId);
    Task<Invoice>                              DeleteInvoiceAsync(Invoice                             invoice);
    Task<InvoicesSpResponse?>                   HandleDebitTransaction(DebitTransactionRequest          request);
    Task<Invoice?>                             GetInvoiceByReceiptNumber(string                        receiptNumber, long brandId);
    Task<List<Invoice>>                        GetInvoicesByReceiptNumber(ICollection<string>          transactionIds);
    Task                                        RevertCoinPaymentTransactions(List<InvoiceNumber>       invoiceNumbers);
    Task<bool>                                  InvoiceExistsByReceiptNumber(string                     idTransaction, long brandId);
    Task<bool>                                  GetInvoicesForTradingAcademyPurchases(int               affiliateId);
    Task<List<Invoice>>                        DeleteMultipleInvoicesAndDetailsAsync(long[]             invoiceIds, long brandId);
    Task<List<InvoicesTradingAcademyResponse>?> GetAllInvoicesForTradingAcademyPurchases();
    Task<List<InvoiceModelOneAndTwoResponse>?>  GetAllInvoicesModelOneAndTwo();
    Task<int> CountDetailsModel3ByPaymentGroup(int userId, long brandId);
    Task<decimal>  GetTotalAdquisitionsAdmin(long brandId, int paymentGroupId);
}