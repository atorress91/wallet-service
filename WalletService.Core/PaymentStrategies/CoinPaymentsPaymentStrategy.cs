using System.Reflection;
using WalletService.Core.PaymentStrategies.IPaymentStrategies;
using WalletService.Core.Services.IServices;
using WalletService.Data.Adapters.IAdapters;
using WalletService.Data.Repositories.IRepositories;
using WalletService.Models.Enums;
using WalletService.Models.Requests.WalletRequest;
using WalletService.Models.Responses;
using WalletService.Utility.Extensions;
using Constants = WalletService.Models.Constants.Constants;

namespace WalletService.Core.PaymentStrategies;

public class CoinPaymentsPaymentStrategy : ICoinPaymentsPaymentStrategy
{
    private readonly IInvoiceRepository       _invoiceRepository;
    private readonly IInventoryServiceAdapter _inventoryServiceAdapter;
    private readonly IAccountServiceAdapter   _accountServiceAdapter;
    private readonly IEcosystemPdfService      _ecosystemPdfService;
    private readonly IBrevoEmailService       _brevoEmailService;
    private readonly IWalletRepository        _walletRepository;
    private readonly IBrandService _brandService;
    public CoinPaymentsPaymentStrategy(IInvoiceRepository invoiceRepository,     IInventoryServiceAdapter inventoryServiceAdapter,
        IAccountServiceAdapter                            accountServiceAdapter, IBrevoEmailService       brevoEmailService,
        IEcosystemPdfService ecosystemPdfService, IWalletRepository walletRepository,IBrandService brandService)
    {
        _invoiceRepository       = invoiceRepository;
        _inventoryServiceAdapter = inventoryServiceAdapter;
        _accountServiceAdapter   = accountServiceAdapter;
        _brevoEmailService       = brevoEmailService;
        _ecosystemPdfService      = ecosystemPdfService;
        _walletRepository        = walletRepository;
        _brandService            = brandService;
    }
    
    private async Task<Dictionary<string, byte[]>> GetPdfContentForTradingAcademy()
    {
        Dictionary<string, byte[]> pdfContents = new Dictionary<string, byte[]>();

        var workingDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        var separator        = Path.DirectorySeparatorChar;

        var pdfDirectory = $"{workingDirectory}{separator}Assets{separator}TradingAcademy";

        var pdfFiles = Directory.GetFiles(pdfDirectory, "*.pdf");

        foreach (var pdfFile in pdfFiles)
        {
            var fileName   = Path.GetFileName(pdfFile);
            var pdfContent = await File.ReadAllBytesAsync(pdfFile);
            pdfContents[fileName] = pdfContent;
        }

        return pdfContents;
    }

