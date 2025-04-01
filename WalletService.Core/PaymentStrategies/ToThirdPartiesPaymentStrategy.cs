using AutoMapper;
using WalletService.Core.Caching;
using WalletService.Core.Services;
using WalletService.Core.Services.IServices;
using WalletService.Data.Adapters.IAdapters;
using WalletService.Data.Database.Models;
using WalletService.Data.Repositories.IRepositories;
using WalletService.Models.Constants;
using WalletService.Models.DTO.BalanceInformationDto;
using WalletService.Models.Enums;
using WalletService.Models.Requests.BonusRequest;
using WalletService.Models.Requests.WalletRequest;
using WalletService.Models.Requests.WalletTransactionRequest;
using WalletService.Models.Responses;
using WalletService.Utility.Extensions;

namespace WalletService.Core.PaymentStrategies;

public class ToThirdPartiesPaymentStrategy : BaseService
{
    private readonly IInventoryServiceAdapter _inventoryServiceAdapter;
    private readonly IAccountServiceAdapter _accountServiceAdapter;
    private readonly IWalletRepository _walletRepository;
    private readonly IEcosystemPdfService _ecosystemPdfService;
    private readonly IBrevoEmailService _brevoEmailService;
    private readonly IWalletRequestRepository _walletRequestRepository;
    private readonly IBonusRepository _bonusRepository;
    private readonly IRecyCoinPdfService _recyCoinPdfService;
    private readonly RedisCache _redisCache;
    private readonly IHouseCoinPdfService _houseCoinPdfService;

    public ToThirdPartiesPaymentStrategy(IInventoryServiceAdapter inventoryServiceAdapter,
        IAccountServiceAdapter accountServiceAdapter, IWalletRepository walletRepository,
        IEcosystemPdfService ecosystemPdfService, RedisCache redisCache,
        IBrevoEmailService brevoEmailService, IWalletRequestRepository walletRequestRepository,
        IMapper mapper, IBonusRepository bonusRepository, IRecyCoinPdfService recyCoinPdfService,
        IHouseCoinPdfService houseCoinPdfService) : base(mapper)
    {
        _inventoryServiceAdapter = inventoryServiceAdapter;
        _accountServiceAdapter = accountServiceAdapter;
        _walletRepository = walletRepository;
        _brevoEmailService = brevoEmailService;
        _ecosystemPdfService = ecosystemPdfService;
        _walletRequestRepository = walletRequestRepository;
        _bonusRepository = bonusRepository;
        _recyCoinPdfService = recyCoinPdfService;
        _redisCache = redisCache;
        _houseCoinPdfService = houseCoinPdfService;
    }

    public async Task<bool> ExecutePayment(WalletRequest request)
    {
        var debit = 0;
        var points = 0m;
        var commissionable = 0m;
        byte origin = 0;
        var today = DateTime.Now;

        var invoiceDetails = new List<InvoiceDetailsTransactionRequest>();
        var userInfoResponse = await _accountServiceAdapter.GetUserInfo(request.AffiliateId, request.BrandId);
        var purchaseFor = await _accountServiceAdapter.GetUserInfo(request.PurchaseFor, request.BrandId);
        var balanceInfo = await GetBalanceInformationByAffiliateId(request.AffiliateId, request.BrandId);
        var productIds = request.ProductsList.Select(p => p.IdProduct).ToArray();
        var responseList = await _inventoryServiceAdapter.GetProductsIds(productIds, request.BrandId);

        if (!responseList.IsSuccessful)
            return false;

        if (string.IsNullOrEmpty(responseList.Content))
            return false;

        var result = responseList.Content.ToJsonObject<ProductsResponse>();

        if (result?.Data.Count == Constants.EmptyValue)
        {
            var firstProductId = request.ProductsList.First().IdProduct;
            var membershipResult = await _inventoryServiceAdapter.GetProductById(firstProductId, request.BrandId);

            var productResponse = membershipResult.Content!.ToJsonObject<ProductResponse>();

            await _accountServiceAdapter.UpdateActivationDate(request.AffiliateId, request.BrandId);

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
                AccumMinPurchase = item.AcumCompMin,
                ProductName = item.Name!,
                ProductPrice = item.SalePrice,
                ProductPriceBtc = Constants.EmptyValue,
                ProductIva = item.Tax,
                ProductQuantity = product.Count,
                ProductCommissionable = item.CommissionableValue,
                BinaryPoints = item.BinaryPoints,
                ProductPoints = item.ValuePoints,
                ProductDiscount = Constants.EmptyValue,
                CombinationId = Constants.EmptyValue,
                ProductPack = item.ProductPacks,
                BaseAmount = item.BaseAmount,
                DailyPercentage = item.DailyPercentage,
                WaitingDays = item.DaysWait,
                DaysToPayQuantity = Constants.DaysToPayQuantity,
                ProductStart = false,
                BrandId = request.BrandId
            };

            invoiceDetails.Add(invoiceDetail);
        }

