using WalletService.Data.Database.CustomModels;
using WalletService.Models.DTO.InvoiceDto;
using WalletService.Models.Requests.InvoiceRequest;
using WalletService.Models.Requests.WalletRequest;
using WalletService.Models.Responses;

namespace WalletService.Core.Services.IServices;

public interface IInvoiceService
{
    Task<IEnumerable<InvoiceDto>>                GetAllInvoiceUserAsync(int        id);
    Task<InvoiceDto>                             CreateInvoiceAsync(InvoiceRequest request);
    Task<IEnumerable<InvoiceDto>>                GetAllInvoices(DateTime? startDate = null, DateTime? endDate = null);
    Task<bool>                                   RevertUnconfirmedOrUnpaidTransactions();
    Task<IEnumerable<InvoiceTradingAcademyDto?>> GetAllInvoicesForTradingAcademyPurchases();
    Task<IEnumerable<UserAffiliateResponse>>     SendInvitationsForUpcomingCourses(string link, string code);
    Task<IEnumerable<InvoiceModelOneAndTwoDto>>  GetAllInvoicesModelOneAndTwo();

    Task<ModelBalancesAndInvoicesDto?> ProcessAndReturnBalancesForModels1A1B2(
        ModelBalancesAndInvoicesRequest request);

    Task<byte[]> CreateInvoice(int invoiceId);
    Task<InvoiceResultDto?> CreateInvoiceByReference(string reference);
    Task<InvoicesSpResponse?> HandleDebitTransaction(DebitTransactionRequest request);
}