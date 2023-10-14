using AutoMapper;
using Microsoft.Extensions.Logging;
using System.Text;
using WalletService.Core.Services.IServices;
using WalletService.Data.Database.CustomModels;
using WalletService.Data.Database.Models;
using WalletService.Data.Repositories.IRepositories;
using WalletService.Models.DTO.InvoiceDto;
using WalletService.Models.Requests.InvoiceRequest;
using WalletService.Utility.Extensions;

namespace WalletService.Core.Services;

public class InvoiceService : BaseService, IInvoiceService
{
    private readonly IInvoiceRepository                _invoiceRepository;
    private readonly ICoinPaymentTransactionRepository _coinPaymentTransactionRepository;
    private readonly ILogger<InvoiceService>           _logger;

    public InvoiceService(IMapper         mapper, IInvoiceRepository invoiceRepository,
        ICoinPaymentTransactionRepository coinPaymentTransactionRepository,
        ILogger<InvoiceService>           logger) : base(mapper)
    {
        _invoiceRepository                = invoiceRepository;
        _coinPaymentTransactionRepository = coinPaymentTransactionRepository;
        _logger                           = logger;
    }

    public async Task<IEnumerable<InvoiceDto>> GetAllInvoiceUserAsync(int id)
    {
        var response   = await _invoiceRepository.GetAllInvoicesUser(id);
        var mappedList = Mapper.Map<IEnumerable<InvoiceDto>>(response).ToList();
        mappedList.Reverse();

        return mappedList;
    }

    public async Task<InvoiceDto> CreateInvoiceAsync(InvoiceRequest request)
    {
        var invoice = Mapper.Map<Invoices>(request);
        invoice = await _invoiceRepository.CreateInvoiceAsync(invoice);
        return Mapper.Map<InvoiceDto>(invoice);
    }

    public async Task<IEnumerable<InvoiceDto>> GetAllInvoices()
    {
        var response   = await _invoiceRepository.GetAllInvoices();
        var mappedList = Mapper.Map<IEnumerable<InvoiceDto>>(response).ToList();
        mappedList.Reverse();
        return mappedList;
    }

    public async Task<bool> RevertUnconfirmedOrUnpaidTransactions()
    {
        var builder             = new StringBuilder();

        builder.AppendLine($"[InvoiceService] | RevertUnconfirmedOrUnpaidTransactions | Started");
        var transactionsResults = await _coinPaymentTransactionRepository.GetAllUnconfirmedOrUnpaidTransactions();
        var idsTransactions     = transactionsResults.Select(e => e.IdTransaction).ToList();

        if (idsTransactions is { Count: 0 })
        {
            _logger.LogWarning(builder.ToString());
            return false;
        }
        
        builder.AppendLine($"[InvoiceService] | RevertUnconfirmedOrUnpaidTransactions | idsTransactions {idsTransactions.ToJsonString()}");
        var invoicesResult   = await _invoiceRepository.GetInvoicesByReceiptNumber(idsTransactions);
        var invoicesToRevert = invoicesResult!.Select(e => new InvoiceNumber { InvoiceNumberValue = e.Id }).ToList();

        if (invoicesToRevert is { Count: 0 })
        {
            _logger.LogWarning(builder.ToString());
            return false;
        }

        builder.AppendLine($"[InvoiceService] | RevertUnconfirmedOrUnpaidTransactions | invoicesToRevert {idsTransactions.ToJsonString()}");
        await _invoiceRepository.RevertCoinPaymentTransactions(invoicesToRevert);

        builder.AppendLine($"[InvoiceService] | RevertUnconfirmedOrUnpaidTransactions | Completed");
        
        _logger.LogInformation(builder.ToString());
        
        return true;
    }
}