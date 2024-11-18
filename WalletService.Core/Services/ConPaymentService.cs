using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Globalization;
using Microsoft.IdentityModel.Tokens;
using WalletService.Core.Caching;
using WalletService.Core.PaymentStrategies.IPaymentStrategies;
using WalletService.Core.Services.IServices;
using WalletService.Data.Adapters.IAdapters;
using WalletService.Data.Database.CustomModels;
using WalletService.Data.Database.Models;
using WalletService.Data.Repositories.IRepositories;
using WalletService.Models.Configuration;
using WalletService.Models.Constants;
using WalletService.Models.Enums;
using WalletService.Models.Requests.ConPaymentRequest;
using WalletService.Models.Requests.WalletRequest;
using WalletService.Models.Responses;
using WalletService.Models.Responses.BaseResponses;
using WalletService.Utility.Extensions;


namespace WalletService.Core.Services;

public class ConPaymentService : BaseService, IConPaymentService
{
    private readonly string _merchantId;
    private readonly ApplicationConfiguration _appSettings;
    private readonly ConPaymentsApi.ConPaymentsApi _conPaymentsApi;
    private readonly ICoinPaymentTransactionRepository _coinPaymentTransactionRepository;
    private readonly IInvoiceRepository _invoiceRepository;
    private readonly IAccountServiceAdapter _accountServiceAdapter;
    private readonly ILogger<ConPaymentService> _logger;
    private readonly IWalletRepository _walletRepository;
    private readonly IBrevoEmailService _brevoEmailService;
    private readonly IWalletRequestRepository _walletRequestRepository;
    private readonly IInventoryServiceAdapter _inventoryServiceAdapter;
    private readonly ICoinPaymentsPaymentStrategy _coinPaymentsPaymentStrategy;
    private readonly RedisCache _redisCache;
    private readonly IBrandService _brandService;

    public ConPaymentService(
        IMapper mapper, IOptions<ApplicationConfiguration> appSettings,
        ICoinPaymentTransactionRepository coinPaymentTransactionRepository,
        IInvoiceRepository invoiceRepository,
        ILogger<ConPaymentService> logger,
        IAccountServiceAdapter accountServiceAdapter,
        IWalletRepository walletRepository,
        IBrevoEmailService brevoEmailService,
        IWalletRequestRepository walletRequestRepository,
        IInventoryServiceAdapter inventoryServiceAdapter,
        ICoinPaymentsPaymentStrategy coinPaymentsPaymentStrategy,
        RedisCache redisCache,
        IBrandService brandService
    ) : base(mapper)
    {
        _invoiceRepository = invoiceRepository;
        _appSettings = appSettings.Value;
        _conPaymentsApi =
            new ConPaymentsApi.ConPaymentsApi(_appSettings.ConPayments!.Secret, _appSettings.ConPayments.Key);
        _merchantId = _appSettings.ConPayments.MerchantId;
        _coinPaymentTransactionRepository = coinPaymentTransactionRepository;
        _logger = logger;
        _accountServiceAdapter = accountServiceAdapter;
        _walletRepository = walletRepository;
        _brevoEmailService = brevoEmailService;
        _walletRequestRepository = walletRequestRepository;
        _inventoryServiceAdapter = inventoryServiceAdapter;
        _coinPaymentsPaymentStrategy = coinPaymentsPaymentStrategy;
        _redisCache = redisCache;
        _brandService = brandService;
    }

    public async Task<GetPayByNameProfileResponse> GetPayByNameProfile(string nameTag)
    {
        _logger.LogInformation($"[ConPaymentService] | GetPayByNameProfile | Start | nameTag: {nameTag}");
        SortedList<string, string> parms = new SortedList<string, string>
        {
            { "pbntag", nameTag }
        };

        IRestResponse restResponse = await _conPaymentsApi.CallApi("get_pbn_info", parms);

        _logger.LogInformation($"[ConPaymentService] | GetPayByNameProfile | Response: {restResponse.ToJsonString()}");

        if (restResponse.IsSuccessful)
        {
            var result = restResponse.Content!.ToJsonObject<GetPayByNameProfileResponse>();
            return result!;
        }
        else
        {
            throw new Exception($"Error calling API: {restResponse.StatusDescription}");
        }
    }

