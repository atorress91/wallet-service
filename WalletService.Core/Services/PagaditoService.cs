using System.Globalization;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
using JsonSerializer = System.Text.Json.JsonSerializer;

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
    private readonly IBrandService _brandService;
    public PagaditoService(IOptions<ApplicationConfiguration> appSettings, IMapper mapper,
        IPagaditoAdapter pagaditoAdapter, ICoinPaymentTransactionRepository transactionRepository,
        IPagaditoPaymentStrategy pagaditoPaymentStrategy,
        IInventoryServiceAdapter inventoryServiceAdapter, ILogger<PagaditoService> logger,
        IAccountServiceAdapter accountServiceAdapter, IInvoiceRepository invoiceRepository,IBrandService brandService) : base(mapper)
    {
        _pagaditoAdapter = pagaditoAdapter;
        _appSettings = appSettings.Value;
        _transactionRepository = transactionRepository;
        _pagaditoPaymentStrategy = pagaditoPaymentStrategy;
        _inventoryServiceAdapter = inventoryServiceAdapter;
        _logger = logger;
        _accountServiceAdapter = accountServiceAdapter;
        _invoiceRepository = invoiceRepository;
        _brandService = brandService;
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
        var responseList = await _inventoryServiceAdapter.GetProductsIds(productIds,_brandService.BrandId);

        var result = JsonSerializer.Deserialize<ProductsResponse>(responseList.Content!);

        int firstProductCategory;

        if (!result!.Data.IsNullOrEmpty())
        {
            firstProductCategory = result.Data.First().PaymentGroup;
        }
        else
        {
            var membershipResult = await _inventoryServiceAdapter.GetProductById(productIds.First(),_brandService.BrandId);
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

        var userInfo = await _accountServiceAdapter.GetUserInfo(transactionResult.AffiliateId,_brandService.BrandId);

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

        var isExists = await _invoiceRepository.InvoiceExistsByReceiptNumber(transactionResult.IdTransaction,_brandService.BrandId);
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
            $"[PagaditoService] | CreateTransaction | executeTransaction: {executeTransaction?.ToJsonString()}");

        if (string.IsNullOrEmpty(executeTransaction?.Value))
            return "This transaction is not valid";

        var productDetails = request.Details.Select(detail => new ProductRequest
        {
            ProductId = int.Parse(detail.UrlProduct!),
            Quantity = detail.Quantity,
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
            UpdatedAt = today,
            BrandId   = _brandService.BrandId 
        };

        await _transactionRepository.CreateCoinPaymentTransaction(paymentTransaction);

        return executeTransaction.Value;
    }
    
    public async Task<bool> VerifySignature(IHeaderDictionary headers, string requestBody)
    {
        _logger.LogInformation($"[PagaditoService] | VerifySignature | Starting signature verification: {requestBody.ToJsonString()}");

        var notificationId = headers["pagadito-notification-id"];
        var notificationTimestamp = headers["pagadito-notification-timestamp"];
        var certUrl = headers["pagadito-cert-url"];
        var notificationSignature = headers["pagadito-signature"];
        var wsk = _appSettings.Pagadito!.Wsk;

        _logger.LogInformation($"NotificationId: {notificationId}, Timestamp: {notificationTimestamp}, CertUrl: {certUrl}");

        var jsonObject = JObject.Parse(requestBody);
        var eventId = jsonObject["id"]?.ToString();
        
        _logger.LogInformation($"EventId: {eventId}");

        var crc32 = CommonExtensions.Crc32(requestBody);
        var dataSigned = string.Join("|", notificationId, notificationTimestamp, eventId, crc32, wsk);

        _logger.LogInformation($"DataSigned: {dataSigned}");

        try
        {
            byte[] certContent;
            using (var httpClient = new HttpClient())
            {
                certContent = await httpClient.GetByteArrayAsync(certUrl);
            }

            using var cert = new X509Certificate2(certContent);
            _logger.LogInformation($"Certificate Thumbprint: {cert.Thumbprint}");

            using var pubkey = cert.GetRSAPublicKey();

            var signatureBytes = Convert.FromBase64String(notificationSignature);
            var signatureResult = pubkey!.VerifyData(
                Encoding.UTF8.GetBytes(dataSigned),
                signatureBytes,
                HashAlgorithmName.SHA256,
                RSASignaturePadding.Pkcs1);

            _logger.LogInformation($"Signature verification result: {signatureResult}");
            
            var request = JsonConvert.DeserializeObject<WebHookRequest>(requestBody);
            
            return   await UpdateTransactionStatus(request);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error during signature verification: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> UpdateTransactionStatus(WebHookRequest? purchaseRequest)
    {
        _logger.LogInformation($"[PagaditoService] | UpdateTransactionStatus | Starting transaction status update");

        if (purchaseRequest is null)
        {
            _logger.LogWarning($"[PagaditoService] | UpdateTransactionStatus | purchaseRequest is null");
            return false;
        }

        var existingTransaction =
            await _transactionRepository.GetCoinPaymentTransactionByIdTransaction(purchaseRequest.Resource!.Ern!,_brandService.BrandId);

        if (existingTransaction is null)
        {
            _logger.LogWarning($"[PagaditoService] | UpdateTransactionStatus | No existing transaction found");
            return false;
        }

        if (existingTransaction.Acredited)
        {
            _logger.LogInformation(
                $"[PagaditoService] | UpdateTransactionStatus | Existing transaction is already accredited");
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
            decimal.TryParse(purchaseRequest.Resource.Amount!.Total!, NumberStyles.Any, CultureInfo.InvariantCulture,
                out decimal amountReceived);
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