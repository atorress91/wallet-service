using System.Globalization;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
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
using WalletService.Models.Requests.PagaditoRequest;
using WalletService.Models.Requests.WalletRequest;
using WalletService.Models.Responses;
using WalletService.Utility.Extensions;

namespace WalletService.Core.Services;

public class PagaditoService : BaseService, IPagaditoService
{
    private readonly IPagaditoAdapter _pagaditoAdapter;
    private readonly ApplicationConfiguration _appSettings;
    private readonly ICoinPaymentTransactionRepository _transactionRepository;
    private readonly IPagaditoPaymentStrategy _pagaditoPaymentStrategy;
    private readonly IInventoryServiceAdapter _inventoryServiceAdapter;
    private readonly ILogger<PagaditoService> _logger;
    private readonly IAccountServiceAdapter _accountServiceAdapter;
    private readonly IInvoiceRepository _invoiceRepository;

    public PagaditoService(IOptions<ApplicationConfiguration> appSettings, IMapper mapper,
        IPagaditoAdapter pagaditoAdapter, ICoinPaymentTransactionRepository transactionRepository,
        IPagaditoPaymentStrategy pagaditoPaymentStrategy,
        IInventoryServiceAdapter inventoryServiceAdapter, ILogger<PagaditoService> logger,
        IAccountServiceAdapter accountServiceAdapter, IInvoiceRepository invoiceRepository) : base(mapper)
    {
        _pagaditoAdapter = pagaditoAdapter;
        _appSettings = appSettings.Value;
        _transactionRepository = transactionRepository;
        _pagaditoPaymentStrategy = pagaditoPaymentStrategy;
        _inventoryServiceAdapter = inventoryServiceAdapter;
        _logger = logger;
        _accountServiceAdapter = accountServiceAdapter;
        _invoiceRepository = invoiceRepository;
    }

    private async Task ExecuteMembershipPayment(WalletRequest walletRequest, ICollection<ProductRequest> products)
    {
        walletRequest.ProductsList = products.Select(product => new ProductsRequests
        {
            IdProduct = product.ProductId,
            Count = product.Quantity
        }).ToList();

        await _pagaditoPaymentStrategy.ExecuteMembershipPayment(walletRequest);
    }

    private async Task ExecuteEcoPoolPayment(WalletRequest walletRequest, ICollection<ProductRequest> products)
    {
        walletRequest.ProductsList = products.Select(product => new ProductsRequests
        {
            IdProduct = product.ProductId,
            Count = product.Quantity
        }).ToList();

        await _pagaditoPaymentStrategy.ExecuteEcoPoolPayment(walletRequest);
    }

    private async Task ExecuteCoursePayment(WalletRequest walletRequest, ICollection<ProductRequest> products)
    {
        walletRequest.ProductsList = products.Select(product => new ProductsRequests
        {
            IdProduct = product.ProductId,
            Count = product.Quantity
        }).ToList();

        await _pagaditoPaymentStrategy.ExecuteCoursePayment(walletRequest);
    }

    private async Task<ProductType> GetProductType(List<ProductRequest> request)
    {
        if (request == null || !request.Any())
        {
            throw new ArgumentException("The request cannot be empty.");
        }

        var productIds = request.Select(p => p.ProductId).ToArray();
        var responseList = await _inventoryServiceAdapter.GetProductsIds(productIds);

        var result = JsonSerializer.Deserialize<ProductsResponse>(responseList.Content!);

        int firstProductCategory;

        if (!result!.Data.IsNullOrEmpty())
        {
            firstProductCategory = result.Data.First().PaymentGroup;
        }
        else
        {
            var membershipResult = await _inventoryServiceAdapter.GetProductById(productIds.First());
            var productResult = JsonSerializer.Deserialize<ProductResponse>(membershipResult.Content!);
            firstProductCategory = productResult!.Data.PaymentGroup;
        }

        switch (firstProductCategory)
        {
            case 1: return ProductType.Membership;
            case 2: return ProductType.EcoPool;
            default:
                return ProductType.Course;
        }
    }

