using WalletService.Models.DTO.InvoiceDto;
using WalletService.Models.Requests.InvoiceRequest;
using WalletService.Models.Responses;

namespace WalletService.Core.Services.IServices;

public interface IInvoiceService
{
    Task<IEnumerable<InvoiceDto>>                GetAllInvoiceUserAsync(int        id);
    Task<InvoiceDto>                             CreateInvoiceAsync(InvoiceRequest request);
    Task<IEnumerable<InvoiceDto>>                GetAllInvoices();
    Task<bool>                                   RevertUnconfirmedOrUnpaidTransactions();
    Task<IEnumerable<InvoiceTradingAcademyDto?>> GetAllInvoicesForTradingAcademyPurchases();
    Task<IEnumerable<UserAffiliateResponse>>     SendInvitationsForUpcomingCourses(string link, string code);
    Task<IEnumerable<InvoiceModelOneAndTwoDto>>  GetAllInvoicesModelOneAndTwo();
}