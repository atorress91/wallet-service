using AutoMapper;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using Microsoft.IdentityModel.Tokens;
using WalletService.Core.PaymentStrategies.IPaymentStrategies;
using WalletService.Core.Services.IServices;
using WalletService.Data.Adapters.IAdapters;
using WalletService.Data.Database.Models;
using WalletService.Data.Repositories.IRepositories;
using WalletService.Models.Constants;
using WalletService.Models.Enums;
using WalletService.Models.Requests.CoinPayRequest;
using WalletService.Models.Requests.ConPaymentRequest;
using WalletService.Models.Requests.WalletRequest;
using WalletService.Models.Responses;
using WalletService.Models.Responses.BaseResponses;
using WalletService.Utility.Extensions;

namespace WalletService.Core.Services;

public class CoinPayService : BaseService, ICoinPayService
{
    private readonly ICoinPayAdapter _coinPayAdapter;
    private readonly ICoinPaymentTransactionRepository _transactionRepository;
    private readonly ILogger<CoinPayService> _logger;
    private readonly IAccountServiceAdapter _accountServiceAdapter;
    private readonly IInvoiceRepository _invoiceRepository;
    private readonly ICoinPayPaymentStrategy _coinPayPaymentStrategy;
    private readonly IInventoryServiceAdapter _inventoryServiceAdapter;
    public CoinPayService(IMapper mapper, ICoinPayAdapter coinPayAdapter,
        ICoinPaymentTransactionRepository transactionRepository, ILogger<CoinPayService> logger,
        IAccountServiceAdapter accountServiceAdapter,
        IInvoiceRepository invoiceRepository,
        ICoinPayPaymentStrategy coinPayPaymentStrategy,
        IInventoryServiceAdapter inventoryServiceAdapter) : base(mapper)
    {
        _coinPayAdapter = coinPayAdapter;
        _transactionRepository = transactionRepository;
        _logger = logger;
        _accountServiceAdapter = accountServiceAdapter;
        _invoiceRepository = invoiceRepository;
        _coinPayPaymentStrategy = coinPayPaymentStrategy;
        _inventoryServiceAdapter = inventoryServiceAdapter;
    }

    #region coingPay

     private async Task ExecuteMembershipPayment(WalletRequest walletRequest, ICollection<ProductRequest> products)
    {
        walletRequest.ProductsList = products.Select(product => new ProductsRequests
        {
            IdProduct = product.ProductId,
            Count = product.Quantity
        }).ToList();

        await _coinPayPaymentStrategy.ExecuteMembershipPayment(walletRequest);
    }

    private async Task ExecuteEcoPoolPayment(WalletRequest walletRequest, ICollection<ProductRequest> products)
    {
        walletRequest.ProductsList = products.Select(product => new ProductsRequests
        {
            IdProduct = product.ProductId,
            Count = product.Quantity
        }).ToList();

        await _coinPayPaymentStrategy.ExecuteEcoPoolPayment(walletRequest);
    }

    private async Task ExecuteCoursePayment(WalletRequest walletRequest, ICollection<ProductRequest> products)
    {
        walletRequest.ProductsList = products.Select(product => new ProductsRequests
        {
            IdProduct = product.ProductId,
            Count = product.Quantity
        }).ToList();

        await _coinPayPaymentStrategy.ExecuteCoursePayment(walletRequest);
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
            $"[CoinPayService] | ProcessPaymentTransaction | transactionResult: {transactionResult.ToJsonString()}");

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

        _logger.LogInformation($"[CoinPayService] | ProcessPaymentTransaction | products: {products.ToJsonString()}");

        var isExists = await _invoiceRepository.InvoiceExistsByReceiptNumber(transactionResult.IdTransaction);
        if (isExists)
            return;

        var productType = await GetProductType(products);