    private async Task ProcessPaymentTransaction(PaymentTransaction transactionResult)
    {
        _logger.LogInformation(
            $"[PagaditoService] | ProcessPaymentTransaction | transactionResult: {transactionResult.ToJsonString()}");

        var userInfo = await _accountServiceAdapter.GetUserInfo(transactionResult.AffiliateId);

        if (userInfo == null || userInfo.UserName == null)
            return;

        var productsList = new List<ProductsRequests>();

        if (!string.IsNullOrEmpty(transactionResult.Products))
        {
            var productRequests = JsonSerializer.Deserialize<List<ProductRequest>>(transactionResult.Products);
            if (productRequests != null)
            {
                productsList = productRequests
                    .Select(p => new ProductsRequests { IdProduct = p.ProductId, Count = p.Quantity })
                    .ToList();
            }
        }

        var walletRequest = new WalletRequest
        {
            AffiliateId = transactionResult.AffiliateId,
            AffiliateUserName = userInfo.UserName,
            PurchaseFor = Constants.EmptyValue,
            Bank = transactionResult.Reference,
            PaymentMethod = 4,
            ReceiptNumber = transactionResult.IdTransaction,
            ProductsList = productsList
        };

        var products = JsonSerializer.Deserialize<List<ProductRequest>>(transactionResult.Products);
        if (products == null)
            return;

        _logger.LogInformation($"[PagaditoService] | ProcessPaymentTransaction | products: {products.ToJsonString()}");

        var isExists = await _invoiceRepository.InvoiceExistsByReceiptNumber(transactionResult.IdTransaction);
        if (isExists)
            return;

        var productType = await GetProductType(products);

        switch (productType)
        {
            case ProductType.Membership:
                await ExecuteMembershipPayment(walletRequest, products);
                _logger.LogInformation($"[PagaditoService] | ProcessPaymentTransaction | Membership Payment executed");
                break;
            case ProductType.EcoPool:
                await ExecuteEcoPoolPayment(walletRequest, products);
                _logger.LogInformation($"[PagaditoService] | ProcessPaymentTransaction | EcoPool Payment executed");
                break;
            case ProductType.Course:
                await ExecuteCoursePayment(walletRequest, products);
                _logger.LogInformation($"[PagaditoService] | ProcessPaymentTransaction | Course Payment executed");
                break;
        }
    }

    public async Task<string?> CreateTransaction(CreatePagaditoTransactionRequest request)
    {
        _logger.LogInformation($"[PagaditoService] | CreateTransaction | request: {request.ToJsonString()}");
        var today = DateTime.Now;
        var connectResponse = await _pagaditoAdapter.ConnectAsync();

        if (connectResponse == null || string.IsNullOrEmpty(connectResponse.Value))
            return "This connect is not valid";

        var pagaditoTransaction = Mapper.Map<CreatePagaditoTransaction>(request);
        pagaditoTransaction.Token = connectResponse.Value;
        pagaditoTransaction.Ern = Guid.NewGuid().ToString();

        var executeTransaction = await _pagaditoAdapter.ExecuteTransaction(pagaditoTransaction);

        _logger.LogInformation(
            $"[PagaditoService] | CreateTransaction | executeTransaction: {executeTransaction.ToJsonString()}");

        if (executeTransaction == null || string.IsNullOrEmpty(executeTransaction.Value))
            return "This transaction is not valid";

        var productDetails = request.Details.Select(detail => new ProductRequest
        {
            ProductId = int.Parse(detail.UrlProduct!),
            Quantity = int.Parse(detail.Quantity!),
        }).ToList();

        var productsJson = JsonSerializer.Serialize(productDetails);

        var paymentTransaction = new PaymentTransaction
        {
            IdTransaction = pagaditoTransaction.Ern,
            AffiliateId = request.AffiliateId,
            Amount = pagaditoTransaction.Amount,
            AmountReceived = Constants.EmptyValue,
            Products = productsJson,
            PaymentMethod = Constants.Pagadito,
            Status = Constants.EmptyValue,
            Acredited = false,
            CreatedAt = today,
            UpdatedAt = today
        };

        await _transactionRepository.CreateCoinPaymentTransaction(paymentTransaction);

        return executeTransaction.Value;
    }

