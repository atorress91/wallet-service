using WalletService.Models.DTO.InvoiceDto;
using WalletService.Models.Requests.InvoiceRequest;

namespace WalletService.Core.Services.IServices;

public interface IInvoiceService
{
    Task<IEnumerable<InvoiceDto>> GetAllInvoiceUserAsync(int id);
    Task<InvoiceDto> CreateInvoiceAsync(InvoiceRequest       request);
    Task<IEnumerable<InvoiceDto>> GetAllInvoices();
    Task<bool> RevertUnconfirmedOrUnpaidTransactions();
}