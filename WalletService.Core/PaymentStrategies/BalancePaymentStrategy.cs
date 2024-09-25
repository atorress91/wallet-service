using WalletService.Core.Caching;
using WalletService.Core.PaymentStrategies.IPaymentStrategies;
using WalletService.Core.Services.IServices;
using WalletService.Data.Adapters.IAdapters;
using WalletService.Data.Repositories.IRepositories;
using WalletService.Models.Constants;
using WalletService.Models.DTO.BalanceInformationDto;
using WalletService.Models.Enums;
using WalletService.Models.Requests.BonusRequest;
using WalletService.Models.Requests.WalletRequest;
using WalletService.Models.Responses;
using WalletService.Utility.Extensions;
using Constants = WalletService.Models.Constants.Constants;

namespace WalletService.Core.PaymentStrategies;

public class BalancePaymentStrategy : IBalancePaymentStrategy
{
    private readonly IInventoryServiceAdapter _inventoryServiceAdapter;
    private readonly IAccountServiceAdapter   _accountServiceAdapter;
    private readonly IWalletRepository        _walletRepository;
    private readonly IEcosystemPdfService      _ecosystemPdfService;
    private readonly IBrevoEmailService       _brevoEmailService;
    private readonly IWalletRequestRepository _walletRequestRepository;
    private readonly RedisCache               _redisCache;
    private readonly IBrandService            _brandService;
    private readonly IRecyCoinPdfService      _recyCoinPdfService;
    private readonly IBonusRepository         _bonusRepository;
    public BalancePaymentStrategy(IInventoryServiceAdapter inventoryServiceAdapter,
        IAccountServiceAdapter                             accountServiceAdapter, IWalletRepository walletRepository,
        IEcosystemPdfService                                ecosystemPdfService,
        IBrevoEmailService                                 brevoEmailService, IWalletRequestRepository walletRequestRepository,
        RedisCache                                         redisCache,IBrandService brandService,
        IRecyCoinPdfService                                recyCoinPdfService,
        IBonusRepository bonusRepository)
    {
        _inventoryServiceAdapter = inventoryServiceAdapter;
        _accountServiceAdapter   = accountServiceAdapter;
        _walletRepository        = walletRepository;
        _brevoEmailService       = brevoEmailService;
        _ecosystemPdfService     = ecosystemPdfService;
        _walletRequestRepository = walletRequestRepository;
        _redisCache              = redisCache;
        _brandService            = brandService;
        _recyCoinPdfService      = recyCoinPdfService;
        _bonusRepository         = bonusRepository;
    }

    private async Task<BalanceInformationDto> GetBalanceInformationByAffiliateId(int affiliateId,int brandId)
    {
        var amountRequests    = await _walletRequestRepository.GetTotalWalletRequestAmountByAffiliateId(affiliateId, brandId);
        var availableBalance  = await _walletRepository.GetAvailableBalanceByAffiliateId(affiliateId,brandId);
        var reverseBalance    = await _walletRepository.GetReverseBalanceByAffiliateId(affiliateId,brandId);
        var totalAcquisitions = await _walletRepository.GetTotalAcquisitionsByAffiliateId(affiliateId,brandId);

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

    private async Task<bool> PayMembershipBonusToFather(CreditTransactionRequest request)
    {
        var createBonus = await _walletRepository.CreditTransaction(request);

        return createBonus;
    }
    
    public async Task<bool> ExecuteEcoPoolPayment(WalletRequest request)
    {
        var  debit          = 0;
        var  points         = 0m;
        var  commissionable = 0m;
        byte origin         = 0;

        var invoiceDetails   = new List<InvoiceDetailsTransactionRequest>();
        var userInfoResponse = await _accountServiceAdapter.GetUserInfo(request.AffiliateId,request.BrandId);
        var balanceInfo      = await GetBalanceInformationByAffiliateId(request.AffiliateId,request.BrandId);
        var productIds       = request.ProductsList.Select(p => p.IdProduct).ToArray();
        var responseList     = await _inventoryServiceAdapter.GetProductsIds(productIds,request.BrandId);

        if (!responseList.IsSuccessful)
            return false;

        if (string.IsNullOrEmpty(responseList.Content))
            return false;

        var result = responseList.Content.ToJsonObject<ProductsResponse>();

        if (result?.Data.Count == Constants.EmptyValue)
        {
            var firstProductId   = request.ProductsList.First().IdProduct;
            var membershipResult = await _inventoryServiceAdapter.GetProductById(firstProductId,request.BrandId);

            var productResponse = membershipResult.Content!.ToJsonObject<ProductResponse>();

            await _accountServiceAdapter.UpdateActivationDate(request.AffiliateId,request.BrandId);

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
                ProductStart          = Constants.EmptyValue,
                BrandId               = request.BrandId, 
            };

            invoiceDetails.Add(invoiceDetail);
        }

        if (debit == Constants.EmptyValue)
            return false;

        if (invoiceDetails.Count == Constants.EmptyValue)
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
            Level             = Constants.EmptyValue,
            AffiliateUserName = request.AffiliateUserName,
            AdminUserName     = Constants.AdminEcosystemUserName,
            ReceiptNumber     = request.ReceiptNumber,
            Type              = Constants.EmptyValue,
            SecretKey         = request.SecretKey,
            invoices          = invoiceDetails,
            Reason            = string.Empty,
            BrandId           = request.BrandId, 
        };

