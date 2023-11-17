using System.Reflection;
using System.Text.Json;
using WalletService.Core.PaymentStrategies.IPaymentStrategies;
using WalletService.Core.Services.IServices;
using WalletService.Data.Adapters.IAdapters;
using WalletService.Data.Repositories.IRepositories;
using WalletService.Models.DTO.BalanceInformationDto;
using WalletService.Models.Enums;
using WalletService.Models.Requests.WalletRequest;
using WalletService.Models.Responses;
using Constants = WalletService.Models.Constants.Constants;

namespace WalletService.Core.PaymentStrategies;

public class BalancePaymentStrategy : IBalancePaymentStrategy
{
    private readonly IInventoryServiceAdapter _inventoryServiceAdapter;
    private readonly IAccountServiceAdapter   _accountServiceAdapter;
    private readonly IWalletRepository        _walletRepository;
    private readonly IMediatorPdfService      _mediatorPdfService;
    private readonly IBrevoEmailService       _brevoEmailService;
    private readonly IWalletRequestRepository _walletRequestRepository;

    public BalancePaymentStrategy(IInventoryServiceAdapter inventoryServiceAdapter,
        IAccountServiceAdapter                             accountServiceAdapter, IWalletRepository        walletRepository, IMediatorPdfService mediatorPdfService,
        IBrevoEmailService                                 brevoEmailService,     IWalletRequestRepository walletRequestRepository)
    {
        _inventoryServiceAdapter = inventoryServiceAdapter;
        _accountServiceAdapter   = accountServiceAdapter;
        _walletRepository        = walletRepository;
        _brevoEmailService       = brevoEmailService;
        _mediatorPdfService      = mediatorPdfService;
        _walletRequestRepository = walletRequestRepository;
    }
    
    private async Task<Dictionary<string, byte[]>> GetPdfContentFromProductIds(int[] productIds)
    {
        Dictionary<string, byte[]> pdfContents = new Dictionary<string, byte[]>();

        var workingDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        var separator = Path.DirectorySeparatorChar;

        foreach(var id in productIds)
        {
            if (Enum.IsDefined(typeof(ProductPdfs), id))
            {
                var enumValue = (ProductPdfs)id;
                var pdfName = $"{enumValue}.pdf";
                var path = $"{workingDirectory}{separator}Assets{separator}EcoPooles{separator}{enumValue}.pdf";
                
                var pdfContent = await File.ReadAllBytesAsync(path);
                pdfContents[pdfName] = pdfContent;
            }
            else
            {
                Console.WriteLine($"The product ID {{id}} does not have an associated PDF.");
            }
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
        var userInfoResponse = await _accountServiceAdapter.GetUserInfo(request.AffiliateId);
        var balanceInfo      = await GetBalanceInformationByAffiliateId(request.AffiliateId);
        var productIds       = request.ProductsList.Select(p => p.IdProduct).ToArray();
        var responseList     = await _inventoryServiceAdapter.GetProductsIds(productIds);
        
        if (!responseList.IsSuccessful)
            return false;

        if (string.IsNullOrEmpty(responseList.Content))
            return false;

        var result = JsonSerializer.Deserialize<ProductsResponse>(responseList.Content);

        if (result?.Data.Count == 0)
        {
            var firstProductId   = request.ProductsList.First().IdProduct;
            var membershipResult = await _inventoryServiceAdapter.GetProductById(firstProductId);

            var productResponse = JsonSerializer.Deserialize<ProductResponse>(membershipResult.Content!);

            await _accountServiceAdapter.UpdateActivationDate(request.AffiliateId);

            result.Data.Add(productResponse!.Data);
        }

        if (result?.Data == null)
            return false;

        if (result.Data.Count != request.ProductsList.Count)
            return false;


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
                ProductPriceBtc       = Constants.None,
                ProductIva            = item.Tax,
                ProductQuantity       = product.Count,
                ProductCommissionable = item.CommissionableValue,
                BinaryPoints          = item.BinaryPoints,
                ProductPoints         = item.ValuePoints,
                ProductDiscount       = item.ProductDiscount,
                CombinationId         = Constants.None,
                ProductPack           = Convert.ToByte(item.ProductPacks),
                BaseAmount            = (item.BaseAmount * product.Count),
                DailyPercentage       = item.DailyPercentage,
                WaitingDays           = item.DaysWait,
                DaysToPayQuantity     = Constants.DaysToPayQuantity,
                ProductStart          = Constants.None,
            };

            invoiceDetails.Add(invoiceDetail);
        }

        if (debit == 0)
            return false;

        if (invoiceDetails.Count == 0)
            return false;