        switch (productType)
        {
            case ProductType.Membership:
                await ExecuteMembershipPayment(walletRequest, products);
                _logger.LogInformation($"[CoinPayService] | ProcessPaymentTransaction | Membership Payment executed");
                break;
            case ProductType.EcoPool:
                await ExecuteEcoPoolPayment(walletRequest, products);
                _logger.LogInformation($"[CoinPayService] | ProcessPaymentTransaction | EcoPool Payment executed");
                break;
            case ProductType.Course:
                await ExecuteCoursePayment(walletRequest, products);
                _logger.LogInformation($"[CoinPayService] | ProcessPaymentTransaction | Course Payment executed");
                break;
        }
    }

    public async Task<CreateTransactionResponse?> CreateTransaction(CreateTransactionRequest request)
    {
        var today = DateTime.Now;
        var paymentRequest = new PaymentRequest
        {
            Amount = request.Amount,
            IdCurrency = Constants.UsdtIdCurrency,
            Details = JsonSerializer.Serialize(request.Products)
        };

        var response = await _coinPayAdapter.CreateTransaction(paymentRequest);

        if (response.Content is null)
            return new CreateTransactionResponse();

        var result = response.Content!.ToJsonObject<CreateTransactionResponse>() ??
                     new CreateTransactionResponse();

        var paymentTransaction = new PaymentTransaction
        {
            IdTransaction = result.Data!.IdTransaction.ToString(),
            AffiliateId = request.AffiliateId,
            Amount = result.Data!.Amount,
            AmountReceived = Constants.EmptyValue,
            Products = result.Data.Details!,
            Acredited = false,
            Status = result.StatusCode,
            PaymentMethod = "CoinPay",
            CreatedAt = today,
            UpdatedAt = today
        };

        await _transactionRepository.CreateCoinPaymentTransaction(paymentTransaction);

        return result;
    }

    public async Task<CreateChannelResponse?> CreateChannel(CreateTransactionRequest request)
    {
        var today = DateTime.Now;
        PaymentTransaction? transactionResponse;

        var channelRequest = new CreateChannelRequest
        {
            IdCurrency = Constants.UsdtIdCurrency,
            IdExternalIdentification = Guid.NewGuid().GetHashCode(),
            IdNetwork = Constants.UsdtIdNetwork,
            TagName = request.UserName,
        };

        var channelResponse = await _coinPayAdapter.CreateChannel(channelRequest);

        if (channelResponse.Content is null)
            return null;

        var channel = channelResponse.Content.ToJsonObject<CreateChannelResponse>();

        if (channel is null)
            return null;

        if (channel.StatusCode != 1)
            return null;

        var existingTransaction =
            await _transactionRepository.GetCoinPaymentTransactionByIdTransaction(channel.Data!.Id.ToString());

        var paymentTransaction = new PaymentTransaction
        {
            IdTransaction = channel.Data!.Id.ToString(),
            AffiliateId = request.AffiliateId,
            Amount = request.Amount,
            AmountReceived = Constants.EmptyValue,
            Products = JsonSerializer.Serialize(request.Products),
            Acredited = false,
            Status = Constants.EmptyValue,
            PaymentMethod = "CoinPay",
            CreatedAt = today,
            UpdatedAt = today,
            Reference = channelRequest.IdExternalIdentification.ToString()
        };

        if (existingTransaction != null && existingTransaction.Acredited == false)
        {
            existingTransaction.Amount = request.Amount;
            existingTransaction.Products = JsonSerializer.Serialize(request.Products);
            existingTransaction.UpdatedAt = today;

            transactionResponse = await _transactionRepository.UpdateCoinPaymentTransactionAsync(existingTransaction);
        }
        else
        {
            transactionResponse = await _transactionRepository.CreateCoinPaymentTransaction(paymentTransaction);
        }

        if (transactionResponse is null)
            return null;

        return channel;
    }

    public async Task<GetNetworkResponse?> GetNetworksByIdCurrency(int idCurrency)
    {
        var result = await _coinPayAdapter.GetNetworksByIdCurrency(idCurrency);

        if (result.Content is null)
            return null;

        var networkResult = result.Content.ToJsonObject<GetNetworkResponse>();

        return networkResult;
    }

    public async Task<CreateAddressResponse?> CreateAddress(CreateAddresRequest request)
    {
        var result = await _coinPayAdapter.CreateAddress(Constants.CoinPayIdWallet, request);

        if (result.Content is null)
            return null;

        var address = result.Content.ToJsonObject<CreateAddressResponse>();

        if (address is null)
            return null;

        return address;
    }

    public async Task<GetTransactionByIdResponse?> GetTransactionById(int idTransaction)
    {
        var result = await _coinPayAdapter.GetTransactionById(idTransaction);

        if (result.Content is null)
            return null;

        var transaction = result.Content.ToJsonObject<GetTransactionByIdResponse>();

        return transaction;
    }

    public async Task<bool> ReceiveCoinPayNotifications(WebhookNotificationRequest? request)
    {
        _logger.LogInformation($"[CoinPayService] | UpdateTransactionStatus | Starting transaction status update");

        if (request is null)
        {
            _logger.LogWarning($"[CoinPayService] | UpdateTransactionStatus | request is null");
            return false;
        }

        var existingTransaction = await _transactionRepository.GetTransactionByReference(request.IdExternalReference);

        if (existingTransaction is null)
        {
            _logger.LogWarning($"[CoinPayService] | UpdateTransactionStatus | No existing transaction found");
            return false;
        }

        if (existingTransaction.Acredited)
        {
            _logger.LogInformation(
                $"[CoinPayService] | UpdateTransactionStatus | Existing transaction is already accredited");
            return false;
        }

        if (request.TransactionStatus.Id == Constants.CoinPayPendingStatusCode)
        {
            existingTransaction.Status = Constants.RegisteredStatusCode;
            existingTransaction.Acredited = false;
        }
        else if (request.TransactionStatus.Id == Constants.CoinPayExpiredStatusCode)
        {
            existingTransaction.Status = Constants.ExpiredStatusCode;
            existingTransaction.Acredited = false;
        }
        else if (request.TransactionStatus.Id == Constants.CoinPaySuccessStatusCode &&
                 existingTransaction.Acredited == false)
        {
            existingTransaction.Status = Constants.CompletedStatusCode;
            existingTransaction.AmountReceived = (decimal)request.Amount;
            existingTransaction.Acredited = true;
            existingTransaction.Reference = request.IdExternalReference;
            await ProcessPaymentTransaction(existingTransaction);
            _logger.LogInformation($"[CoinPayService] | UpdateTransactionStatus | Transaction processed");
        }

        await _transactionRepository.UpdateCoinPaymentTransactionAsync(existingTransaction);
        _logger.LogInformation($"[CoinPayService] | UpdateTransactionStatus | Transaction status updated");

        return true;
    }

    #endregion
}