        var spResponse = await _walletRepository.DebitTransaction(debitTransactionRequest);

        if (spResponse is null)
            return false;
        
        if (request.BrandId == Constants.RecyCoin)
        { 
            await _bonusRepository.CreateBonus(new BonusRequest
            {
                AffiliateId = request.AffiliateId,
                Amount = (debitTransactionRequest.Debit / 2),
                InvoiceId = spResponse.Id,
                Comment = "Bonus for Recycoin"
            });
            await _walletRepository.DistributeCommissionsPerPurchaseAsync(new DistributeCommissionsRequest
            {
                AffiliateId = request.AffiliateId, InvoiceAmount = debitTransactionRequest.Debit,
                BrandId = request.BrandId
            });
        }
        
        await RemoveCacheKey(request.AffiliateId, CacheKeys.BalanceInformationModel2);
        await RemoveCacheKey(request.AffiliateId, CacheKeys.BalanceInformationModel1A);
        await RemoveCacheKey(request.AffiliateId, CacheKeys.BalanceInformationModel1B);

        byte[] invoicePdf;
        if (request.BrandId == Constants.RecyCoin)
        {
            invoicePdf = await _recyCoinPdfService.GenerateInvoice(userInfoResponse!, debitTransactionRequest, spResponse);
          
        }
        else
        {
            invoicePdf = await _ecosystemPdfService.GenerateInvoice(userInfoResponse!, debitTransactionRequest, spResponse);
        }
        
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
            await _brevoEmailService.SendEmailPurchaseConfirm(userInfoResponse!, allPdfData, spResponse,request.BrandId);
        }

        return true;
    }

    public async Task<bool> ExecuteAdminPayment(WalletRequest request)
    {
        var  debit          = 0;
        var  points         = 0m;
        var  commissionable = 0m;
        byte origin         = 0;

        var invoiceDetails   = new List<InvoiceDetailsTransactionRequest>();
        var userInfoResponse = await _accountServiceAdapter.GetUserInfo(request.AffiliateId,request.BrandId);
        var productIds       = request.ProductsList.Select(p => p.IdProduct).ToArray();
        var responseList     = await _inventoryServiceAdapter.GetProductsIds(productIds,request.BrandId);

        if (!responseList.IsSuccessful)
            return false;

        if (string.IsNullOrEmpty(responseList.Content))
            return false;

        var result = responseList.Content.ToJsonObject<ProductsResponse>();

        if (result?.Data.Count == Constants.EmptyValue)
        {
            var firstProductId   = request.ProductsList.First().IdProduct;
            var membershipResult = await _inventoryServiceAdapter.GetProductById(firstProductId,request.BrandId);

            var productResponse = membershipResult.Content!.ToJsonObject<ProductResponse>();

            await _accountServiceAdapter.UpdateActivationDate(request.AffiliateId,request.BrandId);

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
                ProductStart          = Constants.EmptyValue,
                BrandId               = request.BrandId
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
            Concept           = Constants.EcoPoolProductCategoryForAdmin,
            Commissionable    = commissionable,
            Bank              = request.Bank,
            PaymentMethod     = Constants.AdminPayment,
            Origin            = origin,
            Level             = Constants.EmptyValue,
            AffiliateUserName = request.AffiliateUserName,
            AdminUserName     = Constants.AdminEcosystemUserName,
            ReceiptNumber     = request.ReceiptNumber,
            Type              = Constants.EmptyValue,
            SecretKey         = request.SecretKey,
            invoices          = invoiceDetails,
            Reason            = string.Empty,
            BrandId           = request.BrandId
        };

        var spResponse = await _walletRepository.AdminDebitTransaction(debitTransactionRequest);

        if (spResponse is null)
            return false;
        
        await RemoveCacheKey(request.AffiliateId, CacheKeys.BalanceInformationModel2);

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

        if (allPdfData.Count > Constants.EmptyValue)
        {
            await _brevoEmailService.SendEmailPurchaseConfirm(userInfoResponse!, allPdfData, spResponse,request.BrandId);
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
        var userInfoResponse = await _accountServiceAdapter.GetUserInfo(request.AffiliateId,request.BrandId);
        var balanceInfo      = await GetBalanceInformationByAffiliateId(request.AffiliateId, request.BrandId);
        var productIds       = request.ProductsList.Select(p => p.IdProduct).ToArray();
        var responseList     = await _inventoryServiceAdapter.GetProductsIds(productIds,request.BrandId);

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
                ProductStart          = Constants.EmptyValue,
                BrandId               = request.BrandId 
            };

            invoiceDetails.Add(invoiceDetail);
        }

        if (debit == Constants.EmptyValue)
            return false;

        if (invoiceDetails.Count == Constants.EmptyValue)
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
            Level             = Constants.EmptyValue,
            AffiliateUserName = request.AffiliateUserName,
            AdminUserName     = Constants.AdminEcosystemUserName,
            ReceiptNumber     = request.ReceiptNumber,
            Type              = Constants.EmptyValue,
            SecretKey         = request.SecretKey,
            invoices          = invoiceDetails,
            Reason            = string.Empty ,
            BrandId           = request.BrandId,
        };

        var spResponse = await _walletRepository.CoursesDebitTransaction(debitTransactionRequest);

        if (spResponse is null)
            return false;
        
        await RemoveCacheKey(request.AffiliateId, CacheKeys.BalanceInformationModel2);

        var invoicePdf =
            await _ecosystemPdfService.GenerateInvoice(userInfoResponse!, debitTransactionRequest, spResponse);

        Dictionary<string, byte[]> allPdfData = new Dictionary<string, byte[]>
        {
            ["Invoice.pdf"] = invoicePdf
        };

        if (allPdfData.Count > Constants.EmptyValue)
        {
            await _brevoEmailService.SendEmailPurchaseConfirm(userInfoResponse!, allPdfData, spResponse,request.BrandId);
        }

        return true;
    }

    public async Task<bool> ExecuteCustomPayment(WalletRequest request)
    {
        var  debit          = 0m;
        var  points         = 0m;
        var  commissionable = 0m;
        byte origin         = 0;

        var invoiceDetails   = new List<InvoiceDetailsTransactionRequest>();
        var userInfoResponse = await _accountServiceAdapter.GetUserInfo(request.AffiliateId,request.BrandId);
        var productIds       = request.ProductsList.Select(p => p.IdProduct).ToArray();
        var responseList     = await _inventoryServiceAdapter.GetProductsIds(productIds,request.BrandId);

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
            item.SalePrice  = Convert.ToDecimal(request.ReceiptNumber);
            item.BaseAmount = item.SalePrice;
            item.Name       = "CustomEcoPool";

            debit          =  item.BaseAmount;
            points         += item.BinaryPoints * product!.Count;
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
                ProductStart          = Constants.EmptyValue,
                BrandId               = request.BrandId
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
            Concept           = Constants.EcoPoolProductCategoryForAdmin,
            Commissionable    = commissionable,
            Bank              = request.Bank,
            PaymentMethod     = Constants.AdminPayment,
            Origin            = origin,
            Level             = Constants.EmptyValue,
            AffiliateUserName = request.AffiliateUserName,
            AdminUserName     = Constants.AdminEcosystemUserName,
            ReceiptNumber     = request.ReceiptNumber,
            Type              = Constants.EmptyValue,
            SecretKey         = request.SecretKey,
            invoices          = invoiceDetails,
            Reason            = string.Empty ,
            BrandId           = request.BrandId,
        };

        var spResponse = await _walletRepository.AdminDebitTransaction(debitTransactionRequest);

        if (spResponse is null)
            return false;

        await RemoveCacheKey(request.AffiliateId, CacheKeys.BalanceInformationModel2);

        var invoicePdf = await _ecosystemPdfService.GenerateInvoice(userInfoResponse!, debitTransactionRequest, spResponse);

        Dictionary<string, byte[]> allPdfData = new Dictionary<string, byte[]>
        {
            ["Invoice.pdf"] = invoicePdf
        };

        if (allPdfData.Count > Constants.EmptyValue)
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
        var userInfoResponse     = await _accountServiceAdapter.GetUserInfo(request.AffiliateId,request.BrandId);
        var balanceInfo          = await GetBalanceInformationByAffiliateId(request.AffiliateId, request.BrandId);
        var affiliateBonusWinner = await _accountServiceAdapter.GetUserInfo(userInfoResponse!.Father,request.BrandId);

        var firstProductId   = request.ProductsList.First().IdProduct;
        var membershipResult = await _inventoryServiceAdapter.GetProductById(firstProductId,request.BrandId);

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
                BrandId               = request.BrandId 
            };

            invoiceDetails.Add(invoiceDetail);
        }

        if (debit > balanceInfo.AvailableBalance.GetValueOrDefault())
            return false;

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
            Bank              = request.Bank,
            PaymentMethod     = Constants.WalletBalance,
            Origin            = origin,
            Level             = Constants.EmptyValue,
            AffiliateUserName = request.AffiliateUserName,
            AdminUserName     = Constants.AdminEcosystemUserName,
            ReceiptNumber     = request.ReceiptNumber,
            Type              = Constants.EmptyValue,
            SecretKey         = request.SecretKey,
            invoices          = invoiceDetails,
            Reason            = string.Empty,
            BrandId           = request.BrandId 
        };

        var spResponse = await _walletRepository.MembershipDebitTransaction(debitTransactionRequest);

        if (spResponse is null)
            return false;
        
        await RemoveCacheKey(request.AffiliateId, CacheKeys.BalanceInformationModel2);

        await _accountServiceAdapter.UpdateActivationDate(request.AffiliateId,_brandService.BrandId);

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
        
        await RemoveCacheKey(affiliateBonusWinner.Id, CacheKeys.BalanceInformationModel2);

        await _brevoEmailService.SendBonusConfirmation(affiliateBonusWinner, request.AffiliateUserName,request.BrandId);
        var pdfResult = await _ecosystemPdfService.GenerateInvoice(userInfoResponse, debitTransactionRequest, spResponse);
        await _brevoEmailService.SendEmailWelcome(userInfoResponse, spResponse,request.BrandId);

        if (pdfResult.Length != Constants.EmptyValue)
        {
            await _brevoEmailService.SendEmailMembershipConfirm(userInfoResponse, pdfResult, spResponse,request.BrandId);
        }

        return true;
    }
    private async Task RemoveCacheKey(int affiliateId, string stringKey)
    {
        var key      = string.Format(stringKey, affiliateId);
        var existsKey = await _redisCache.KeyExists(key);
        
        if (existsKey)
            await _redisCache.Delete(key);
    }
}