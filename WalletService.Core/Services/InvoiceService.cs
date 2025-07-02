using AutoMapper;
using Microsoft.Extensions.Logging;
using System.Text;
using ClosedXML.Excel;
using WalletService.Core.Caching;
using WalletService.Core.Services.IServices;
using WalletService.Data.Adapters.IAdapters;
using WalletService.Data.Database.CustomModels;
using WalletService.Data.Database.Models;
using WalletService.Data.Repositories.IRepositories;
using WalletService.Models.Constants;
using WalletService.Models.DTO.InvoiceDto;
using WalletService.Models.DTO.PaginationDto;
using WalletService.Models.Enums;
using WalletService.Models.Requests.InvoiceRequest;
using WalletService.Models.Requests.PaginationRequest;
using WalletService.Models.Requests.WalletRequest;
using WalletService.Models.Responses;
using WalletService.Utility.Extensions;

namespace WalletService.Core.Services;

public class InvoiceService : BaseService, IInvoiceService
{
    private readonly IInvoiceRepository _invoiceRepository;
    private readonly ITransactionRepository _transactionRepository;
    private readonly ILogger<InvoiceService> _logger;
    private readonly IAccountServiceAdapter _accountServiceAdapter;
    private readonly IBrevoEmailService _brevoEmailService;
    private readonly IWalletModel1ARepository _walletModel1ARepository;
    private readonly IWalletModel1BRepository _walletModel1BRepository;
    private readonly IWalletRepository _walletRepository;
    private readonly IEcosystemPdfService _ecosystemPdfService;
    private readonly RedisCache _redisCache;
    private readonly IBrandService _brandService;
    private readonly IRecyCoinPdfService _recyCoinPdfService;

    public InvoiceService(IMapper mapper, IInvoiceRepository invoiceRepository,
        ITransactionRepository transactionRepository,
        ILogger<InvoiceService> logger, IAccountServiceAdapter accountServiceAdapter,
        IBrevoEmailService brevoEmailService, IWalletModel1ARepository walletModel1ARepository,
        IWalletModel1BRepository walletModel1BRepository, IWalletRepository walletRepository,
        IEcosystemPdfService ecosystemPdfService, RedisCache redisCache, IBrandService brandService,
        IRecyCoinPdfService recyCoinPdfService) : base(mapper)
    {
        _invoiceRepository = invoiceRepository;
        _transactionRepository = transactionRepository;
        _logger = logger;
        _accountServiceAdapter = accountServiceAdapter;
        _brevoEmailService = brevoEmailService;
        _walletModel1ARepository = walletModel1ARepository;
        _walletModel1BRepository = walletModel1BRepository;
        _walletRepository = walletRepository;
        _ecosystemPdfService = ecosystemPdfService;
        _redisCache = redisCache;
        _brandService = brandService;
        _recyCoinPdfService = recyCoinPdfService;
    }

    public async Task<IEnumerable<InvoiceDto>> GetAllInvoiceUserAsync(int id)
    {
        var response = await _invoiceRepository.GetAllInvoicesUser(id, _brandService.BrandId);
        var mappedList = Mapper.Map<IEnumerable<InvoiceDto>>(response).ToList();
        mappedList.Reverse();

        return mappedList;
    }

    public async Task<InvoiceDto> CreateInvoiceAsync(InvoiceRequest request)
    {
        var invoice = Mapper.Map<Invoice>(request);
        invoice = await _invoiceRepository.CreateInvoiceAsync(invoice);
        return Mapper.Map<InvoiceDto>(invoice);
    }

    public async Task<PaginationDto<InvoiceDto>> GetAllInvoices(PaginationRequest request)
    {
        ValidateDateRange(request.StartDate, request.EndDate);

        var response = await _invoiceRepository.GetAllInvoices(_brandService.BrandId, request);
        var mappedItems = Mapper.Map<List<InvoiceDto>>(response.Items);

        var userTasks = mappedItems.Select(async invoice =>
        {
            try
            {
                var user = await _accountServiceAdapter.GetUserInfo(invoice.AffiliateId, _brandService.BrandId);
                invoice.UserName = user?.UserName;
                invoice.Name = user?.Name;
                invoice.LastName = user?.LastName;
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Error getting user info for affiliate {invoice.AffiliateId}");
                throw;
            }
        });

        await Task.WhenAll(userTasks);

        return new PaginationDto<InvoiceDto>
        {
            CurrentPage = response.CurrentPage,
            PageSize = response.PageSize,
            TotalCount = response.TotalCount,
            TotalPages = response.TotalPages,
            Items = mappedItems
        };
    }