        if (debit == Constants.EmptyValue)
            return false;

        if (invoiceDetails.Count == Constants.EmptyValue)
            return false;

        if (debit > balanceInfo.AvailableBalance.GetValueOrDefault())
            return false;

        var debitTransaction = new WalletTransactionRequest
        {
            Debit = debit,
            Deferred = Constants.EmptyValue,
            Detail = null,
            AffiliateId = request.AffiliateId,
            AdminUserName = request.BrandId switch
            {
                1 => Constants.AdminEcosystemUserName,
                2 => Constants.RecycoinAdmin,
                3 => Constants.HouseCoinAdmin,
                _ => Constants.AdminEcosystemUserName
            },
            Status = true,
            UserId = Constants.AdminUserId,
            Credit = Constants.EmptyValue,
            Concept = "Transferencia para compra al afiliado " + purchaseFor!.UserName,
            Support = null!,
            Date = today,
            Compression = false,
            AffiliateUserName = request.AffiliateUserName,
            ConceptType = WalletConceptType.balance_transfer,
            BrandId = request.BrandId
        };

        var creditTransaction = new WalletTransactionRequest
        {
            Debit = Constants.EmptyValue,
            Deferred = Constants.EmptyValue,
            Detail = null,
            AffiliateId = request.PurchaseFor,
            AdminUserName = request.BrandId == 1 ? Constants.AdminEcosystemUserName : Constants.RecycoinAdmin,
            Status = true,
            UserId = Constants.AdminUserId,
            Credit = debit,
            Concept = "Transferencia para compra del afiliado " + userInfoResponse!.UserName,
            Support = null!,
            Date = today,
            Compression = false,
            AffiliateUserName = purchaseFor.UserName,
            ConceptType = WalletConceptType.balance_transfer,
            BrandId = request.BrandId
        };

        var debitWallet = Mapper.Map<Wallet>(debitTransaction);
        var creditWallet = Mapper.Map<Wallet>(creditTransaction);

        var success = await _walletRepository.CreateTransferBalance(debitWallet, creditWallet);

        if (!success)
            return false;

        var debitTransactionRequest = new DebitTransactionRequest
        {
            Debit = debit,
            AffiliateId = purchaseFor.Id,
            UserId = Constants.AdminUserId,
            ConceptType = WalletConceptType.purchasing_pool.ToString(),
            Points = points,
            Concept = Constants.EcoPoolProductCategory,
            Commissionable = commissionable,
            Bank = request.Bank,
            PaymentMethod = Constants.WalletBalance,
            Origin = origin,
            Level = Constants.EmptyValue,
            AffiliateUserName = purchaseFor.UserName!,
            AdminUserName = request.BrandId == 1 ? Constants.AdminEcosystemUserName : Constants.RecycoinAdmin,
            ReceiptNumber = request.ReceiptNumber,
            Type = true,
            SecretKey = request.SecretKey,
            invoices = invoiceDetails,
            Reason = string.Empty,
            BrandId = request.BrandId,
        };

