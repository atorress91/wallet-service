using WalletService.Models.DTO.InvoiceDetailDto;

namespace WalletService.Core.Services.IServices;

public interface IInvoiceDetailService
{
    Task<IEnumerable<InvoiceDetailDto>> GetAllInvoiceDetailAsync();
}