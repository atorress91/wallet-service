using System.Text.Json;
using WalletService.Core.PaymentStrategies.IPaymentStrategies;
using WalletService.Core.Services.IServices;
using WalletService.Data.Adapters.IAdapters;
using WalletService.Data.Repositories.IRepositories;
using WalletService.Models.DTO.WalletModel1ADto;
using WalletService.Models.Enums;
using WalletService.Models.Requests.WalletRequest;
using WalletService.Models.Responses;
using WalletService.Utility.Extensions;
using Constants = WalletService.Models.Constants.Constants;

namespace WalletService.Core.PaymentStrategies;

public class BalancePaymentStrategy1A : IBalancePaymentStrategyModel1A
{
    private readonly IInventoryServiceAdapter _inventoryServiceAdapter;
    private readonly IAccountServiceAdapter _accountServiceAdapter;
    private readonly IWalletModel1ARepository _walletModel1ARepository;
    private readonly IMediatorPdfService _mediatorPdfService;
    private readonly IBrevoEmailService _brevoEmailService;


    public BalancePaymentStrategy1A(IInventoryServiceAdapter inventoryServiceAdapter,
        IAccountServiceAdapter accountServiceAdapter, IWalletModel1ARepository walletModel1ARepository,
        IMediatorPdfService mediatorPdfService,
        IBrevoEmailService brevoEmailService)
    {
        _inventoryServiceAdapter = inventoryServiceAdapter;
        _accountServiceAdapter = accountServiceAdapter;
        _walletModel1ARepository = walletModel1ARepository;
        _brevoEmailService = brevoEmailService;
        _mediatorPdfService = mediatorPdfService;
    }
    
    private async Task<BalanceInformationModel1ADto> GetBalanceInformationByAffiliateId(int affiliateId)
    {
        var availableBalance = await _walletModel1ARepository.GetAvailableBalanceByAffiliateId(affiliateId);
        var reverseBalance = await _walletModel1ARepository.GetReverseBalanceByAffiliateId(affiliateId);
        var totalAcquisitions = await _walletModel1ARepository.GetTotalAcquisitionsByAffiliateId(affiliateId);
        var serviceBalance = await _walletModel1ARepository.GetTotalServiceBalance(affiliateId);

        var response = new BalanceInformationModel1ADto()
        {
            AvailableBalance = availableBalance,
            ReverseBalance = reverseBalance ?? 0,
            TotalAcquisitions = totalAcquisitions ?? 0,
            ServiceBalance = serviceBalance ?? 0
        };

        if (response.ReverseBalance == 0m) return response;

        response.AvailableBalance -= response.ReverseBalance;

        return response;
    }