    public async Task<bool> ExecuteEcoPoolPayment(WalletRequest request)
    {
        var  debit          = 0;
        var  points         = 0m;
        var  commissionable = 0m;
        byte origin         = 0;

        var invoiceDetails   = new List<InvoiceDetailsTransactionRequest>();
        var userInfoResponse = await _accountServiceAdapter.GetUserInfo(request.AffiliateId,_brandService.BrandId);
        var productIds       = request.ProductsList.Select(p => p.IdProduct).ToArray();
        var responseList     = await _inventoryServiceAdapter.GetProductsIds(productIds,_brandService.BrandId);

        if (!responseList.IsSuccessful)
            return false;

        if (string.IsNullOrEmpty(responseList.Content))
            return false;

        var result = responseList.Content.ToJsonObject<ProductsResponse>();

        if (result?.Data.Count == Constants.EmptyValue)
        {
            var firstProductId   = request.ProductsList.First().IdProduct;
            var membershipResult = await _inventoryServiceAdapter.GetProductById(firstProductId,_brandService.BrandId);

            var productResponse = membershipResult.Content!.ToJsonObject<ProductResponse>();

            await _accountServiceAdapter.UpdateActivationDate(request.AffiliateId,_brandService.BrandId);

            result.Data.Add(productResponse!.Data);
        }

        if (result?.Data == null)
            return false;

        if (result.Data.Count != request.ProductsList.Count)
            return false;
        
        var productNames = result.Data.Select(item => item.Name).ToArray();
        
        foreach (var item in result.Data)
            
        {
            var product = request.ProductsList.FirstOrDefault(x => x.IdProduct == item.Id);
            var tax     = item.Tax;
            debit          += (int)((item.SalePrice * product!.Count) * (1 + (tax / 100)));
            points         += item.BinaryPoints * product.Count;
            commissionable += item.CommissionableValue * product.Count;
            if (item.CategoryId == 2)
            {
                origin = 1;
            }

            var invoiceDetail = new InvoiceDetailsTransactionRequest
            {
                ProductId             = item.Id,
                PaymentGroupId        = item.PaymentGroup,
                AccumMinPurchase      = Convert.ToByte(item.AcumCompMin),
                ProductName           = item.Name!,
                ProductPrice          = item.SalePrice,
                ProductPriceBtc       = Constants.EmptyValue,
                ProductIva            = item.Tax,
                ProductQuantity       = product.Count,
                ProductCommissionable = item.CommissionableValue,
                BinaryPoints          = item.BinaryPoints,
                ProductPoints         = item.ValuePoints,
                ProductDiscount       = item.ProductDiscount,
                CombinationId         = Constants.EmptyValue,
                ProductPack           = Convert.ToByte(item.ProductPacks),
                BaseAmount            = (item.BaseAmount * product.Count),
                DailyPercentage       = item.DailyPercentage,
                WaitingDays           = item.DaysWait,
                DaysToPayQuantity     = Constants.DaysToPayQuantity,
                ProductStart          = Constants.EmptyValue
            };

            invoiceDetails.Add(invoiceDetail);
        }

        if (debit == Constants.EmptyValue)
            return false;

        if (invoiceDetails.Count == Constants.EmptyValue)
            return false;

        var debitTransactionRequest = new DebitTransactionRequest
        {
            Debit             = debit,
            AffiliateId       = request.AffiliateId,
            UserId            = Constants.AdminUserId,
            ConceptType       = WalletConceptType.purchasing_pool.ToString(),
            Points            = points,
            Concept           = Constants.EcoPoolProductCategory,
            Commissionable    = commissionable,
            Bank              = Constants.CoinPayments,
            PaymentMethod     = Constants.CoinPayments,
            Origin            = origin,
            Level             = Constants.EmptyValue,
            AffiliateUserName = request.AffiliateUserName,
            AdminUserName     = Constants.AdminEcosystemUserName,
            ReceiptNumber     = request.ReceiptNumber,
            Type              = Constants.EmptyValue,
            SecretKey         = request.SecretKey,
            invoices          = invoiceDetails,
            Reason            = string.Empty    
        };

        var spResponse = await _invoiceRepository.HandleDebitTransaction(debitTransactionRequest);

        if (spResponse is null)
            return false;

        var invoicePdf = await _ecosystemPdfService.GenerateInvoice(userInfoResponse!, debitTransactionRequest, spResponse);

        var productPdfsContents = await CommonExtensions.GetPdfContentFromProductNames(productNames!);

        Dictionary<string, byte[]> allPdfData = new Dictionary<string, byte[]>
        {
            ["Invoice.pdf"] = invoicePdf
        };

        foreach (var pdfDataEntry in productPdfsContents)
        {
            allPdfData[pdfDataEntry.Key] = pdfDataEntry.Value;
        }
        if (result.Data.Find(dto => dto.ProductType) != null)
        {
            await _brevoEmailService.SendEmailWelcome(userInfoResponse!, spResponse,request.BrandId);
        }

        if (invoicePdf.Length != Constants.EmptyValue)
        {
            await _brevoEmailService.SendEmailPurchaseConfirm(userInfoResponse!, allPdfData, spResponse,request.BrandId);
        }

        return true;
    }