    public async Task<GetDepositAddressResponse?> GetDepositAddress(string currency)
    {
        SortedList<string, string> parms = new SortedList<string, string>
        {
            { "currency", currency }
        };

        IRestResponse restResponse = await _conPaymentsApi.CallApi("get_deposit_address", parms);

        if (restResponse.IsSuccessful)
        {
            var result = restResponse.Content!.ToJsonObject<GetDepositAddressResponse>()!;
            return result;
        }
        else
        {
            throw new Exception($"Error calling API: {restResponse.StatusDescription}");
        }
    }

    public async Task<GetCoinBalancesResponse> GetCoinBalances(bool includeZeroBalances = false)
    {
        var parms = new SortedList<string, string>();
        if (includeZeroBalances)
        {
            parms.Add("all", "1");
        }

        var restResponse = await _conPaymentsApi.CallApi("balances", parms);

        if (restResponse.IsSuccessful)
        {
            var result = restResponse.Content!.ToJsonObject<GetCoinBalancesResponse>();
            return result!;
        }
        else
        {
            throw new Exception($"Error calling API: {restResponse.StatusDescription}");
        }
    }

    public async Task<CreateConPaymentsTransactionResponse?> CreatePayment(ConPaymentRequest request)
    {
        _logger.LogInformation($"[ConPaymentService] | CreatePayment | Start | request: {request.ToJsonString()}");
        SetRequestDefaults(request);
        var parms = ConfigureParms(request);

        var restResponse = await CallApiAsync("create_transaction", parms);
        _logger.LogInformation(
            $"[ConPaymentService] | CreatePayment | Response Api | response: {restResponse.ToJsonString()}");

        if (!restResponse.IsSuccessful)
        {
            throw new Exception($"Error calling API: {restResponse.StatusDescription}");
        }

        return await HandleSuccessfulResponse(restResponse, request);
    }

    private void SetRequestDefaults(ConPaymentRequest request)
    {
        request.Address = Constants.ConPaymentAddress;
        request.Currency1 = Constants.ConPaymentCurrency;
        request.Currency2 = Constants.ConPaymentCurrency;

        if (request.Products.Count > 0)
        {
            request.Custom = JsonConvert.SerializeObject(request.Products);
        }
    }

    private SortedList<string, string> ConfigureParms(ConPaymentRequest request)
    {
        var parms = new SortedList<string, string>
        {
            { "cmd", "create_transaction" },
            { "amount", request.Amount.ToString(CultureInfo.InvariantCulture) },
            { "currency1", request.Currency1 },
            { "currency2", request.Currency2 },
            { "buyer_email", request.BuyerEmail },
            { "address", request.Address }
        };

        if (request.BuyerName != null) parms.Add("buyer_name", request.BuyerName);
        if (request.ItemName != null) parms.Add("item_name", request.ItemName);
        if (request.ItemNumber != null) parms.Add("item_number", request.ItemNumber);
        if (request.Invoice != null) parms.Add("invoice", request.Invoice);
        if (request.Custom != null) parms.Add("custom", request.Custom);
        if (request.IpnUrl != null) parms.Add("ipn_url", request.IpnUrl);
        if (request.SuccessUrl != null) parms.Add("success_url", request.SuccessUrl);
        if (request.CancelUrl != null) parms.Add("cancel_url", request.CancelUrl);

        return parms;
    }

    private async Task<IRestResponse> CallApiAsync(string endpoint, SortedList<string, string> parms)
    {
        return await _conPaymentsApi.CallApi(endpoint, parms);
    }

    private async Task<CreateConPaymentsTransactionResponse> HandleSuccessfulResponse(IRestResponse restResponse,
        ConPaymentRequest request)
    {
        var result = JsonConvert.DeserializeObject<CreateConPaymentsTransactionResponse>(restResponse.Content!);

        var paymentTransaction = new CoinpaymentTransaction
        {
            IdTransaction = result!.Result!.Txn_Id!,
            AffiliateId = Convert.ToInt32(request.ItemNumber),
            Amount = request.Amount,
            AmountReceived = 0,
            Products = request.Custom!,
            Acredited = false,
            Status = 0,
            PaymentMethod = "Coinpayment",
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now,
            BrandId = _brandService.BrandId
        };

        await _coinPaymentTransactionRepository.CreateCoinPaymentTransaction(paymentTransaction);

        return result;
    }