    private void ValidateDateRange(DateTime? startDate, DateTime? endDate)
    {
        if (startDate.HasValue && endDate.HasValue && startDate > endDate)
        {
            throw new ArgumentException("The start date must be before the end date.");
        }
    }

    public async Task<bool> RevertUnconfirmedOrUnpaidTransactions()
    {
        var builder = new StringBuilder();

        builder.AppendLine($"[InvoiceService] | RevertUnconfirmedOrUnpaidTransactions | Started");
        var transactionsResults =
            await _transactionRepository.GetAllUnconfirmedOrUnpaidTransactions(_brandService.BrandId);
        var idsTransactions = transactionsResults.Select(e => e.IdTransaction).ToList();

        if (idsTransactions is { Count: 0 })
        {
            _logger.LogWarning(builder.ToString());
            return false;
        }

        builder.AppendLine(
            $"[InvoiceService] | RevertUnconfirmedOrUnpaidTransactions | idsTransactions {idsTransactions.ToJsonString()}");
        var invoicesResult = await _invoiceRepository.GetInvoicesByReceiptNumber(idsTransactions);
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
                invoice.StartDay = startDate.Date.ToString("yyyy-MM-dd");
                invoice.EndDay = endDate.Date.ToString("yyyy-MM-dd");
            }
            else if (invoice.ProductId == 32)
            {
                var (startDate, endDate) = CommonExtensions.CalculateWeeklyCourseDates(invoice.CreatedAt);
                invoice.StartDay = startDate.Date.ToString("yyyy-MM-dd");
                invoice.EndDay = endDate.Date.ToString("yyyy-MM-dd");
            }
        }

        return mappedList;
    }

    public async Task<IEnumerable<UserAffiliateResponse>> SendInvitationsForUpcomingCourses(string link, string code)
    {
        var allInvoices = await _invoiceRepository.GetAllInvoicesForTradingAcademyPurchases();
        var nextMonday = CommonExtensions.CalculateNextMonday(DateTime.Today);

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

                var userResponse =
                    await _accountServiceAdapter.GetAffiliateByUserName(invoice.UserName, _brandService.BrandId);
                if (userResponse.Content is null)
                    return null;

                var user = userResponse.Content.ToJsonObject<UserAffiliateResponse>();
                await _brevoEmailService.SendInvitationsForTradingAcademy(user!, link, code, _brandService.BrandId);
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
        var invoicesOrdered = invoices.OrderByDescending(e => e.CreatedAt).ToList();

        return invoicesOrdered;
    }

    public async Task<ModelBalancesAndInvoicesDto?> ProcessAndReturnBalancesForModels1A1B2(
        ModelBalancesAndInvoicesRequest request)
    {
        var totalModels = request.Model1AAmount + request.Model1BAmount + request.Model2Amount;
        if (request.InvoiceId.Length == 0)
            return null;

        var (validInvoiceIds, affiliateId, totalInvoices) = await ProcessInvoices(request.InvoiceId);

        if (affiliateId == 0 || totalModels > totalInvoices)
            return null;

        bool isAnyCreditTransactionSuccessful = false;

        if (request.Model1AAmount > 0)
            isAnyCreditTransactionSuccessful |=
                await CreditAmountToWallet(affiliateId, request.UserName, request.Model1AAmount, "Model1A");

        if (request.Model1BAmount > 0)
            isAnyCreditTransactionSuccessful |=
                await CreditAmountToWallet(affiliateId, request.UserName, request.Model1BAmount, "Model1B");

        if (request.Model2Amount > 0)
            isAnyCreditTransactionSuccessful |=
                await CreditAmountToWallet(affiliateId, request.UserName, request.Model2Amount, "Model2");

        if (isAnyCreditTransactionSuccessful && validInvoiceIds.Any())
        {
            await _invoiceRepository.DeleteMultipleInvoicesAndDetailsAsync(validInvoiceIds.ToArray(),
                _brandService.BrandId);
        }

        return new ModelBalancesAndInvoicesDto
        {
            UserName = request.UserName,
            Model1AAmount = request.Model1AAmount,
            Model1BAmount = request.Model1BAmount,
            Model2Amount = request.Model2Amount,
            InvoiceId = validInvoiceIds.ToArray()
        };
    }

    private async Task<(List<long>, int, decimal)> ProcessInvoices(long[] invoiceIds)
    {
        var totalInvoices = 0m;
        var validInvoiceIds = new HashSet<long>();
        int affiliateId = 0;

        foreach (var invoiceId in invoiceIds)
        {
            if (!validInvoiceIds.Contains(invoiceId))
            {
                var invoice = await _invoiceRepository.GetInvoiceById(invoiceId, _brandService.BrandId);

                if (invoice != null)
                {
                    if (affiliateId == 0)
                        affiliateId = invoice.AffiliateId;
                    else if (affiliateId != invoice.AffiliateId)
                    {
                        return (new List<long>(), 0, 0m);
                    }

                    totalInvoices += invoice.TotalInvoice ?? 0;
                    validInvoiceIds.Add(invoiceId);
                }
            }
        }

        return (validInvoiceIds.ToList(), affiliateId, totalInvoices);
    }

    private async Task<bool> CreditAmountToWallet(int affiliateId, string userName, decimal amount, string model)
    {
        if (amount <= 0) return false;

        var creditTransactionRequest = new CreditTransactionRequest
        {
            AffiliateId = affiliateId,
            UserId = Constants.AdminUserId,
            Concept = Constants.BalanceRefunds,
            Credit = Convert.ToDouble(amount),
            AffiliateUserName = userName,
            AdminUserName = Constants.AdminEcosystemUserName,
            ConceptType = WalletConceptType.revert_pool.ToString()
        };

        try
        {
            switch (model)
            {
                case "Model1A":
                    await RemoveCacheKey(affiliateId, CacheKeys.BalanceInformationModel1A);
                    await RemoveCacheKey(affiliateId, CacheKeys.BalanceInformationModel1B);
                    await RemoveCacheKey(affiliateId, CacheKeys.BalanceInformationModel2);
                    return await _walletModel1ARepository.CreditTransaction(creditTransactionRequest);
                case "Model1B":
                    await RemoveCacheKey(affiliateId, CacheKeys.BalanceInformationModel1A);
                    await RemoveCacheKey(affiliateId, CacheKeys.BalanceInformationModel1B);
                    await RemoveCacheKey(affiliateId, CacheKeys.BalanceInformationModel2);
                    return await _walletModel1BRepository.CreditTransaction(creditTransactionRequest);
                case "Model2":
                    await RemoveCacheKey(affiliateId, CacheKeys.BalanceInformationModel1A);
                    await RemoveCacheKey(affiliateId, CacheKeys.BalanceInformationModel1B);
                    await RemoveCacheKey(affiliateId, CacheKeys.BalanceInformationModel2);
                    return await _walletRepository.CreditTransaction(creditTransactionRequest);
            }

            return false;
        }
        catch
        {
            return false;
        }
    }

    public async Task<byte[]> CreateInvoice(int invoiceId)
    {
        var invoice = await _invoiceRepository.GetInvoiceById(invoiceId, _brandService.BrandId);
        if (invoice is null)
            return Array.Empty<byte>();

        var user = await _accountServiceAdapter.GetUserInfo(invoice.AffiliateId, _brandService.BrandId);

        if (user is null)
            return Array.Empty<byte>();

        var generatedInvoice = Array.Empty<byte>();
        if (Constants.Ecosystem == _brandService.BrandId)
        {
            generatedInvoice = await _ecosystemPdfService.RegenerateInvoice(user, invoice);
        }
        else if (Constants.RecyCoin == _brandService.BrandId)
        {
            generatedInvoice = await _recyCoinPdfService.RegenerateInvoice(user, invoice);
        }

        return generatedInvoice;
    }

    public async Task<InvoiceResultDto?> CreateInvoiceByReference(string reference)
    {
        var invoice = await _invoiceRepository.GetInvoiceByReceiptNumber(reference, _brandService.BrandId);
        if (invoice is null)
            return null;

        var user = await _accountServiceAdapter.GetUserInfo(invoice.AffiliateId, _brandService.BrandId);

        if (user is null)
            return null;

        byte[] generatedInvoice;
        if (Constants.Ecosystem == _brandService.BrandId)
        {
            generatedInvoice = await _ecosystemPdfService.RegenerateInvoice(user, invoice);
        }
        else if (Constants.RecyCoin == _brandService.BrandId)
        {
            generatedInvoice = await _recyCoinPdfService.RegenerateInvoice(user, invoice);
        }
        else
        {
            return null;
        }

        return new InvoiceResultDto
        {
            PdfContent = generatedInvoice,
            BrandId = _brandService.BrandId
        };
    }

    private async Task RemoveCacheKey(int affiliateId, string stringKey)
    {
        var key = string.Format(stringKey, affiliateId);
        var existsKey = await _redisCache.KeyExists(key);

        if (existsKey)
            await _redisCache.Delete(key);
    }

    public async Task<InvoicesSpResponse?> HandleDebitTransaction(DebitTransactionRequest request)
    {
        try
        {
            var result = await _invoiceRepository.HandleDebitTransaction(request);

            if (result == null)
            {
                _logger.LogWarning("No se pudo procesar la transacción de débito para el afiliado: {AffiliateId}",
                    request.AffiliateId);
                return null;
            }

            return result;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error al procesar la transacción de débito para el afiliado: {AffiliateId}",
                request.AffiliateId);
            throw;
        }
    }

    public async Task<MemoryStream> GenerateExcelReport(DateTime? startDate, DateTime? endDate)
    {
        const int batchSize = 30;
        var stream = new MemoryStream();

        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Compras");

        int currentRow = 1;
        ConfigureHeaders(worksheet);

        decimal totalSum = 0;
        currentRow = 2;

        await foreach (var invoiceBatch in _invoiceRepository.GetInvoicesInBatches(startDate, endDate, batchSize,
                           _brandService.BrandId))
        {
            var userTasks = invoiceBatch.Select(invoice =>
                _accountServiceAdapter.GetUserInfo(invoice.AffiliateId, _brandService.BrandId)
            ).ToList();

            var users = await Task.WhenAll(userTasks);

            for (int i = 0; i < invoiceBatch.Count; i++)
            {
                var invoice = invoiceBatch[i];
                var user = users[i];

                worksheet.Cell(currentRow, 1).Value = user?.UserName ?? "N/A";
                worksheet.Cell(currentRow, 2).Value = $"{user?.Name ?? "N/A"} {user?.LastName ?? ""}";
                worksheet.Cell(currentRow, 3).Value = invoice.Id;
                worksheet.Cell(currentRow, 4).Value = invoice.Status ? "Activa" : "Pendiente o Nula";
                worksheet.Cell(currentRow, 5).Value = invoice.TotalInvoice;
                worksheet.Cell(currentRow, 6).Value = invoice.PaymentMethod;
                worksheet.Cell(currentRow, 7).Value = invoice.Bank;
                worksheet.Cell(currentRow, 8).Value = invoice.ReceiptNumber;
                worksheet.Cell(currentRow, 9).Value = invoice.DepositDate.ToString();

                totalSum += invoice.TotalInvoice ?? 0;
                currentRow++;
            }
        }

        worksheet.Cell(currentRow + 1, 1).Value = "TOTAL:";
        worksheet.Cell(currentRow + 1, 5).Value = totalSum;

        FormatWorksheet(worksheet, currentRow);

        workbook.SaveAs(stream);
        stream.Position = 0;
        return stream;
    }

    private void ConfigureHeaders(IXLWorksheet worksheet)
    {
        var headers = new[]
        {
            "Afiliado",
            "Nombre y Apellido",
            "No. Factura",
            "Estado factura",
            "Total pagado",
            "Método de pago",
            "Banco",
            "No. Recibo",
            "Fecha de depósito"
        };

        for (int i = 0; i < headers.Length; i++)
        {
            var cell = worksheet.Cell(1, i + 1);
            cell.Value = headers[i];
            cell.Style.Font.Bold = true;
            cell.Style.Fill.BackgroundColor = XLColor.LightGray;
        }
    }

    private void FormatWorksheet(IXLWorksheet worksheet, int lastRow)
    {
        worksheet.Column(1).Width = 15;
        worksheet.Column(2).Width = 25;
        worksheet.Column(3).Width = 12;
        worksheet.Column(4).Width = 15;
        worksheet.Column(5).Width = 15;
        worksheet.Column(6).Width = 15;
        worksheet.Column(7).Width = 15;
        worksheet.Column(8).Width = 15;
        worksheet.Column(9).Width = 15;

        worksheet.Column(5).Style.NumberFormat.Format = "$#,##0.00";
        worksheet.Column(9).Style.NumberFormat.Format = "dd/mm/yyyy";

        var table = worksheet.Range(1, 1, lastRow, 9);
        table.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
        table.Style.Border.InsideBorder = XLBorderStyleValues.Thin;

        var totalRow = worksheet.Range(lastRow + 1, 1, lastRow + 1, 9);
        totalRow.Style.Font.Bold = true;
        totalRow.Style.Fill.BackgroundColor = XLColor.LightGray;
    }
}