    public async Task<bool> ExecuteCoursePayment(WalletRequest request)
    {
        var  debit          = 0m;
        var  points         = 0m;
        var  commissionable = 0m;
        byte origin         = 0;

        var invoiceDetails   = new List<InvoiceDetailsTransactionRequest>();
        var userInfoResponse = await _accountServiceAdapter.GetUserInfo(request.AffiliateId,_brandService.BrandId);
        var productIds       = request.ProductsList.Select(p => p.IdProduct).ToArray();
        var responseList     = await _inventoryServiceAdapter.GetProductsIds(productIds,_brandService.BrandId);

        if (!responseList.IsSuccessful)
            return false;

        if (string.IsNullOrEmpty(responseList.Content))
            return false;

        var result = responseList.Content.ToJsonObject<ProductsResponse>();

        if (result?.Data == null)
            return false;

        if (result.Data.Count != request.ProductsList.Count)
            return false;

        foreach (var item in result.Data)
        {
            var product = request.ProductsList.FirstOrDefault(x => x.IdProduct == item.Id);
            var tax     = item.Tax;
            debit          += (item.SalePrice * product!.Count) * (1 + (tax / 100));
            points         += item.BinaryPoints * product.Count;
            commissionable += item.CommissionableValue * product.Count;
            if (item.CategoryId == 2)
            {
                origin = 1;
            }

            var invoiceDetail = new InvoiceDetailsTransactionRequest
            {
                ProductId             = item.Id,
                PaymentGroupId        = item.PaymentGroup,
                AccumMinPurchase      = Convert.ToByte(item.AcumCompMin),
                ProductName           = item.Name!,
                ProductPrice          = item.SalePrice,
                ProductPriceBtc       = Constants.EmptyValue,
                ProductIva            = item.Tax,
                ProductQuantity       = product.Count,
                ProductCommissionable = item.CommissionableValue,
                BinaryPoints          = item.BinaryPoints,
                ProductPoints         = item.ValuePoints,
                ProductDiscount       = item.ProductDiscount,
                CombinationId         = Constants.EmptyValue,
                ProductPack           = Convert.ToByte(item.ProductPacks),
                BaseAmount            = (item.BaseAmount * product.Count),
                DailyPercentage       = item.DailyPercentage,
                WaitingDays           = item.DaysWait,
                DaysToPayQuantity     = Constants.DaysToPayQuantity,
                ProductStart          = Constants.EmptyValue
            };

            invoiceDetails.Add(invoiceDetail);
        }

        if (debit == Constants.EmptyValue)
            return false;

        if (invoiceDetails.Count == Constants.EmptyValue)
            return false;

        var debitTransactionRequest = new DebitTransactionRequest
        {
            Debit             = debit,
            AffiliateId       = request.AffiliateId,
            UserId            = Constants.AdminUserId,
            ConceptType       = WalletConceptType.purchasing_pool.ToString(),
            Points            = points,
            Concept           = Constants.EcoPoolProductCategory,
            Commissionable    = commissionable,
            Bank              = Constants.CoinPayments,
            PaymentMethod     = Constants.CoinPayments,
            Origin            = origin,
            Level             = Constants.EmptyValue,
            AffiliateUserName = request.AffiliateUserName,
            AdminUserName     = Constants.AdminEcosystemUserName,
            ReceiptNumber     = request.ReceiptNumber,
            Type              = Constants.EmptyValue,
            SecretKey         = request.SecretKey,
            invoices          = invoiceDetails,
            Reason            = string.Empty    
        };

        var hasCourse  = await _invoiceRepository.GetInvoicesForTradingAcademyPurchases(request.AffiliateId);
        var spResponse = await _invoiceRepository.HandleDebitTransactionForCourse(debitTransactionRequest);

        if (spResponse is null)
            return false;

        Dictionary<string, byte[]> allPdfData = new Dictionary<string, byte[]>();
        var                        invoicePdf = await _ecosystemPdfService.GenerateInvoice(userInfoResponse!, debitTransactionRequest, spResponse);

        allPdfData["Invoice.pdf"] = invoicePdf;

        if (!hasCourse && invoicePdf.Length != Constants.EmptyValue)
        {
            var pdfContents = await GetPdfContentForTradingAcademy();
            foreach (var pdf in pdfContents)
            {
                allPdfData[pdf.Key] = pdf.Value;
            }
            await _brevoEmailService.SendEmailPurchaseConfirmForAcademy(userInfoResponse!, allPdfData, spResponse,request.BrandId);
        }
        else if (invoicePdf.Length != Constants.EmptyValue)
        {
            await _brevoEmailService.SendEmailPurchaseConfirm(userInfoResponse!, allPdfData, spResponse,request.BrandId);
        }

        return true;
    }
    public async Task<bool> ExecuteMembershipPayment(WalletRequest request)
    {
        var  debit          = 0;
        var  points         = 0m;
        var  commissionable = 0m;
        byte origin         = 0;

        var invoiceDetails       = new List<InvoiceDetailsTransactionRequest>();
        var userInfoResponse     = await _accountServiceAdapter.GetUserInfo(request.AffiliateId,_brandService.BrandId);
        var affiliateBonusWinner = await _accountServiceAdapter.GetUserInfo(userInfoResponse!.Father,_brandService.BrandId);

        var firstProductId   = request.ProductsList.First().IdProduct;
        var membershipResult = await _inventoryServiceAdapter.GetProductById(firstProductId,_brandService.BrandId);

        var productResponse = membershipResult.Content!.ToJsonObject<ProductResponse>();

        if (productResponse?.Data == null)
            return false;

        var item    = productResponse.Data;
        var product = request.ProductsList.FirstOrDefault(x => x.IdProduct == item.Id);
        if (product != null)
        {
            var tax = item.Tax;
            debit          += (int)((item.SalePrice * product.Count) * (1 + (tax / 100)));
            points         += item.BinaryPoints * product.Count;
            commissionable += item.CommissionableValue * product.Count;
            if (item.CategoryId == 2)
            {
                origin = 1;
            }

            var invoiceDetail = new InvoiceDetailsTransactionRequest
            {
                ProductId             = item.Id,
                PaymentGroupId        = item.PaymentGroup,
                AccumMinPurchase      = Convert.ToByte(item.AcumCompMin),
                ProductName           = item.Name!,
                ProductPrice          = item.SalePrice,
                ProductPriceBtc       = Constants.EmptyValue,
                ProductIva            = item.Tax,
                ProductQuantity       = product.Count,
                ProductCommissionable = item.CommissionableValue,
                BinaryPoints          = item.BinaryPoints,
                ProductPoints         = item.ValuePoints,
                ProductDiscount       = Constants.EmptyValue,
                CombinationId         = Constants.EmptyValue,
                ProductPack           = Convert.ToByte(item.ProductPacks),
                BaseAmount            = item.BaseAmount,
                DailyPercentage       = item.DailyPercentage,
                WaitingDays           = item.DaysWait,
                DaysToPayQuantity     = Constants.EmptyValue,
                ProductStart          = Constants.EmptyValue,
            };

            invoiceDetails.Add(invoiceDetail);
        }

        if (debit == Constants.EmptyValue)
            return false;

        if (invoiceDetails.Count == Constants.EmptyValue)
            return false;

        var debitTransactionRequest = new DebitTransactionRequest
        {
            Debit             = debit,
            AffiliateId       = request.AffiliateId,
            UserId            = Constants.AdminUserId,
            ConceptType       = WalletConceptType.purchasing_pool.ToString(),
            Points            = points,
            Concept           = Constants.Membership,
            Commissionable    = commissionable,
            Bank              = Constants.CoinPayments,
            PaymentMethod     = Constants.CoinPayments,
            Origin            = origin,
            Level             = Constants.EmptyValue,
            AffiliateUserName = request.AffiliateUserName,
            AdminUserName     = Constants.AdminEcosystemUserName,
            ReceiptNumber     = request.ReceiptNumber,
            Type              = Constants.EmptyValue,
            SecretKey         = request.SecretKey,
            invoices          = invoiceDetails,
            Reason            = string.Empty    
        };

        var spResponse = await _walletRepository.HandleMembershipTransaction(debitTransactionRequest);

        if (spResponse is null)
            return false;

        await _accountServiceAdapter.UpdateActivationDate(request.AffiliateId,_brandService.BrandId);

        var pdfResult = await _ecosystemPdfService.GenerateInvoice(userInfoResponse, debitTransactionRequest, spResponse);

        await _brevoEmailService.SendEmailWelcome(userInfoResponse, spResponse,request.BrandId);

        if (pdfResult.Length != Constants.EmptyValue)
        {
            await _brevoEmailService.SendEmailMembershipConfirm(userInfoResponse, pdfResult, spResponse,request.BrandId);
        }

        return true;
    }
}