    public async Task<bool> VerifySignature(IHeaderDictionary headers, string requestBody)
    {
        _logger.LogInformation($"[PagaditoService] | VerifySignature | Starting signature verification");

        var notificationId = headers["PAGADITO-NOTIFICATION-ID"];
        var notificationTimestamp = headers["PAGADITO-NOTIFICATION-TIMESTAMP"];
        var certUrl = headers["PAGADITO-CERT-URL"];
        var notificationSignature = Convert.FromBase64String(headers["PAGADITO-SIGNATURE"]!);
        var wsk = _appSettings.Pagadito!.Wsk;

        _logger.LogInformation($"[PagaditoService] | VerifySignature | Headers and settings retrieved");

        var eventId = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(requestBody)?.id;

        var dataSigned = string.Join("|", notificationId, notificationTimestamp, eventId,
            CommonExtensions.Crc32(requestBody),
            wsk);

        _logger.LogInformation($"[PagaditoService] | VerifySignature | Data signed");

        byte[] certContent;
        using (var httpClient = new HttpClient())
        {
            certContent = await httpClient.GetByteArrayAsync(certUrl);
        }

        _logger.LogInformation($"[PagaditoService] | VerifySignature | Certificate content retrieved");

        using var cert = new X509Certificate2(certContent);
        using var pubkey = cert.GetRSAPublicKey();

        var signatureResult = pubkey!.VerifyData(Encoding.UTF8.GetBytes(dataSigned), notificationSignature,
            HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);

        if (signatureResult)
        {
            var request = JsonSerializer.Deserialize<WebHookRequest>(requestBody);
            signatureResult = await UpdateTransactionStatus(request);
        }

        _logger.LogInformation(
            $"[PagaditoService] | VerifySignature | Signature verification result: {signatureResult}");

        return signatureResult;
    }

    public async Task<bool> UpdateTransactionStatus(WebHookRequest? purchaseRequest)
    {
        _logger.LogInformation($"[PagaditoService] | UpdateTransactionStatus | Starting transaction status update");

        if (purchaseRequest is null)
        {
            _logger.LogWarning($"[PagaditoService] | UpdateTransactionStatus | purchaseRequest is null");
            return false;
        }

        var existingTransaction = await _transactionRepository.GetCoinPaymentTransactionByIdTransaction(purchaseRequest.Resource!.Ern!);

        if (existingTransaction is null)
        {
            _logger.LogWarning($"[PagaditoService] | UpdateTransactionStatus | No existing transaction found");
            return false;
        }

        if (existingTransaction.Acredited)
        {
            _logger.LogInformation($"[PagaditoService] | UpdateTransactionStatus | Existing transaction is already accredited");
            return false;
        }

        if (purchaseRequest.Resource.Status == Constants.RegisteredStatus)
        {
            existingTransaction.Status = Constants.RegisteredStatusCode;
            existingTransaction.Acredited = false;
        }
        else if (purchaseRequest.Resource.Status == Constants.ExpiredStatus)
        {
            existingTransaction.Status = Constants.ExpiredStatusCode;
            existingTransaction.Acredited = false;
        }
        else if (purchaseRequest.Resource.Status == Constants.CompletedStatus && existingTransaction.Acredited == false)
        {
            existingTransaction.Status = Constants.CompletedStatusCode;
            decimal.TryParse(purchaseRequest.Resource.Amount!.Total!, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal amountReceived);
            existingTransaction.AmountReceived = amountReceived;
            existingTransaction.Acredited = true;
            existingTransaction.Reference = purchaseRequest.Resource.Reference;
            await ProcessPaymentTransaction(existingTransaction);
            _logger.LogInformation($"[PagaditoService] | UpdateTransactionStatus | Transaction processed");
        }

        await _transactionRepository.UpdateCoinPaymentTransactionAsync(existingTransaction);
        _logger.LogInformation($"[PagaditoService] | UpdateTransactionStatus | Transaction status updated");

        return true;
    }
}