    public async Task<GetTransactionInfoResponse> GetTransactionInfo(string idTransaction, bool full = false)
    {
        var parms = new SortedList<string, string>();


        parms.Add("txid", idTransaction);

        if (full)
            parms.Add("full", "1");

        var restResponse = await _conPaymentsApi.CallApi("get_tx_info", parms);

        if (restResponse.IsSuccessful)
        {
            var result = restResponse.Content!.ToJsonObject<GetTransactionInfoResponse>();
            return result!;
        }
        else
            throw new Exception($"Error calling API: {restResponse.StatusDescription}");
    }

    public async Task<string> ProcessIpnAsync(IpnRequest ipnRequest, IHeaderDictionary headers)
    {
        _logger.LogInformation($"[ConPaymentService] | ProcessIpnAsync | IpnRequest: {ipnRequest.ToJsonString()} | headers: {headers.ToJsonString()}");
        if (!IsRequestValid(ipnRequest, headers))
            return "Invalid IPN Request";

        var transactionResult = await _coinPaymentTransactionRepository.GetTransactionByTxnId(ipnRequest.txn_id);
        
        if (transactionResult is null)
            return "Transaction not found";

        _logger.LogInformation(
            $"[ConPaymentService] | ProcessIpnAsync | transactionResult: {transactionResult.ToJsonString()}");

        if (transactionResult.Status == 100)
            return "Transaction cannot be updated";

        transactionResult.Status = ipnRequest.status;
        transactionResult.AmountReceived = ipnRequest.received_amount;

        if (ipnRequest.status == -1)
        {
            await HandleCancelledTransaction(transactionResult, ipnRequest, transactionResult.Products);
            return "Transaction is delete";
        }

        if (!transactionResult.Acredited)
        {
            await HandlePaymentTransaction(transactionResult, ipnRequest);
        }

        if (ipnRequest.status == 100)
        {
            await GrantWelcomeBonus(transactionResult.AffiliateId, transactionResult.Products);
        }

        await _coinPaymentTransactionRepository.UpdateCoinPaymentTransactionAsync(transactionResult);
        return "IPN OK";
    }

    private async Task HandleCancelledTransaction(CoinpaymentTransaction transactionResult, IpnRequest ipnRequest,
        string products)
    {
        _logger.LogInformation(
            $"[ConPaymentService] | HandleCancelledTransaction | transactionResult: {transactionResult.ToJsonString()}");
        await _coinPaymentTransactionRepository.UpdateCoinPaymentTransactionAsync(transactionResult);

        await RevertUnconfirmedOrUnpaidTransactions(ipnRequest.txn_id, products);
    }

    private async Task HandlePaymentTransaction(CoinpaymentTransaction transactionResult, IpnRequest ipnRequest)
    {
        _logger.LogInformation($"[ConPaymentService] | HandlePaymentTransaction | transactionResult: {transactionResult.ToJsonString()}");
        var walletRequest = BuildWalletRequest(ipnRequest,transactionResult);

        var products = transactionResult.Products.ToJsonObject<List<ProductRequest>>();
        if (products is null) return;

        _logger.LogInformation($"[ConPaymentService] | HandlePaymentTransaction | products: {products.ToJsonString()}");

        var isExists = await _invoiceRepository.InvoiceExistsByReceiptNumber(transactionResult.IdTransaction, _brandService.BrandId);
        var productType = await GetProductType(products,transactionResult.BrandId);

        if (!isExists && ipnRequest.status is 1 or 100)
        {
            transactionResult.Acredited = true;

            await RemoveCacheKey(walletRequest.AffiliateId, CacheKeys.BalanceInformationModel2);
            await RemoveCacheKey(walletRequest.AffiliateId, CacheKeys.BalanceInformationModel1A);
            await RemoveCacheKey(walletRequest.AffiliateId, CacheKeys.BalanceInformationModel1B);
            switch (productType)
            {
                case ProductType.Membership:
                    await ExecuteMembershipPayment(walletRequest, products);
                    break;
                case ProductType.EcoPool:
                    await ExecuteEcoPoolPayment(walletRequest, products);
                    break;
                case ProductType.RecyCoin:
                    await ExecuteRecyCoinPayment(walletRequest, products);
                    _logger.LogInformation($"[CoinPayService] | ProcessPaymentTransaction | RecyCoin Payment executed");
                    break;
                case ProductType.Course:
                    await ExecuteCoursePayment(walletRequest, products);
                    break;
            }
        }
    }