    public async Task<bool> ExecuteEcoPoolPayment(WalletRequest request)
    {
        var debit = 0;
        var points = 0m;
        var commissionable = 0m;
        byte origin = 0;

        var invoiceDetails = new List<InvoiceDetailsTransactionRequest>();
        var userInfoResponse = await _accountServiceAdapter.GetUserInfo(request.AffiliateId);
        var balanceInfo = await GetBalanceInformationByAffiliateId(request.AffiliateId);
        var productIds = request.ProductsList.Select(p => p.IdProduct).ToArray();
        var responseList = await _inventoryServiceAdapter.GetProductsIds(productIds);

        if (!responseList.IsSuccessful)
            return false;

        if (string.IsNullOrEmpty(responseList.Content))
            return false;

        var result = responseList.Content.ToJsonObject<ProductsResponse>();

        if (result?.Data == null)
            return false;

        if (result.Data.Count != request.ProductsList.Count)
            return false;
        
        var productNames = result.Data.Select(item => item.Name).ToArray();

        foreach (var item in result.Data)
        {
            var product = request.ProductsList.FirstOrDefault(x => x.IdProduct == item.Id);
            var tax = item.Tax;
            debit += (int)((item.SalePrice * product!.Count) * (1 + (tax / 100)));
            points += item.BinaryPoints * product.Count;
            commissionable += item.CommissionableValue * product.Count;
            if (item.CategoryId == 2)
            {
                origin = 1;
            }

            var invoiceDetail = new InvoiceDetailsTransactionRequest
            {
                ProductId = item.Id,
                PaymentGroupId = item.PaymentGroup,
                AccumMinPurchase = Convert.ToByte(item.AcumCompMin),
                ProductName = item.Name!,
                ProductPrice = item.SalePrice,
                ProductPriceBtc = Constants.None,
                ProductIva = item.Tax,
                ProductQuantity = product.Count,
                ProductCommissionable = item.CommissionableValue,
                BinaryPoints = item.BinaryPoints,
                ProductPoints = item.ValuePoints,
                ProductDiscount = item.ProductDiscount,
                CombinationId = Constants.None,
                ProductPack = Convert.ToByte(item.ProductPacks),
                BaseAmount = (item.BaseAmount * product.Count),
                DailyPercentage = item.DailyPercentage,
                WaitingDays = item.DaysWait,
                DaysToPayQuantity = Constants.DaysToPayQuantity,
                ProductStart = Constants.None,
            };

            invoiceDetails.Add(invoiceDetail);
        }

        if (debit == 0)
            return false;

        if (invoiceDetails.Count == 0)
            return false;

        if (debit > balanceInfo.ReverseBalance.GetValueOrDefault())
            return false;

        var debitTransactionRequest = new DebitTransactionRequest
        {
            Debit = debit,
            AffiliateId = request.AffiliateId,
            UserId = Constants.AdminUserId,
            ConceptType = WalletConceptType.purchase_with_reverse_balance.ToString(),
            Points = points,
            Concept = Constants.EcoPoolProductCategory,
            Commissionable = commissionable,
            Bank = request.Bank,
            PaymentMethod = Constants.WalletModel1ABalance,
            Origin = origin,
            Level = Constants.None,
            AffiliateUserName = request.AffiliateUserName,
            AdminUserName = Constants.AdminEcosystemUserName,
            ReceiptNumber = request.ReceiptNumber,
            Type = Constants.None,
            SecretKey = request.SecretKey,
            invoices = invoiceDetails,
        };

        var spResponse = await _walletModel1ARepository.DebitTransaction(debitTransactionRequest);

        if (spResponse is null)
            return false;

        var invoicePdf =
            await _mediatorPdfService.GenerateInvoice(userInfoResponse!, debitTransactionRequest, spResponse);

        var productPdfsContents = await CommonExtensions.GetPdfContentFromProductNames(productNames!);

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

    public async Task<bool> ExecuteEcoPoolPaymentWithServiceBalance(WalletRequest request)
    {
        var debit = 0;
        var points = 0m;
        var commissionable = 0m;
        byte origin = 0;

        var invoiceDetails = new List<InvoiceDetailsTransactionRequest>();
        var userInfoResponse = await _accountServiceAdapter.GetUserInfo(request.AffiliateId);
        var balanceInfo = await GetBalanceInformationByAffiliateId(request.AffiliateId);
        var productIds = request.ProductsList.Select(p => p.IdProduct).ToArray();
        var responseList = await _inventoryServiceAdapter.GetProductsIds(productIds);

        if (!responseList.IsSuccessful)
            return false;

        if (string.IsNullOrEmpty(responseList.Content))
            return false;

        var result = responseList.Content.ToJsonObject<ProductsResponse>(); 

        if (result?.Data == null)
            return false;

        if (result.Data.Count != request.ProductsList.Count)
            return false;

        var productNames = result.Data.Select(item => item.Name).ToArray();
        
        foreach (var item in result.Data)
        {
            var product = request.ProductsList.FirstOrDefault(x => x.IdProduct == item.Id);
            var tax = item.Tax;
            debit += (int)((item.SalePrice * product!.Count) * (1 + (tax / 100)));
            points += item.BinaryPoints * product.Count;
            commissionable += item.CommissionableValue * product.Count;
            if (item.CategoryId == 2)
            {
                origin = 1;
            }

            var invoiceDetail = new InvoiceDetailsTransactionRequest
            {
                ProductId = item.Id,
                PaymentGroupId = item.PaymentGroup,
                AccumMinPurchase = Convert.ToByte(item.AcumCompMin),
                ProductName = item.Name!,
                ProductPrice = item.SalePrice,
                ProductPriceBtc = Constants.None,
                ProductIva = item.Tax,
                ProductQuantity = product.Count,
                ProductCommissionable = item.CommissionableValue,
                BinaryPoints = item.BinaryPoints,
                ProductPoints = item.ValuePoints,
                ProductDiscount = item.ProductDiscount,
                CombinationId = Constants.None,
                ProductPack = Convert.ToByte(item.ProductPacks),
                BaseAmount = (item.BaseAmount * product.Count),
                DailyPercentage = item.DailyPercentage,
                WaitingDays = item.DaysWait,
                DaysToPayQuantity = Constants.DaysToPayQuantity,
                ProductStart = Constants.None,
            };

            invoiceDetails.Add(invoiceDetail);
        }

        if (debit == 0)
            return false;

        if (invoiceDetails.Count == 0)
            return false;

        if (debit > balanceInfo.ServiceBalance.GetValueOrDefault())
            return false;

        var debitTransactionRequest = new DebitTransactionRequest
        {
            Debit = debit,
            AffiliateId = request.AffiliateId,
            UserId = Constants.AdminUserId,
            ConceptType = WalletConceptType.purchasing_pool.ToString(),
            Points = points,
            Concept = Constants.EcoPoolProductCategory,
            Commissionable = commissionable,
            Bank = Constants.ServiceBalanceModel1A,
            PaymentMethod = Constants.ServiceBalanceModel1A,
            Origin = origin,
            Level = Constants.None,
            AffiliateUserName = request.AffiliateUserName,
            AdminUserName = Constants.AdminEcosystemUserName,
            ReceiptNumber = request.ReceiptNumber,
            Type = Constants.None,
            SecretKey = request.SecretKey,
            invoices = invoiceDetails,
        };

        var spResponse = await _walletModel1ARepository.DebitServiceBalanceTransaction(debitTransactionRequest);

        if (spResponse is null)
            return false;

        var invoicePdf =
            await _mediatorPdfService.GenerateInvoice(userInfoResponse!, debitTransactionRequest, spResponse);

        var productPdfsContents = await CommonExtensions.GetPdfContentFromProductNames(productNames!);

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
}