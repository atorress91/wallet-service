using System.Reflection;
using System.Text.Json;
using WalletService.Core.PaymentStrategies.IPaymentStrategies;
using WalletService.Core.Services.IServices;
using WalletService.Data.Adapters.IAdapters;
using WalletService.Data.Repositories.IRepositories;
using WalletService.Models.Constants;
using WalletService.Models.DTO.BalanceInformationDto;
using WalletService.Models.Enums;
using WalletService.Models.Requests.WalletRequest;
using WalletService.Models.Responses;

namespace WalletService.Core.PaymentStrategies;

public class MembershipPaymentStrategy : IMembershipPaymentStrategy
{
    private readonly IInventoryServiceAdapter _inventoryServiceAdapter;
    private readonly IAccountServiceAdapter   _accountServiceAdapter;
    private readonly IWalletRepository        _walletRepository;
    private readonly IMediatorPdfService      _mediatorPdfService;
    private readonly IBrevoEmailService       _brevoEmailService;
    private readonly IWalletRequestRepository _walletRequestRepository;

    public MembershipPaymentStrategy(IInventoryServiceAdapter inventoryServiceAdapter,
        IAccountServiceAdapter                                accountServiceAdapter, IWalletRepository walletRepository, IMediatorPdfService mediatorPdfService,
        IBrevoEmailService                                    brevoEmailService,     IWalletRequestRepository walletRequestRepository)
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

        var invoiceDetails       = new List<InvoiceDetailsTransactionRequest>();
        var userInfoResponse     = await _accountServiceAdapter.GetUserInfo(request.AffiliateId);
        var balanceInfo          = await GetBalanceInformationByAffiliateId(request.AffiliateId);
        var affiliateBonusWinner = await _accountServiceAdapter.GetUserInfo(userInfoResponse!.Father);

        var firstProductId   = request.ProductsList.First().IdProduct;
        var membershipResult = await _inventoryServiceAdapter.GetProductById(firstProductId);

        var productResponse = JsonSerializer.Deserialize<ProductResponse>(membershipResult.Content!);

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
                ProductPriceBtc       = Constants.None,
                ProductIva            = item.Tax,
                ProductQuantity       = product.Count,
                ProductCommissionable = item.CommissionableValue,
                BinaryPoints          = item.BinaryPoints,
                ProductPoints         = item.ValuePoints,
                ProductDiscount       = Constants.None,
                CombinationId         = Constants.None,
                ProductPack           = Convert.ToByte(item.ProductPacks),
                BaseAmount            = item.BaseAmount,
                DailyPercentage       = item.DailyPercentage,
                WaitingDays           = item.DaysWait,
                DaysToPayQuantity     = Constants.None,
                ProductStart          = Constants.None,
            };

            invoiceDetails.Add(invoiceDetail);
        }

        if (debit > balanceInfo.AvailableBalance.GetValueOrDefault())
            return false;

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
            Concept           = Constants.Membership,
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

        var spResponse = await _walletRepository.MembershipDebitTransaction(debitTransactionRequest);

        if (spResponse is null)
            return false;

        await _accountServiceAdapter.UpdateActivationDate(request.AffiliateId);

        var creditTransactionForWinningBonus = new CreditTransactionRequest
        {
            AffiliateId       = affiliateBonusWinner!.Id,
            UserId            = Constants.AdminUserId,
            Concept           = Constants.CommissionMembership + ' ' + request.AffiliateUserName,
            Credit            = Constants.MembershipBonus,
            AffiliateUserName = affiliateBonusWinner.UserName,
            ConceptType       = WalletConceptType.membership_bonus.ToString(),
            AdminUserName     = Constants.AdminEcosystemUserName
        };

        var bonusPaymentResult = await PayMembershipBonusToFather(creditTransactionForWinningBonus);

        if (bonusPaymentResult is false)
            return false;

        await _brevoEmailService.SendBonusConfirmation(affiliateBonusWinner, request.AffiliateUserName);

        var pdfResult = await _mediatorPdfService.GenerateInvoice(userInfoResponse, debitTransactionRequest, spResponse);

        await _brevoEmailService.SendEmailWelcome(userInfoResponse, spResponse);

        if (pdfResult.Length != 0)
        {
            await _brevoEmailService.SendEmailMembershipConfirm(userInfoResponse, pdfResult, spResponse);
        }

        return true;
    }

    private async Task<bool> PayMembershipBonusToFather(CreditTransactionRequest request)
    {
        var createBonus = await _walletRepository.CreditTransaction(request);

        return createBonus;
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

    public async Task<bool> ExecuteMembershipPayment(WalletRequest request)
    {
        var  debit          = 0;
        var  points         = 0m;
        var  commissionable = 0m;
        byte origin         = 0;

        var invoiceDetails       = new List<InvoiceDetailsTransactionRequest>();
        var userInfoResponse     = await _accountServiceAdapter.GetUserInfo(request.AffiliateId);
        var affiliateBonusWinner = await _accountServiceAdapter.GetUserInfo(userInfoResponse!.Father);

        var firstProductId   = request.ProductsList.First().IdProduct;
        var membershipResult = await _inventoryServiceAdapter.GetProductById(firstProductId);

        var productResponse = JsonSerializer.Deserialize<ProductResponse>(membershipResult.Content!);

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
                ProductPriceBtc       = Constants.None,
                ProductIva            = item.Tax,
                ProductQuantity       = product.Count,
                ProductCommissionable = item.CommissionableValue,
                BinaryPoints          = item.BinaryPoints,
                ProductPoints         = item.ValuePoints,
                ProductDiscount       = Constants.None,
                CombinationId         = Constants.None,
                ProductPack           = Convert.ToByte(item.ProductPacks),
                BaseAmount            = item.BaseAmount,
                DailyPercentage       = item.DailyPercentage,
                WaitingDays           = item.DaysWait,
                DaysToPayQuantity     = Constants.None,
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
            Concept           = Constants.Membership,
            Commissionable    = commissionable,
            Bank              = Constants.CoinPayments,
            PaymentMethod     = Constants.CoinPayments,
            Origin            = origin,
            Level             = Constants.None,
            AffiliateUserName = request.AffiliateUserName,
            AdminUserName     = Constants.AdminEcosystemUserName,
            ReceiptNumber     = request.ReceiptNumber,
            Type              = Constants.None,
            SecretKey         = request.SecretKey,
            invoices          = invoiceDetails,
        };

        var spResponse = await _walletRepository.HandleMembershipTransaction(debitTransactionRequest);

        if (spResponse is null)
            return false;

        await _accountServiceAdapter.UpdateActivationDate(request.AffiliateId);

        var pdfResult = await _mediatorPdfService.GenerateInvoice(userInfoResponse, debitTransactionRequest, spResponse);

        await _brevoEmailService.SendEmailWelcome(userInfoResponse, spResponse);

        if (pdfResult.Length != 0)
        {
            await _brevoEmailService.SendEmailMembershipConfirm(userInfoResponse, pdfResult, spResponse);
        }

        return true;
    }
    
}