    private async Task<ProductType> GetProductType(List<ProductRequest> request,long brandId)
    {
        if (request == null || !request.Any())
        {
            throw new ArgumentException("The request cannot be empty.");
        }

        var productIds = request.Select(p => p.ProductId).ToArray();
        var responseList = await _inventoryServiceAdapter.GetProductsIds(productIds, brandId);

        var result = responseList.Content!.ToJsonObject<ProductsResponse>();

        int firstProductCategory;

        if (!result!.Data.IsNullOrEmpty())
        {
            firstProductCategory = result.Data.First().PaymentGroup;
        }
        else
        {
            var membershipResult = await _inventoryServiceAdapter.GetProductById(productIds.First(), _brandService.BrandId);
            var productResult = membershipResult.Content!.ToJsonObject<ProductResponse>();
            firstProductCategory = productResult!.Data.PaymentGroup;
        }

        switch (firstProductCategory)
        {
            case 1: return ProductType.Membership;
            case 2:
            case 7:
            case 8:
                return ProductType.EcoPool;
            case 11: return ProductType.RecyCoin;
            default:
                return ProductType.Course;
        }
    }

    private bool IsRequestValid(IpnRequest ipnRequest, IHeaderDictionary headers)
    {
        if (string.IsNullOrEmpty(ipnRequest.ipn_mode) || ipnRequest.ipn_mode.ToLower() != "hmac")
            return false;

        if (!headers.TryGetValue("Hmac", out var receivedHmac) || String.IsNullOrEmpty(receivedHmac))
            return false;

        if (string.IsNullOrEmpty(ipnRequest.merchant) || ipnRequest.merchant != _merchantId)
            return false;

        if (ipnRequest.ipn_type != "api")
            return false;

        if (ipnRequest.currency1 != "USDT.TRC20")
            return false;

        return true;
    }

    private WalletRequest BuildWalletRequest(IpnRequest ipnRequest,CoinpaymentTransaction transaction)
    {
        return new WalletRequest
        {
            AffiliateId = ipnRequest.item_number,
            AffiliateUserName = ipnRequest.item_name,
            PurchaseFor = 0,
            Bank = Constants.CoinPayments,
            PaymentMethod = 4,
            SecretKey = null,
            ReceiptNumber = ipnRequest.txn_id,
            ProductsList = new List<ProductsRequests>(),
            BrandId      = transaction.BrandId,
        };
    }

    private async Task ExecuteEcoPoolPayment(WalletRequest walletRequest, ICollection<ProductRequest> products)
    {
        walletRequest.ProductsList = products.Select(product => new ProductsRequests
        {
            IdProduct = product.ProductId,
            Count = product.Quantity
        }).ToList();

        await _coinPaymentsPaymentStrategy.ExecuteEcoPoolPayment(walletRequest);
    }
    
    private async Task ExecuteRecyCoinPayment(WalletRequest walletRequest, ICollection<ProductRequest> products)
    {
        walletRequest.ProductsList = products.Select(product => new ProductsRequests
        {
            IdProduct = product.ProductId,
            Count = product.Quantity
        }).ToList();

        await _coinPaymentsPaymentStrategy.ExecuteRecyCoinPayment(walletRequest);
    }

