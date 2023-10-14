using AutoMapper;
using WalletService.Core.Services.IServices;
using WalletService.Data.Repositories.IRepositories;
using WalletService.Models.DTO.InvoiceDetailDto;

namespace WalletService.Core.Services;

public class InvoiceDetailService:BaseService,IInvoiceDetailService
{
    private readonly IInvoiceDetailRepository _invoiceDetailRepository;
    public InvoiceDetailService(IMapper mapper, IInvoiceDetailRepository invoiceDetailRepository) : base(mapper)
    {
        _invoiceDetailRepository = invoiceDetailRepository;
    }
    
    public async Task<IEnumerable<InvoiceDetailDto>> GetAllInvoiceDetailAsync()
    {
        var response = await _invoiceDetailRepository.GetAllInvoiceDetailAsync();
        return Mapper.Map<IEnumerable<InvoiceDetailDto>>(response);
    }

}