        var spResponse = await _walletRepository.DebitTransaction(debitTransactionRequest);

        if (spResponse is null)
            return false;

        if (request.BrandId == Constants.RecyCoin)
        {
            // await _bonusRepository.CreateBonus(new BonusRequest
            // {
            //     AffiliateId = request.PurchaseFor,
            //     Amount = (debitTransactionRequest.Debit / 2),
            //     InvoiceId = spResponse.Id,
            //     Comment = "Bonus for Recycoin"
            // });
            await _walletRepository.DistributeCommissionsPerPurchaseAsync(new DistributeCommissionsRequest
            {
                AffiliateId = request.PurchaseFor,
                InvoiceAmount = debitTransactionRequest.Debit,
                BrandId = request.BrandId,
                AdminUserName = Constants.RecycoinAdmin,
                LevelPercentages = [8.0m,5.0m,4.0m,2.0m,1.0m]
            });
        }

        if (request.BrandId == Constants.HouseCoin)
        {
            await _walletRepository.DistributeCommissionsPerPurchaseAsync(new DistributeCommissionsRequest
            {
                AffiliateId = request.PurchaseFor, InvoiceAmount = debitTransactionRequest.Debit,
                BrandId = request.BrandId,
                AdminUserName = Constants.HouseCoinAdmin,
                LevelPercentages = [8.0m, 6.0m, 5.0m, 4.0m, 2.0m],
            });
        }

        await RemoveCacheKey(request.AffiliateId, CacheKeys.BalanceInformationModel2);
        await RemoveCacheKey(request.AffiliateId, CacheKeys.BalanceInformationModel1A);
        await RemoveCacheKey(request.AffiliateId, CacheKeys.BalanceInformationModel1B);

        byte[] invoicePdf;
        if (request.BrandId == Constants.RecyCoin)
        {
            invoicePdf =
                await _recyCoinPdfService.GenerateInvoice(userInfoResponse, debitTransactionRequest, spResponse);
        }
        else if (request.BrandId == Constants.HouseCoin)
        {
            invoicePdf =
                await _houseCoinPdfService.GenerateInvoice(userInfoResponse, debitTransactionRequest, spResponse);
        }
        else
        {
            invoicePdf =
                await _ecosystemPdfService.GenerateInvoice(userInfoResponse, debitTransactionRequest, spResponse);
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

        if (invoicePdf.Length != Constants.EmptyValue)
        {
            await _brevoEmailService.SendEmailPurchaseConfirm(purchaseFor, allPdfData, spResponse, request.BrandId);
        }

        return true;
    }

    private async Task<BalanceInformationDto> GetBalanceInformationByAffiliateId(int affiliateId, long brandId)
    {
        var amountRequests =
            await _walletRequestRepository.GetTotalWalletRequestAmountByAffiliateId(affiliateId, brandId);
        var availableBalance = await _walletRepository.GetAvailableBalanceByAffiliateId(affiliateId, brandId);
        var reverseBalance = await _walletRepository.GetReverseBalanceByAffiliateId(affiliateId, brandId);
        var totalAcquisitions = await _walletRepository.GetTotalAcquisitionsByAffiliateId(affiliateId, brandId);

        var response = new BalanceInformationDto
        {
            AvailableBalance = availableBalance,
            ReverseBalance = reverseBalance ?? Constants.EmptyValue,
            TotalAcquisitions = totalAcquisitions ?? Constants.EmptyValue
        };

        if (amountRequests == 0m && response.ReverseBalance == 0m) return response;

        response.AvailableBalance -= amountRequests;
        response.AvailableBalance -= response.ReverseBalance;

        return response;
    }

    private async Task RemoveCacheKey(int affiliateId, string stringKey)
    {
        var key = string.Format(stringKey, affiliateId);
        var existsKey = await _redisCache.KeyExists(key);

        if (existsKey)
            await _redisCache.Delete(key);
    }
}