    private async Task ExecuteCoursePayment(WalletRequest walletRequest, ICollection<ProductRequest> products)
    {
        walletRequest.ProductsList = products.Select(product => new ProductsRequests
        {
            IdProduct = product.ProductId,
            Count = product.Quantity
        }).ToList();

        await _coinPaymentsPaymentStrategy.ExecuteCoursePayment(walletRequest);
    }

    private async Task GrantWelcomeBonus(int userId, string productsInfo)
    {
        var products = productsInfo.ToJsonObject<List<ProductRequest>>();
        if (products is null) return;

        var isMembership = products.Any(p => p.ProductId == 1);
        if (!isMembership)
            return;

        var userInfo = await _accountServiceAdapter.GetUserInfo(userId, _brandService.BrandId);
        if (userInfo is null)
            return;

        var affiliateBonusWinner = await _accountServiceAdapter.GetUserInfo(userInfo.Father, _brandService.BrandId);

        var creditTransactionForWinningBonus = new CreditTransactionRequest
        {
            AffiliateId = affiliateBonusWinner!.Id,
            UserId = Constants.AdminUserId,
            Concept = Constants.CommissionMembership + ' ' + userInfo.UserName,
            Credit = Constants.MembershipBonus,
            AffiliateUserName = affiliateBonusWinner.UserName,
            ConceptType = WalletConceptType.membership_bonus.ToString(),
            AdminUserName = Constants.AdminEcosystemUserName
        };

        var bonusPaymentResult = await _walletRepository.CreditTransaction(creditTransactionForWinningBonus);

        if (bonusPaymentResult is false)
            return;

        await RemoveCacheKey(affiliateBonusWinner.Id, CacheKeys.BalanceInformationModel2);

        await _brevoEmailService.SendBonusConfirmation(affiliateBonusWinner, userInfo.UserName ?? string.Empty,
            _brandService.BrandId);
    }

    private async Task ExecuteMembershipPayment(WalletRequest walletRequest, ICollection<ProductRequest> products)
    {
        walletRequest.ProductsList = products.Select(product => new ProductsRequests
        {
            IdProduct = product.ProductId,
            Count = product.Quantity
        }).ToList();

        await _coinPaymentsPaymentStrategy.ExecuteMembershipPayment(walletRequest);
    }

    private async Task<bool> RevertUnconfirmedOrUnpaidTransactions(string idTransaction, string productsInfo)
    {
        if (string.IsNullOrEmpty(idTransaction))
            return false;

        var invoicesResult = await _invoiceRepository.GetInvoiceByReceiptNumber(idTransaction, _brandService.BrandId);
        if (invoicesResult is null)
            return false;

        var invoiceNumber = new InvoiceNumber
        {
            InvoiceNumberValue = invoicesResult.Id
        };

        var products = productsInfo.ToJsonObject<List<ProductRequest>>();

        if (products is null) return false;

        bool isMembership = products.Any(p => p.ProductId == 1);
        _logger.LogInformation(
            $"[ConPaymentService] | RevertUnconfirmedOrUnpaidTransactions | idTransaction: {idTransaction} | products: {products.ToJsonString()}");

        if (isMembership)
            await _accountServiceAdapter.RevertActivationUser(invoicesResult.AffiliateId, _brandService.BrandId);

        await _invoiceRepository.RevertCoinPaymentTransactions(new List<InvoiceNumber> { invoiceNumber });
        return true;
    }