        if (debit > balanceInfo.AvailableBalance.GetValueOrDefault())
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
            Bank              = request.Bank,
            PaymentMethod     = Constants.WalletBalance,
            Origin            = origin,
            Level             = Constants.None,
            AffiliateUserName = request.AffiliateUserName,
            AdminUserName     = Constants.AdminEcosystemUserName,
            ReceiptNumber     = request.ReceiptNumber,
            Type              = Constants.None,
            SecretKey         = request.SecretKey,
            invoices          = invoiceDetails,
        };

        var spResponse = await _walletRepository.DebitTransaction(debitTransactionRequest);

        if (spResponse is null)
            return false;

        var invoicePdf = await _mediatorPdfService.GenerateInvoice(userInfoResponse!, debitTransactionRequest, spResponse);

        var productPdfsContents = await GetPdfContentFromProductIds(productIds);

        Dictionary<string, byte[]> allPdfData = new Dictionary<string, byte[]>
        {
            ["Invoice.pdf"] = invoicePdf
        };

        foreach (var pdfDataEntry in productPdfsContents)
        {
            allPdfData[pdfDataEntry.Key] = pdfDataEntry.Value;
        }

        if (allPdfData.Count > 0)
        {
            await _brevoEmailService.SendEmailPurchaseConfirm(userInfoResponse!, allPdfData, spResponse);
        }

        return true;
    }
    
    private async Task<BalanceInformationDto> GetBalanceInformationByAffiliateId(int affiliateId)
    {
        var amountRequests    = await _walletRequestRepository.GetTotalWalletRequestAmountByAffiliateId(affiliateId);
        var availableBalance  = await _walletRepository.GetAvailableBalanceByAffiliateId(affiliateId);
        var reverseBalance    = await _walletRepository.GetReverseBalanceByAffiliateId(affiliateId);
        var totalAcquisitions = await _walletRepository.GetTotalAcquisitionsByAffiliateId(affiliateId);

        var response = new BalanceInformationDto
        {
            AvailableBalance  = availableBalance,
            ReverseBalance    = reverseBalance ?? 0,
            TotalAcquisitions = totalAcquisitions ?? 0
        };

        if (amountRequests == 0m && response.ReverseBalance == 0m) return response;

         response.AvailableBalance -= amountRequests;
         response.AvailableBalance -= response.ReverseBalance;

        return response;
    }
    public async Task<bool> ExecuteAdminPayment(WalletRequest request)
    {
        var  debit          = 0;
        var  points         = 0m;
        var  commissionable = 0m;
        byte origin         = 0;

        var invoiceDetails   = new List<InvoiceDetailsTransactionRequest>();
        var userInfoResponse = await _accountServiceAdapter.GetUserInfo(request.AffiliateId);
        var productIds       = request.ProductsList.Select(p => p.IdProduct).ToArray();
        var responseList     = await _inventoryServiceAdapter.GetProductsIds(productIds);

        if (!responseList.IsSuccessful)
            return false;

        if (string.IsNullOrEmpty(responseList.Content))
            return false;

        var result = JsonSerializer.Deserialize<ProductsResponse>(responseList.Content);

        if (result?.Data.Count == 0)
        {
            var firstProductId   = request.ProductsList.First().IdProduct;
            var membershipResult = await _inventoryServiceAdapter.GetProductById(firstProductId);

            var productResponse = JsonSerializer.Deserialize<ProductResponse>(membershipResult.Content!);

            await _accountServiceAdapter.UpdateActivationDate(request.AffiliateId);

            result.Data.Add(productResponse!.Data);
        }

        if (result?.Data == null)
            return false;

        if (result.Data.Count != request.ProductsList.Count)
            return false;

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
                ProductPriceBtc       = Constants.None,
                ProductIva            = item.Tax,
                ProductQuantity       = product.Count,
                ProductCommissionable = item.CommissionableValue,
                BinaryPoints          = item.BinaryPoints,
                ProductPoints         = item.ValuePoints,
                ProductDiscount       = item.ProductDiscount,
                CombinationId         = Constants.None,
                ProductPack           = Convert.ToByte(item.ProductPacks),
                BaseAmount            = (item.BaseAmount * product.Count),
                DailyPercentage       = item.DailyPercentage,
                WaitingDays           = item.DaysWait,
                DaysToPayQuantity     = Constants.DaysToPayQuantity,
                ProductStart          = Constants.None,
            };

            invoiceDetails.Add(invoiceDetail);
        }

        if (debit == 0)
            return false;

        if (invoiceDetails.Count == 0)
            return false;

        var debitTransactionRequest = new DebitTransactionRequest
        {
            Debit             = debit,
            AffiliateId       = request.AffiliateId,
            UserId            = Constants.AdminUserId,
            ConceptType       = WalletConceptType.purchasing_pool.ToString(),
            Points            = points,
            Concept           = Constants.EcoPoolProductCategoryForAdmin,
            Commissionable    = commissionable,
            Bank              = request.Bank,
            PaymentMethod     = Constants.AdminPayment,
            Origin            = origin,
            Level             = Constants.None,
            AffiliateUserName = request.AffiliateUserName,
            AdminUserName     = Constants.AdminEcosystemUserName,
            ReceiptNumber     = request.ReceiptNumber,
            Type              = Constants.None,
            SecretKey         = request.SecretKey,
            invoices          = invoiceDetails,
        };

        var spResponse = await _walletRepository.AdminDebitTransaction(debitTransactionRequest);

        if (spResponse is null)
            return false;

        var invoicePdf = await _mediatorPdfService.GenerateInvoice(userInfoResponse!, debitTransactionRequest, spResponse);
        var productPdfsContents = await GetPdfContentFromProductIds(productIds);

        Dictionary<string, byte[]> allPdfData = new Dictionary<string, byte[]>
        {
            ["Invoice.pdf"] = invoicePdf
        };

        foreach (var pdfDataEntry in productPdfsContents)
        {
            allPdfData[pdfDataEntry.Key] = pdfDataEntry.Value;
        }

        if (allPdfData.Count > 0)
        {
            await _brevoEmailService.SendEmailPurchaseConfirm(userInfoResponse!, allPdfData, spResponse);
        }
        
        return true;
    }
    
     public async Task<bool> ExecutePaymentCourses(WalletRequest request)
    {
        var  debit          = 0;
        var  points         = 0m;
        var  commissionable = 0m;
        byte origin         = 0;

        var invoiceDetails   = new List<InvoiceDetailsTransactionRequest>();
        var userInfoResponse = await _accountServiceAdapter.GetUserInfo(request.AffiliateId);
        var balanceInfo      = await GetBalanceInformationByAffiliateId(request.AffiliateId);
        var productIds       = request.ProductsList.Select(p => p.IdProduct).ToArray();
        var responseList     = await _inventoryServiceAdapter.GetProductsIds(productIds);
        
        if (!responseList.IsSuccessful)
            return false;

        if (string.IsNullOrEmpty(responseList.Content))
            return false;

        var result = JsonSerializer.Deserialize<ProductsResponse>(responseList.Content);
        
        if (result?.Data == null)
            return false;

        if (result.Data.Count != request.ProductsList.Count)
            return false;
        
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
                ProductPriceBtc       = Constants.None,
                ProductIva            = item.Tax,
                ProductQuantity       = product.Count,
                ProductCommissionable = item.CommissionableValue,
                BinaryPoints          = item.BinaryPoints,
                ProductPoints         = item.ValuePoints,
                ProductDiscount       = item.ProductDiscount,
                CombinationId         = Constants.None,
                ProductPack           = Convert.ToByte(item.ProductPacks),
                BaseAmount            = (item.BaseAmount * product.Count),
                DailyPercentage       = item.DailyPercentage,
                WaitingDays           = item.DaysWait,
                DaysToPayQuantity     = Constants.DaysToPayQuantity,
                ProductStart          = Constants.None,
            };

            invoiceDetails.Add(invoiceDetail);
        }

        if (debit == 0)
            return false;

        if (invoiceDetails.Count == 0)
            return false;

        if (debit > balanceInfo.AvailableBalance.GetValueOrDefault())
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
            Bank              = request.Bank,
            PaymentMethod     = Constants.WalletBalance,
            Origin            = origin,
            Level             = Constants.None,
            AffiliateUserName = request.AffiliateUserName,
            AdminUserName     = Constants.AdminEcosystemUserName,
            ReceiptNumber     = request.ReceiptNumber,
            Type              = Constants.None,
            SecretKey         = request.SecretKey,
            invoices          = invoiceDetails,
        };

        var spResponse = await _walletRepository.CoursesDebitTransaction(debitTransactionRequest);

        if (spResponse is null)
            return false;

        var invoicePdf = await _mediatorPdfService.GenerateInvoice(userInfoResponse!, debitTransactionRequest, spResponse);
        
        Dictionary<string, byte[]> allPdfData = new Dictionary<string, byte[]>
        {
            ["Invoice.pdf"] = invoicePdf
        };

        if (allPdfData.Count > 0)
        {
            await _brevoEmailService.SendEmailPurchaseConfirm(userInfoResponse!, allPdfData, spResponse);
        }

        return true;
    }
}