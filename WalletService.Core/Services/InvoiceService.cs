using AutoMapper;
using Microsoft.Extensions.Logging;
using System.Text;
using WalletService.Core.Services.IServices;
using WalletService.Data.Adapters.IAdapters;
using WalletService.Data.Database.CustomModels;
using WalletService.Data.Database.Models;
using WalletService.Data.Repositories.IRepositories;
using WalletService.Models.Constants;
using WalletService.Models.DTO.InvoiceDto;
using WalletService.Models.Requests.InvoiceRequest;
using WalletService.Models.Responses;
using WalletService.Utility.Extensions;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace WalletService.Core.Services;

public class InvoiceService : BaseService, IInvoiceService
{
    private readonly IInvoiceRepository                _invoiceRepository;
    private readonly ICoinPaymentTransactionRepository _coinPaymentTransactionRepository;
    private readonly ILogger<InvoiceService>           _logger;
    private readonly IAccountServiceAdapter            _accountServiceAdapter;
    private readonly IBrevoEmailService                _brevoEmailService;

    public InvoiceService(IMapper         mapper, IInvoiceRepository invoiceRepository,
        ICoinPaymentTransactionRepository coinPaymentTransactionRepository,
        ILogger<InvoiceService>           logger, IAccountServiceAdapter accountServiceAdapter,
        IBrevoEmailService                brevoEmailService) : base(mapper)
    {
        _invoiceRepository                = invoiceRepository;
        _coinPaymentTransactionRepository = coinPaymentTransactionRepository;
        _logger                           = logger;
        _accountServiceAdapter            = accountServiceAdapter;
        _brevoEmailService                = brevoEmailService;
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
        var builder = new StringBuilder();

        builder.AppendLine($"[InvoiceService] | RevertUnconfirmedOrUnpaidTransactions | Started");
        var transactionsResults = await _coinPaymentTransactionRepository.GetAllUnconfirmedOrUnpaidTransactions();
        var idsTransactions     = transactionsResults.Select(e => e.IdTransaction).ToList();

        if (idsTransactions is { Count: 0 })
        {
            _logger.LogWarning(builder.ToString());
            return false;
        }

        builder.AppendLine(
            $"[InvoiceService] | RevertUnconfirmedOrUnpaidTransactions | idsTransactions {idsTransactions.ToJsonString()}");
        var invoicesResult   = await _invoiceRepository.GetInvoicesByReceiptNumber(idsTransactions);
        var invoicesToRevert = invoicesResult.Select(e => new InvoiceNumber { InvoiceNumberValue = e.Id }).ToList();

        if (invoicesToRevert is { Count: 0 })
        {
            _logger.LogWarning(builder.ToString());
            return false;
        }

        builder.AppendLine(
            $"[InvoiceService] | RevertUnconfirmedOrUnpaidTransactions | invoicesToRevert {idsTransactions.ToJsonString()}");
        await _invoiceRepository.RevertCoinPaymentTransactions(invoicesToRevert);

        builder.AppendLine($"[InvoiceService] | RevertUnconfirmedOrUnpaidTransactions | Completed");

        _logger.LogInformation(builder.ToString());

        return true;
    }

    public async Task<IEnumerable<InvoiceTradingAcademyDto?>> GetAllInvoicesForTradingAcademyPurchases()
    {
        var response = await _invoiceRepository.GetAllInvoicesForTradingAcademyPurchases();

        if (response is null)
            return new List<InvoiceTradingAcademyDto>();

        var mappedList = Mapper.Map<List<InvoiceTradingAcademyDto>>(response);

        if (mappedList is null)
            return new List<InvoiceTradingAcademyDto>();

        foreach (var invoice in mappedList)
        {
            if (invoice.ProductId == 31)
            {
                var (startDate, endDate) = CommonExtensions.CalculateMonthlyCourseDates(invoice.CreatedAt);
                invoice.StartDay         = startDate.Date.ToString("yyyy-MM-dd");
                invoice.EndDay           = endDate.Date.ToString("yyyy-MM-dd");
            }
            else if (invoice.ProductId == 32)
            {
                var (startDate, endDate) = CommonExtensions.CalculateWeeklyCourseDates(invoice.CreatedAt);
                invoice.StartDay         = startDate.Date.ToString("yyyy-MM-dd");
                invoice.EndDay           = endDate.Date.ToString("yyyy-MM-dd");
            }
        }

        return mappedList;
    }

    public async Task<IEnumerable<UserAffiliateResponse>> SendInvitationsForUpcomingCourses(string link, string code)
    {
        var allInvoices = await _invoiceRepository.GetAllInvoicesForTradingAcademyPurchases();
        var nextMonday  = CommonExtensions.CalculateNextMonday(DateTime.Today);

        if (allInvoices is null)
            return new List<UserAffiliateResponse>();

        var tasks = allInvoices.Where(invoice =>
                invoice.ProductId == Constants.ForMonth || invoice.ProductId == Constants.ForWeek)
            .Select(async invoice =>
            {
                var endDate = invoice.ProductId == Constants.ForMonth
                                  ? CommonExtensions.CalculateMonthlyCourseDates(invoice.CreatedAt).EndDate
                                  : CommonExtensions.CalculateWeeklyCourseDates(invoice.CreatedAt).EndDate;

                if (endDate.Date < nextMonday)
                    return null;

                var userResponse = await _accountServiceAdapter.GetAffiliateByUserName(invoice.UserName);
                if (userResponse.Content is null)
                    return null;

                var user = JsonSerializer.Deserialize<UserAffiliateResponse>(userResponse.Content);
                await _brevoEmailService.SendInvitationsForTradingAcademy(user!, link, code);
                return user?.Data is null ? null : user;
            });

        var results = await Task.WhenAll(tasks);
        return results.Where(user => user != null).Select(user => user!);
    }

    public async Task<IEnumerable<InvoiceModelOneAndTwoDto>> GetAllInvoicesModelOneAndTwo()
    {
        var response = await _invoiceRepository.GetAllInvoicesModelOneAndTwo();

        if (response is null)
            return new List<InvoiceModelOneAndTwoDto>();

        var invoices = Mapper.Map<IEnumerable<InvoiceModelOneAndTwoDto>>(response);
        var invoiceOrder = invoices.OrderByDescending(e=>e.CreatedAt).ToList();
        
        return invoiceOrder;
    }
}