    public async Task<CoinPaymentWithdrawalResponse?> CreateMassWithdrawal(WalletsRequest[] requests)
    {
        _logger.LogInformation(
            $"[CoinPaymentService] | CreateMassWithdrawal | Start | requests: {requests.ToJsonString()}");

        var withdrawals = await GetAddressesByAffiliateId(requests);

        SortedList<string, string> parms = new SortedList<string, string>
        {
            { "cmd", "create_mass_withdrawal" }
        };

        int count = 1;
        var tax = Constants.CoinPaymentTax;

        foreach (var withdrawal in withdrawals)
        {
            decimal amountAfterTax = withdrawal.Amount - tax;
            string prefix = $"wd[wd{count}]";
            parms.Add($"{prefix}[amount]", amountAfterTax.ToString(CultureInfo.InvariantCulture));
            parms.Add($"{prefix}[address]", withdrawal.Address);
            parms.Add($"{prefix}[currency]", withdrawal.Currency);
            count++;
        }

        var restResponse = await CallApiAsync("create_mass_withdrawal", parms);
        _logger.LogInformation(
            $"[CoinPaymentService] | CreateMassWithdrawal | Response Api | response: {restResponse.ToJsonString()}");

        if (!restResponse.IsSuccessful)
        {
            throw new Exception($"Error calling API: {restResponse.StatusDescription}");
        }

        var withdrawalResults = restResponse.Content!.ToJsonObject<CoinPaymentWithdrawalResponse>();

        if (withdrawalResults?.Result != null)
        {
            var successfulRequests = new List<WalletsRequest>();

            int index = 0;
            foreach (var key in withdrawalResults.Result.Keys)
            {
                var withdrawalInfo = withdrawalResults.Result[key];

                if (withdrawalInfo.Error == "ok")
                {
                    successfulRequests.Add(requests[index]);
                }

                index++;
            }

            await CreateDebitTransaction(successfulRequests.ToArray());
            await UpdateWithdrawals(successfulRequests.ToArray());
        }

        return withdrawalResults;
    }

    private async Task<List<CoinPaymentMassWithdrawalRequest>> GetAddressesByAffiliateId(WalletsRequest[] requests)
    {
        List<CoinPaymentMassWithdrawalRequest> withdrawals = new List<CoinPaymentMassWithdrawalRequest>();

        foreach (var request in requests)
        {
            var response =
                await _accountServiceAdapter.GetAffiliateBtcByAffiliateId(request.AffiliateId, _brandService.BrandId);

            if (response.Content is null)
                continue;

            var userInfo = response.Content.ToJsonObject<AffiliateBtcResponse>();

            if (userInfo?.Data is null)
                continue;

            var btcAddress = userInfo.Data.Address;

            if (string.IsNullOrEmpty(btcAddress))
                continue;

            var withdrawal = new CoinPaymentMassWithdrawalRequest
            {
                Amount = request.Amount,
                Address = btcAddress,
                Currency = Constants.ConPaymentCurrency
            };

            withdrawals.Add(withdrawal);
        }

        return withdrawals;
    }

    private async Task<bool> CreateDebitTransaction(WalletsRequest[] requests)
    {
        if (requests.Length is 0)
            return false;

        List<Wallet> walletsList = new List<Wallet>();
        var today = DateTime.Now;

        foreach (var request in requests)
        {
            var walletEntry = new Wallet
            {
                AffiliateId = request.AffiliateId,
                AffiliateUserName = request.AdminUserName,
                AdminUserName = Constants.AdminEcosystemUserName,
                UserId = Constants.AdminUserId,
                Credit = Constants.EmptyValue,
                Debit = request.Amount,
                Deferred = Constants.EmptyValue,
                Status = true,
                Concept = Constants.WithdrawalBalance,
                ConceptType = WalletConceptType.wallet_withdrawal_request.ToString(),
                Support = Constants.EmptyValue,
                Date = today,
                Compression = false,
                Detail = null,
                CreatedAt = today,
                UpdatedAt = today,
                DeletedAt = null
            };

            walletsList.Add(walletEntry);
        }

        var result = await _walletRepository.BulkAdministrativeDebitTransaction(walletsList.ToArray());

        if (!result)
            return false;

        return result;
    }

    private async Task UpdateWithdrawals(WalletsRequest[] requests)
    {
        List<WalletsRequest> withdrawalList = requests.ToList();

        DateTime today = DateTime.Now;

        Parallel.ForEach(withdrawalList, item =>
        {
            item.AttentionDate = today;
            item.UpdatedAt = today;
            item.Status = WalletRequestStatus.approved.ToByte();
        });

        await _walletRequestRepository.UpdateBulkWalletRequestsAsync(withdrawalList);
    }

    private async Task RemoveCacheKey(int affiliateId, string stringKey)
    {
        var key = string.Format(stringKey, affiliateId);
        var existsKey = await _redisCache.KeyExists(key);

        if (existsKey)
            await _redisCache.Delete(key);
    }
}