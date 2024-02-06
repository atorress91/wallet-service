using System.Text.Json;
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
    private readonly IMediatorPdfService      _mediatorPdfService;
    private readonly IBrevoEmailService       _brevoEmailService;
    private readonly IWalletRequestRepository _walletRequestRepository;

    public ToThirdPartiesPaymentStrategy(IInventoryServiceAdapter inventoryServiceAdapter,
        IAccountServiceAdapter                                    accountServiceAdapter,   IWalletRepository walletRepository, IMediatorPdfService mediatorPdfService,
        IBrevoEmailService                                        brevoEmailService,       IWalletRequestRepository walletRequestRepository,IMapper                    mapper):base(mapper)
    {
        _inventoryServiceAdapter = inventoryServiceAdapter;
        _accountServiceAdapter   = accountServiceAdapter;
        _walletRepository        = walletRepository;
        _brevoEmailService       = brevoEmailService;
        _mediatorPdfService      = mediatorPdfService;
        _walletRequestRepository = walletRequestRepository;
    }
    public async Task<bool> ExecutePayment(WalletRequest request)
    {
        var  debit          = 0;
        var  points         = 0m;
        var  commissionable = 0m;
        byte origin         = 0;
        var today  = DateTime.Now;

        var          invoiceDetails   = new List<InvoiceDetailsTransactionRequest>();
        var          userInfoResponse = await _accountServiceAdapter.GetUserInfo(request.AffiliateId);
        var          purchaseFor      = await _accountServiceAdapter.GetUserInfo(request.PurchaseFor);
        var          balanceInfo      = await GetBalanceInformationByAffiliateId(request.AffiliateId);
        var          productIds       = request.ProductsList.Select(p => p.IdProduct).ToArray();
        var          responseList     = await _inventoryServiceAdapter.GetProductsIds(productIds);
   

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
                ProductPriceBtc       = 0,
                ProductIva            = item.Tax,
                ProductQuantity       = product.Count,
                ProductCommissionable = item.CommissionableValue,
                BinaryPoints          = item.BinaryPoints,
                ProductPoints         = item.ValuePoints,
                ProductDiscount       = 0,
                CombinationId         = 0,
                ProductPack           = Convert.ToByte(item.ProductPacks),
                BaseAmount            = item.BaseAmount,
                DailyPercentage       = item.DailyPercentage,
                WaitingDays           = item.DaysWait,
                DaysToPayQuantity     = Constants.DaysToPayQuantity,
                ProductStart          = 0
            };

            invoiceDetails.Add(invoiceDetail);
        }

        if (debit == 0)
            return false;

        if (invoiceDetails.Count == 0)
            return false;

        if (debit > balanceInfo.ReverseBalance.GetValueOrDefault())
            return false;

        var debitTransaction = new WalletTransactionRequest
        {
            Debit             = debit,
            Deferred          = Constants.None,
            Detail            = null,
            AffiliateId       = request.AffiliateId,
            AdminUserName     = Constants.AdminEcosystemUserName,
            Status            = true,
            UserId            = Constants.AdminUserId,
            Credit            = Constants.None,
            Concept           = "Transferencia de saldo revertido al afiliado "+ purchaseFor!.UserName,
            Support           = null!,
            Date              = today,
            Compression       = false,
            AffiliateUserName = request.AffiliateUserName,
            ConceptType       = WalletConceptType.purchase_with_reverse_balance
        };

        var creditTransaction = new WalletTransactionRequest
        {
            Debit             = Constants.None,
            Deferred          = Constants.None,
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
            Level             = 0,
            AffiliateUserName = purchaseFor.UserName!,
            AdminUserName     = Constants.AdminEcosystemUserName,
            ReceiptNumber     = request.ReceiptNumber,
            Type              = 0,
            SecretKey         = request.SecretKey,
            invoices          = invoiceDetails,
        };

        var spResponse = await _walletRepository.DebitTransaction(debitTransactionRequest);

        if (spResponse is null)
            return false;
        
        var fullName                   = purchaseFor.Name + " " + purchaseFor.LastName;
        var invoicePdf = await _mediatorPdfService.GenerateInvoice(purchaseFor, debitTransactionRequest, spResponse);
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

        if (invoicePdf.Length != 0)
        {
            await _brevoEmailService.SendEmailPurchaseConfirm(purchaseFor, allPdfData, spResponse);
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
}