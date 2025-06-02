using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Globalization;
using Microsoft.IdentityModel.Tokens;
using WalletService.Core.Caching;
using WalletService.Core.Caching.Extensions;
using WalletService.Core.PaymentStrategies.IPaymentStrategies;
using WalletService.Core.Services.IServices;
using WalletService.Data.Adapters.IAdapters;
using WalletService.Data.Database;
using WalletService.Data.Database.CustomModels;
using WalletService.Data.Database.Models;
using WalletService.Data.Repositories.IRepositories;
using WalletService.Models.Configuration;
using WalletService.Models.Constants;
using WalletService.Models.Enums;
using WalletService.Models.Requests.ConPaymentRequest;
using WalletService.Models.Requests.WalletRequest;
using WalletService.Models.Requests.WalletWithDrawalRequest;
using WalletService.Models.Responses;
using WalletService.Models.Responses.BaseResponses;
using WalletService.Utility.Extensions;

namespace WalletService.Core.Services;

public class ConPaymentService : BaseService, IConPaymentService
{
    private readonly string _merchantId;
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
    private readonly IWalletWithdrawalService _walletWithDrawalService;
    private readonly ICoinPayPaymentStrategy _coinPayPaymentStrategy;
    private readonly WalletServiceDbContext _dbContext;
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
        IBrandService brandService,
        IWalletWithdrawalService walletWithDrawalService,
        ICoinPayPaymentStrategy coinPayPaymentStrategy,
        WalletServiceDbContext dbContext
    ) : base(mapper)
    {
        _invoiceRepository = invoiceRepository;
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
        _walletWithDrawalService = walletWithDrawalService;
        _coinPayPaymentStrategy = coinPayPaymentStrategy;
        _dbContext = dbContext;
        var appSettings1 = appSettings.Value;
        // Configuración específica para brandId = 3
        string apiKey, apiSecret,merchantId;
        if (brandService.BrandId == 3)
        {
            // Configuración específica para HouseCoin (brandId = 3)
            apiKey = "1d0bbcde6b8ed9ba7330269abd07f8f594602cbe5c1f85e156668c2fdadcf3e6";
            apiSecret = "B975d24bB02d25f3E7Ac9C992e5e7E657fF8b07cbC91ad5D1ad79c9fB3222388";
            merchantId = "7d5464c234445ce8e1ed8f328f74d10f";
        }
        else
        {
            // Configuración por defecto
            apiKey = appSettings1.ConPayments!.Key;
            apiSecret = appSettings1.ConPayments.Secret;
            merchantId = appSettings1.ConPayments.MerchantId;
        }
        _conPaymentsApi = new ConPaymentsApi.ConPaymentsApi(apiSecret, apiKey);
        _merchantId = merchantId;
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
        // request.Address = Constants.ConPaymentAddress;
        request.Currency1 = Constants.CoinPaymentsBnbCurrency ;
        request.Currency2 = Constants.CoinPaymentsBnbCurrency ;

        if (request.Products.Count > 0)
        {
            request.Custom = JsonConvert.SerializeObject(request.Products);
        }
    }

    private SortedList<string, string> ConfigureParms(ConPaymentRequest request)
    {
        request.Amount += 2;
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
        _logger.LogInformation(
            $"[ConPaymentService] | HandlePaymentTransaction | transactionResult: {transactionResult.ToJsonString()}");
        var walletRequest = BuildWalletRequest(ipnRequest, transactionResult);

        var products = transactionResult.Products.ToJsonObject<List<ProductRequest>>();
        if (products is null) return;

        _logger.LogInformation($"[ConPaymentService] | HandlePaymentTransaction | products: {products.ToJsonString()}");

        var isExists =
            await _invoiceRepository.InvoiceExistsByReceiptNumber(transactionResult.IdTransaction,
                _brandService.BrandId);
        var productType = await GetProductType(products, transactionResult.BrandId);

        if (!isExists && ipnRequest.status is 1 or 100)
        {
            transactionResult.Acredited = true;

            await _redisCache.InvalidateBalanceAsync(walletRequest.AffiliateId);
            switch (productType)
            {
                case ProductType.Membership:
                    await ExecuteMembershipPayment(walletRequest, products);
                    _logger.LogInformation($"[CoinPaymentService] | ProcessPaymentTransaction | Membership Payment executed");
                    break;
                case ProductType.EcoPool:
                    await ExecuteEcoPoolPayment(walletRequest, products);
                    _logger.LogInformation($"[CoinPaymentService] | ProcessPaymentTransaction | Eco pool Payment executed");
                    break;
                case ProductType.RecyCoin:
                    await ExecuteRecyCoinPayment(walletRequest, products);
                    _logger.LogInformation($"[CoinPaymentService] | ProcessPaymentTransaction | RecyCoin Payment executed");
                    break;
                case ProductType.HouseCoinPlan:
                    await ExecuteHouseCoinPlanPayment(walletRequest, products);
                    _logger.LogInformation($"[CoinPaymentService] | ProcessPaymentTransaction | HouseCoinPlan Payment executed");
                    break;
                case ProductType.Course:
                    await ExecuteCoursePayment(walletRequest, products);
                    _logger.LogInformation($"[CoinPaymentService] | ProcessPaymentTransaction | Course Payment executed");
                    break;
            }
        }
    }

    private async Task<ProductType> GetProductType(List<ProductRequest> request, long brandId)
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
            case 12: return ProductType.HouseCoinPlan;
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

        if (string.IsNullOrEmpty(ipnRequest.merchant))
            return false;

        if (ipnRequest.ipn_type != "api")
            return false;

        var validCurrencies = new[] { "USDT.TRC20", "USDT.BEP20" };

        if (!validCurrencies.Contains(ipnRequest.currency1))
            return false;


        return true;
    }

    private WalletRequest BuildWalletRequest(IpnRequest ipnRequest, CoinpaymentTransaction transaction)
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
            BrandId = transaction.BrandId,
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
        
        await _redisCache.InvalidateBalanceAsync(affiliateBonusWinner.Id);
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

    private async Task RevertUnconfirmedOrUnpaidTransactions(string idTransaction, string productsInfo)
    {
        if (string.IsNullOrEmpty(idTransaction))
        {
            _logger.LogWarning(
                $"[ConPaymentService] | RevertUnconfirmedOrUnpaidTransactions | Transacción inválida: {idTransaction}");
            return;
        }

        var invoice = await _invoiceRepository.GetInvoiceByReceiptNumber(idTransaction, _brandService.BrandId);
        if (invoice is null)
        {
            _logger.LogWarning(
                $"[ConPaymentService] | RevertUnconfirmedOrUnpaidTransactions | Factura no encontrada para la transacción: {idTransaction}");
            return;
        }

        var products = productsInfo.ToJsonObject<List<ProductRequest>>();
        if (products is null || !products.Any())
        {
            _logger.LogWarning(
                $"[ConPaymentService] | RevertUnconfirmedOrUnpaidTransactions | Información de productos inválida: {productsInfo}");
            return;
        }

        _logger.LogInformation(
            $"[ConPaymentService] | RevertUnconfirmedOrUnpaidTransactions | idTransaction: {idTransaction} | products: {products.ToJsonString()}");

        try
        {
            bool isMembership = products.Any(p => p.ProductId == 1);
            if (isMembership)
            {
                await _accountServiceAdapter.RevertActivationUser(invoice.AffiliateId, _brandService.BrandId);
            }

            var invoiceNumber = new InvoiceNumber { InvoiceNumberValue = invoice.Id };
            await _invoiceRepository.RevertCoinPaymentTransactions(new List<InvoiceNumber> { invoiceNumber });

            _logger.LogInformation(
                $"[ConPaymentService] | RevertUnconfirmedOrUnpaidTransactions | Reversión exitosa para transacción: {idTransaction}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                $"[ConPaymentService] | RevertUnconfirmedOrUnpaidTransactions | Error al revertir transacción: {idTransaction}");
        }
    }

     public async Task<CoinPaymentWithdrawalResponse?> CreateMassWithdrawal(WalletsRequest[] requests)
    {
        _logger.LogInformation($"[ConPaymentService] | CreateMassWithdrawal | Start | requests: {requests.ToJsonString()}");

        // 1) Armar parámetros para la llamada masiva
        var withdrawals = await GetAddressesByAffiliateId(requests);
        var parms = new SortedList<string, string> { { "cmd", "create_mass_withdrawal" } };
        var tax = Constants.CoinPaymentTax;
        for (int i = 0; i < withdrawals.Count; i++)
        {
            var w = withdrawals[i];
            var amountAfterTax = w.Amount - tax;
            var prefix = $"wd[wd{i + 1}]";
            parms.Add($"{prefix}[amount]", amountAfterTax.ToString(CultureInfo.InvariantCulture));
            parms.Add($"{prefix}[address]", w.Address);
            parms.Add($"{prefix}[currency]", w.Currency);
        }

        // 2) Llamar a la API
        var restResponse = await CallApiAsync("create_mass_withdrawal", parms);
        if (!restResponse.IsSuccessful)
            throw new Exception($"Error calling API: {restResponse.StatusDescription}");

        var withdrawalResults = restResponse.Content!.ToJsonObject<CoinPaymentWithdrawalResponse>();
        if (withdrawalResults?.Result == null)
        {
            _logger.LogWarning("[ConPaymentService] | CreateMassWithdrawal | withdrawalResults.Result is null.");
            return null;
        }

        // 3) Filtrar solo los IDs que la API devolvió con Error == "ok"
        var succeededApiIds = new List<long>();
        int idx = 0;
        foreach (var key in withdrawalResults.Result.Keys)
        {
            if (withdrawalResults.Result[key].Error == "ok")
                succeededApiIds.Add(requests[idx].Id);
            idx++;
        }

        var today = DateTime.Now;
        foreach (var withdrawalId in succeededApiIds)
        {
            var req = requests.First(r => r.Id == withdrawalId);

            // **EF Core Transaction por cada retiro individual**
            await using var tx = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                // 4a) Obtener usuario
                var user = await _accountServiceAdapter.GetUserInfo(req.AffiliateId, _brandService.BrandId);
                if (user == null)
                {
                    _logger.LogWarning($"[ConPaymentService] | No user found for ID {req.AffiliateId}");
                    await tx.RollbackAsync();
                    continue;
                }

                // 4b) Crear movimiento en Wallet
                var debitTransaction = new Wallet
                {
                    AffiliateId = req.AffiliateId,
                    UserId = 1,
                    Credit = 0,
                    Debit = req.Amount,
                    Deferred = 0,
                    Status = true,
                    Concept = Constants.SendFundsConcept,
                    Support = 1,
                    Date = today,
                    Compression = true,
                    Detail = "Retiro de fondos en coinpayments",
                    CreatedAt = today,
                    UpdatedAt = today,
                    AffiliateUserName = user.UserName ?? "unknown",
                    AdminUserName = _brandService.BrandId switch
                    {
                        1 => "adminecosystem",
                        2 => "adminrecycoin",
                        3 => "adminhousecoin",
                        4 => "adminexitojuntos",
                        _ => "adminecosystem"
                    },
                    ConceptType = WalletConceptType.balance_transfer.ToString(),
                    BrandId = _brandService.BrandId
                };
                await _walletRepository.CreateWalletAsync(debitTransaction);

                // 4c) Crear registro de WalletWithdrawal
                var walletWithdrawal = new WalletWithDrawalRequest
                {
                    AffiliateId = user.Id,
                    AffiliateUserName = user.UserName ?? "unknown",
                    Amount = req.Amount,
                    IsProcessed = true,
                    Observation = $"{Constants.SendFundsConcept} {req.Id}",
                    AdminObservation = "MassWithdrawal - ConPayments",
                    Date = today,
                    ResponseDate = today,
                    RetentionPercentage = Constants.EmptyValue,
                    Status = true
                };
                await _walletWithDrawalService.CreateWalletWithdrawalAsync(walletWithdrawal);

                // 4d) Actualizar el status del WalletRequest
                var wr = await _walletRequestRepository.GetByIdAsync((int)req.Id);
                if (wr == null)
                {
                    _logger.LogWarning($"[ConPaymentService] | No WalletRequest found for ID {req.Id}");
                    await tx.RollbackAsync();
                    continue;
                }
                
                wr.Status = (int)WithdrawalStatus.Completed;
                wr.UpdatedAt = today;
                await _walletRequestRepository.UpdateWalletRequestsAsync(wr);

                // 4e) Commit
                await tx.CommitAsync();

                // 4f) Invalidar cache
                await _redisCache.InvalidateBalanceAsync(req.AffiliateId);
            }
            catch (Exception ex)
            {
                await tx.RollbackAsync();
                _logger.LogError(ex, $"[ConPaymentService] | Error processing withdrawalId {withdrawalId}");
            }
        }

        return withdrawalResults;
    }

    private async Task<List<CoinPaymentMassWithdrawalRequest>> GetAddressesByAffiliateId(WalletsRequest[] requests)
    {
        List<CoinPaymentMassWithdrawalRequest> withdrawals = new List<CoinPaymentMassWithdrawalRequest>();

        foreach (var request in requests)
        {
            try
            {
                var response = await _accountServiceAdapter.GetAffiliateBtcByAffiliateId(request.AffiliateId, _brandService.BrandId);

                if (response.Content == null)
                {
                    _logger.LogWarning($"Null response for affiliate {request.AffiliateId}");
                    continue;
                }

                var userInfo = JsonConvert.DeserializeObject<AffiliateBtcResponse>(response.Content);

                if (userInfo?.Data == null)
                {
                    _logger.LogWarning($"No wallet data found for affiliate {request.AffiliateId}");
                    continue;
                }

                var validData = userInfo.Data.Where(x => x != null).ToArray();
                if (validData.Length == 0)
                {
                    _logger.LogWarning($"No valid data for affiliate {request.AffiliateId}");
                    continue;
                }

                var btcAddress = validData.FirstOrDefault(x => !string.IsNullOrEmpty(x?.Address))?.Address;

                if (string.IsNullOrEmpty(btcAddress))
                {
                    _logger.LogWarning($"No valid BTC address found for affiliate {request.AffiliateId}");
                    continue;
                }

                var withdrawal = new CoinPaymentMassWithdrawalRequest
                {
                    Amount = request.Amount,
                    Address = btcAddress,
                    Currency = Constants.CoinPaymentsBnbCurrency
                };

                withdrawals.Add(withdrawal);
               _logger.LogInformation($"Withdrawal request created for affiliate {request.AffiliateId} to the address {btcAddress}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error processing request for affiliate {request.AffiliateId}");
            }
        }

        return withdrawals;
    }
    private async Task UpdateSuccessfulWithdrawals(List<long> successfulWithdrawalIds)
    {
        _logger.LogInformation($"[WalletService] | UpdateSuccessfulWithdrawals | Starting update for {successfulWithdrawalIds.ToJsonString()} successful withdrawals.");
        var withdrawals = await _walletRequestRepository.GetWalletRequestsByIds(successfulWithdrawalIds);

        foreach (var withdrawal in withdrawals)
        {
            _logger.LogDebug($"[WalletService] | UpdateSuccessfulWithdrawals | Updating status for withdrawal ID: {withdrawal.Id}");
            withdrawal.Status = (int)WithdrawalStatus.Completed;
            withdrawal.UpdatedAt = DateTime.Now;
        }

        await _walletRequestRepository.UpdateBulkWalletRequestsAsync(withdrawals);
        _logger.LogInformation($"[WalletService] | UpdateSuccessfulWithdrawals | Successfully updated withdrawals.");
    }
    
    private async Task ExecuteHouseCoinPlanPayment(WalletRequest walletRequest, ICollection<ProductRequest> products)
    {
        walletRequest.ProductsList = products.Select(product => new ProductsRequests
        {
            IdProduct = product.ProductId,
            Count = product.Quantity
        }).ToList();

        await _coinPayPaymentStrategy.ExecuteHouseCoinPayment(walletRequest);
    }
}