using AutoMapper;
using WalletService.Core.Services;
using WalletService.Core.Services.IServices;
using WalletService.Data.Adapters.IAdapters;
using WalletService.Data.Database.Models;
using WalletService.Data.Repositories.IRepositories;
using WalletService.Models.Constants;
using WalletService.Models.DTO.BalanceInformationDto;
using WalletService.Models.Enums;
using WalletService.Models.Requests.WalletRequest;
using WalletService.Models.Requests.WalletTransactionRequest;
using WalletService.Models.Responses;
using WalletService.Utility.Extensions;

namespace WalletService.Core.PaymentStrategies;

public class ToThirdPartiesPaymentStrategy : BaseService
{
    private readonly IInventoryServiceAdapter _inventoryServiceAdapter;
    private readonly IAccountServiceAdapter   _accountServiceAdapter;
    private readonly IWalletRepository        _walletRepository;
    private readonly IEcosystemPdfService      _ecosystemPdfService;
    private readonly IBrevoEmailService       _brevoEmailService;
    private readonly IWalletRequestRepository _walletRequestRepository;
    private readonly IBrandService _brandService;
    public ToThirdPartiesPaymentStrategy(IInventoryServiceAdapter inventoryServiceAdapter,
        IAccountServiceAdapter                                    accountServiceAdapter,   IWalletRepository walletRepository, IEcosystemPdfService ecosystemPdfService,
        IBrevoEmailService                                        brevoEmailService,       IWalletRequestRepository walletRequestRepository,
        IMapper                    mapper,IBrandService brandService):base(mapper)
    {
        _inventoryServiceAdapter = inventoryServiceAdapter;
        _accountServiceAdapter   = accountServiceAdapter;
        _walletRepository        = walletRepository;
        _brevoEmailService       = brevoEmailService;
        _ecosystemPdfService      = ecosystemPdfService;
        _walletRequestRepository = walletRequestRepository;
        _brandService            = brandService;
    }
    public async Task<bool> ExecutePayment(WalletRequest request)
    {
        var  debit          = 0;
        var  points         = 0m;
        var  commissionable = 0m;
        byte origin         = 0;
        var today  = DateTime.Now;

        var          invoiceDetails   = new List<InvoiceDetailsTransactionRequest>();
        var          userInfoResponse = await _accountServiceAdapter.GetUserInfo(request.AffiliateId,_brandService.BrandId);
        var          purchaseFor      = await _accountServiceAdapter.GetUserInfo(request.PurchaseFor,_brandService.BrandId);
        var          balanceInfo      = await GetBalanceInformationByAffiliateId(request.AffiliateId);
        var          productIds       = request.ProductsList.Select(p => p.IdProduct).ToArray();
        var          responseList     = await _inventoryServiceAdapter.GetProductsIds(productIds,_brandService.BrandId);
        
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

        if (result?.Data is null)
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
                ProductDiscount       = Constants.EmptyValue,
                CombinationId         = Constants.EmptyValue,
                ProductPack           = Convert.ToByte(item.ProductPacks),
                BaseAmount            = item.BaseAmount,
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

        if (debit > balanceInfo.ReverseBalance.GetValueOrDefault())
            return false;

        var debitTransaction = new WalletTransactionRequest
        {
            Debit             = debit,
            Deferred          = Constants.EmptyValue,
            Detail            = null,
            AffiliateId       = request.AffiliateId,
            AdminUserName     = Constants.AdminEcosystemUserName,
            Status            = true,
            UserId            = Constants.AdminUserId,
            Credit            = Constants.EmptyValue,
            Concept           = "Transferencia de saldo revertido al afiliado "+ purchaseFor!.UserName,
            Support           = null!,
            Date              = today,
            Compression       = false,
            AffiliateUserName = request.AffiliateUserName,
            ConceptType       = WalletConceptType.purchase_with_reverse_balance
        };

        var creditTransaction = new WalletTransactionRequest
        {
            Debit             = Constants.EmptyValue,
            Deferred          = Constants.EmptyValue,
            Detail            = null,
            AffiliateId       = request.PurchaseFor,
            AdminUserName     = Constants.AdminEcosystemUserName,
            Status            = true,
            UserId            = Constants.AdminUserId,
            Credit            = debit,
            Concept           = "Transferencia de saldo revertido del afiliado " + userInfoResponse!.UserName,
            Support           = null!,
            Date              = today,
            Compression       = false,
            AffiliateUserName = purchaseFor.UserName,
            ConceptType       = WalletConceptType.revert_pool
        };

        var debitWallet  = Mapper.Map<Wallets>(debitTransaction);
        var creditWallet = Mapper.Map<Wallets>(creditTransaction);

        var success = await _walletRepository.CreateTransferBalance(debitWallet, creditWallet);

        if (!success)
            return false;
        
        var debitTransactionRequest = new DebitTransactionRequest
        {
            Debit             = debit,
            AffiliateId       = purchaseFor.Id,
            UserId            = Constants.AdminUserId,
            ConceptType       = WalletConceptType.purchase_with_reverse_balance.ToString(),
            Points            = points,
            Concept           = Constants.PurchasingPoolRevertBalance,
            Commissionable    = commissionable,
            Bank              = request.Bank,
            PaymentMethod     = Constants.ReverseBalance,
            Origin            = origin,
            Level             = Constants.EmptyValue,
            AffiliateUserName = purchaseFor.UserName!,
            AdminUserName     = Constants.AdminEcosystemUserName,
            ReceiptNumber     = request.ReceiptNumber,
            Type              = Constants.EmptyValue,
            SecretKey         = request.SecretKey,
            invoices          = invoiceDetails,
        };

        var spResponse = await _walletRepository.DebitTransaction(debitTransactionRequest);

        if (spResponse is null)
            return false;
        
        // var fullName                   = purchaseFor.Name + " " + purchaseFor.LastName;
        var invoicePdf = await _ecosystemPdfService.GenerateInvoice(purchaseFor, debitTransactionRequest, spResponse);
        var productPdfsContents = await CommonExtensions.GetPdfContentFromProductNames(productNames!);
        
        // await _brevoEmailService.SendEmailConfirmationEmailToThirdParty(userInfoResponse, fullName, productNames);
        
        Dictionary<string, byte[]> allPdfData = new Dictionary<string, byte[]>
        {
            ["Invoice.pdf"] = invoicePdf
        };

        foreach (var pdfDataEntry in productPdfsContents)
        {
            allPdfData[pdfDataEntry.Key] = pdfDataEntry.Value;
        }

        if (invoicePdf.Length != Constants.EmptyValue)
        {
            await _brevoEmailService.SendEmailPurchaseConfirm(purchaseFor, allPdfData, spResponse,request.BrandId);
        }

        return true;
    }

    private async Task<BalanceInformationDto> GetBalanceInformationByAffiliateId(int affiliateId)
    {
        var amountRequests    = await _walletRequestRepository.GetTotalWalletRequestAmountByAffiliateId(affiliateId,_brandService.BrandId);
        var availableBalance  = await _walletRepository.GetAvailableBalanceByAffiliateId(affiliateId,_brandService.BrandId);
        var reverseBalance    = await _walletRepository.GetReverseBalanceByAffiliateId(affiliateId,_brandService.BrandId);
        var totalAcquisitions = await _walletRepository.GetTotalAcquisitionsByAffiliateId(affiliateId,_brandService.BrandId);

        var response = new BalanceInformationDto
        {
            AvailableBalance  = availableBalance,
            ReverseBalance    = reverseBalance ?? Constants.EmptyValue,
            TotalAcquisitions = totalAcquisitions ?? Constants.EmptyValue
        };

        if (amountRequests == 0m && response.ReverseBalance == 0m) return response;

        response.AvailableBalance -= amountRequests;
        response.AvailableBalance -= response.ReverseBalance;

        return response;
    }
}