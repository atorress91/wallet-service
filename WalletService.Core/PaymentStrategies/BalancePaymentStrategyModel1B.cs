using WalletService.Core.PaymentStrategies.IPaymentStrategies;
using WalletService.Core.Services.IServices;
using WalletService.Data.Adapters.IAdapters;
using WalletService.Data.Repositories.IRepositories;
using WalletService.Models.DTO.WalletModel1BDto;
using WalletService.Models.Enums;
using WalletService.Models.Requests.WalletRequest;
using WalletService.Models.Responses;
using WalletService.Utility.Extensions;
using Constants = WalletService.Models.Constants.Constants;

namespace WalletService.Core.PaymentStrategies;

public class BalancePaymentStrategy1B : IBalancePaymentStrategyModel1B
{
    private readonly IInventoryServiceAdapter _inventoryServiceAdapter;
    private readonly IAccountServiceAdapter _accountServiceAdapter;
    private readonly IWalletModel1BRepository _walletModel1BRepository;
    private readonly IMediatorPdfService _mediatorPdfService;
    private readonly IBrevoEmailService _brevoEmailService;

    public BalancePaymentStrategy1B(IInventoryServiceAdapter inventoryServiceAdapter,
        IAccountServiceAdapter accountServiceAdapter, IWalletModel1BRepository walletModel1BRepository,
        IMediatorPdfService mediatorPdfService,
        IBrevoEmailService brevoEmailService)
    {
        _inventoryServiceAdapter = inventoryServiceAdapter;
        _accountServiceAdapter = accountServiceAdapter;
        _walletModel1BRepository = walletModel1BRepository;
        _brevoEmailService = brevoEmailService;
        _mediatorPdfService = mediatorPdfService;
    }
    
    private async Task<BalanceInformationModel1BDto> GetBalanceInformationByAffiliateId(int affiliateId)
    {
        var availableBalance = await _walletModel1BRepository.GetAvailableBalanceByAffiliateId(affiliateId);
        var reverseBalance = await _walletModel1BRepository.GetReverseBalanceByAffiliateId(affiliateId);
        var totalAcquisitions = await _walletModel1BRepository.GetTotalAcquisitionsByAffiliateId(affiliateId);
        var serviceBalance = await _walletModel1BRepository.GetTotalServiceBalance(affiliateId);

        var response = new BalanceInformationModel1BDto
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
                ProductPriceBtc = Constants.EmptyValue,
                ProductIva = item.Tax,
                ProductQuantity = product.Count,
                ProductCommissionable = item.CommissionableValue,
                BinaryPoints = item.BinaryPoints,
                ProductPoints = item.ValuePoints,
                ProductDiscount = item.ProductDiscount,
                CombinationId = Constants.EmptyValue,
                ProductPack = Convert.ToByte(item.ProductPacks),
                BaseAmount = (item.BaseAmount * product.Count),
                DailyPercentage = item.DailyPercentage,
                WaitingDays = item.DaysWait,
                DaysToPayQuantity = Constants.DaysToPayQuantity,
                ProductStart = Constants.EmptyValue,
            };

            invoiceDetails.Add(invoiceDetail);
        }

        if (debit == Constants.EmptyValue)
            return false;

        if (invoiceDetails.Count == Constants.EmptyValue)
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
            PaymentMethod = Constants.WalletModel1BBalance,
            Origin = origin,
            Level = Constants.EmptyValue,
            AffiliateUserName = request.AffiliateUserName,
            AdminUserName = Constants.AdminEcosystemUserName,
            ReceiptNumber = request.ReceiptNumber,
            Type = Constants.EmptyValue,
            SecretKey = request.SecretKey,
            invoices = invoiceDetails,
        };

        var spResponse = await _walletModel1BRepository.DebitTransaction(debitTransactionRequest);

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

        if (allPdfData.Count > Constants.EmptyValue)
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
                ProductPriceBtc = Constants.EmptyValue,
                ProductIva = item.Tax,
                ProductQuantity = product.Count,
                ProductCommissionable = item.CommissionableValue,
                BinaryPoints = item.BinaryPoints,
                ProductPoints = item.ValuePoints,
                ProductDiscount = item.ProductDiscount,
                CombinationId = Constants.EmptyValue,
                ProductPack = Convert.ToByte(item.ProductPacks),
                BaseAmount = (item.BaseAmount * product.Count),
                DailyPercentage = item.DailyPercentage,
                WaitingDays = item.DaysWait,
                DaysToPayQuantity = Constants.DaysToPayQuantity,
                ProductStart = Constants.EmptyValue,
            };

            invoiceDetails.Add(invoiceDetail);
        }

        if (debit == Constants.EmptyValue)
            return false;

        if (invoiceDetails.Count == Constants.EmptyValue)
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
            Bank = Constants.ServiceBalanceModel1B,
            PaymentMethod = Constants.ServiceBalanceModel1B,
            Origin = origin,
            Level = Constants.EmptyValue,
            AffiliateUserName = request.AffiliateUserName,
            AdminUserName = Constants.AdminEcosystemUserName,
            ReceiptNumber = request.ReceiptNumber,
            Type = Constants.EmptyValue,
            SecretKey = request.SecretKey,
            invoices = invoiceDetails,
        };

        var spResponse = await _walletModel1BRepository.DebitServiceBalanceTransaction(debitTransactionRequest);

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

        if (allPdfData.Count > Constants.EmptyValue)
        {
            await _brevoEmailService.SendEmailPurchaseConfirm(userInfoResponse!, allPdfData, spResponse);
        }

        return true;
    }
}