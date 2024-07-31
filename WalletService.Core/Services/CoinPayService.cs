using System.Net;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using WalletService.Core.PaymentStrategies.IPaymentStrategies;
using WalletService.Core.Services.IServices;
using WalletService.Data.Adapters.IAdapters;
using WalletService.Data.Database.Models;
using WalletService.Data.Repositories.IRepositories;
using WalletService.Models.Constants;
using WalletService.Models.DTO.CoinPayDto;
using WalletService.Models.Enums;
using WalletService.Models.Requests.CoinPayRequest;
using WalletService.Models.Requests.ConPaymentRequest;
using WalletService.Models.Requests.WalletRequest;
using WalletService.Models.Requests.WalletWithDrawalRequest;
using WalletService.Models.Responses;
using WalletService.Models.Responses.BaseResponses;
using WalletService.Utility.Extensions;
using JsonSerializer = System.Text.Json.JsonSerializer;

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
    private readonly IWalletRequestRepository _walletRequestRepository;
    private readonly IWalletWithdrawalService _walletWithDrawalService;

    public CoinPayService(IMapper mapper, ICoinPayAdapter coinPayAdapter,
        ICoinPaymentTransactionRepository transactionRepository, ILogger<CoinPayService> logger,
        IAccountServiceAdapter accountServiceAdapter,
        IInvoiceRepository invoiceRepository,
        ICoinPayPaymentStrategy coinPayPaymentStrategy,
        IInventoryServiceAdapter inventoryServiceAdapter,
        IWalletRequestRepository walletRequestRepository,
        IWalletWithdrawalService walletWithDrawalService
    ) : base(mapper)
    {
        _coinPayAdapter = coinPayAdapter;
        _transactionRepository = transactionRepository;
        _logger = logger;
        _accountServiceAdapter = accountServiceAdapter;
        _invoiceRepository = invoiceRepository;
        _coinPayPaymentStrategy = coinPayPaymentStrategy;
        _inventoryServiceAdapter = inventoryServiceAdapter;
        _walletRequestRepository = walletRequestRepository;
        _walletWithDrawalService = walletWithDrawalService;
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
        _logger.LogInformation("Starting to create a channel: {RequestDetails}", request.ToJsonString());

        var today = DateTime.Now;
        PaymentTransaction? transactionResponse;

        var channelRequest = new CreateChannelRequest
        {
            IdCurrency = Constants.UsdtIdCurrency,
            IdExternalIdentification = CommonExtensions.GenerateUniqueId(request.AffiliateId),
            IdNetwork = Constants.UsdtIdNetwork,
            TagName = request.UserName,
        };

        var channelResponse = await _coinPayAdapter.CreateChannel(channelRequest);
        _logger.LogInformation("Channel creation response received: {ChannelResponse}", channelResponse.ToJsonString());

        if (channelResponse.Content is null)
        {
            _logger.LogWarning(
                "No content received in response to channel creation request for Affiliate ID: {AffiliateId}",
                request.AffiliateId);
            return null;
        }

        var channel = channelResponse.Content.ToJsonObject<CreateChannelResponse>();
        if (channel is null || channel.StatusCode != 1)
        {
            _logger.LogWarning(
                "Channel creation failed or did not return status code 1 for Affiliate ID: {AffiliateId}. Status code received: {StatusCode}",
                request.AffiliateId, channel?.StatusCode);
            return null;
        }

        var existingTransaction =
            await _transactionRepository.GetCoinPaymentTransactionByIdTransaction(channel.Data!.Id.ToString());
        _logger.LogInformation("Checking for existing transaction with ID: {TransactionId}", channel.Data!.Id);

        var paymentTransaction = new PaymentTransaction
        {
            IdTransaction = channel.Data.Id.ToString(),
            AffiliateId = request.AffiliateId,
            Amount = request.Amount,
            AmountReceived = Constants.EmptyValue,
            Products = JsonSerializer.Serialize(request.Products),
            Acredited = false,
            Status = Constants.EmptyValue,
            PaymentMethod = Constants.CoinPay,
            CreatedAt = today,
            UpdatedAt = today,
            Reference = channelRequest.IdExternalIdentification.ToString()
        };

        if (existingTransaction != null && !existingTransaction.Acredited)
        {
            _logger.LogInformation("Updating an unaccredited existing transaction for Transaction ID: {TransactionId}",
                existingTransaction.IdTransaction);
            existingTransaction.Amount = request.Amount;
            existingTransaction.Products = JsonSerializer.Serialize(request.Products);
            existingTransaction.UpdatedAt = today;
            transactionResponse = await _transactionRepository.UpdateCoinPaymentTransactionAsync(existingTransaction);
        }
        else
        {
            _logger.LogInformation("Creating a new transaction for newly created channel with ID: {TransactionId}",
                paymentTransaction.IdTransaction);
            transactionResponse = await _transactionRepository.CreateCoinPaymentTransaction(paymentTransaction);
        }

        if (transactionResponse == null)
        {
            _logger.LogError("Failed to record transaction details for Affiliate ID: {AffiliateId}",
                request.AffiliateId);
            return null;
        }

        _logger.LogInformation(
            "Channel creation and transaction recording completed successfully for Affiliate ID: {AffiliateId}",
            request.AffiliateId);
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

    public async Task<bool> ReceiveCoinPayNotifications(string requestBody)
    {
        _logger.LogInformation(
            $"[CoinPayService] | ReceiveCoinPayNotifications | Starting transaction status update | Request ID: {requestBody.ToJsonString()}");

        if (string.IsNullOrEmpty(requestBody))
        {
            _logger.LogWarning($"[CoinPayService] | ReceiveCoinPayNotifications | Received a null request");
            return false;
        }

        var request = JsonConvert.DeserializeObject<WebhookNotificationRequest>(requestBody);
        _logger.LogDebug(
            $"[CoinPayService] | ReceiveCoinPayNotifications | Processing request: {request!.ToJsonString()}");

        if (request is null)
            return false;


        if (request.UserChannel?.IdExternalIdentification is null)
        {
            _logger.LogWarning(
                "[CoinPayService] | ReceiveCoinPayNotifications | Request UserChannel or IdExternalIdentification is null");
            return false;
        }

        var existingTransaction =
            await _transactionRepository.GetTransactionByReference(request.UserChannel.IdExternalIdentification);
        if (existingTransaction is null)
        {
            _logger.LogWarning(
                $"[CoinPayService] | ReceiveCoinPayNotifications | No existing transaction found for IdExternalIdentification: {request.UserChannel.IdExternalIdentification}");
            return false;
        }

        _logger.LogInformation(
            $"[CoinPayService] | ReceiveCoinPayNotifications | Found existing transaction | Status: {existingTransaction.Status}, Acredited: {existingTransaction.Acredited}");

        if (existingTransaction.Acredited || existingTransaction.Status == Constants.CompletedStatusCode)
        {
            _logger.LogInformation(
                $"[CoinPayService] | ReceiveCoinPayNotifications | Transaction already completed or accredited | Status: {existingTransaction.Status}, Reference: {existingTransaction.Reference}");
            return false;
        }

        switch (request.TransactionStatus.Id)
        {
            case Constants.CoinPayPendingStatusCode:
                existingTransaction.Status = Constants.RegisteredStatusCode;
                existingTransaction.Acredited = false;
                existingTransaction.IdTransaction = request.IdTransaction.ToString();
                _logger.LogInformation(
                    $"[CoinPayService] | ReceiveCoinPayNotifications | Transaction set to registered status | Reference: {existingTransaction.Reference}");
                break;

            case Constants.CoinPayExpiredStatusCode:
                existingTransaction.Status = Constants.ExpiredStatusCode;
                existingTransaction.Acredited = false;
                _logger.LogInformation(
                    $"[CoinPayService] | ReceiveCoinPayNotifications | Transaction marked as expired | Reference: {existingTransaction.Reference}");
                break;

            case Constants.CoinPaySuccessStatusCode:
                existingTransaction.Status = Constants.CompletedStatusCode;
                existingTransaction.AmountReceived = request.Amount;
                existingTransaction.Acredited = true;
                existingTransaction.IdTransaction = request.IdTransaction.ToString();

                _logger.LogInformation(
                    $"[CoinPayService] | ReceiveCoinPayNotifications | Transaction marked as successful | Amount: {request.Amount} | Reference: {existingTransaction.Reference}");
                await ProcessPaymentTransaction(existingTransaction);
                _logger.LogInformation(
                    $"[CoinPayService] | ReceiveCoinPayNotifications | Payment processing completed | Reference: {existingTransaction.Reference}");
                break;

            default:
                _logger.LogWarning(
                    $"[CoinPayService] | ReceiveCoinPayNotifications | Unknown transaction status ID: {request.TransactionStatus.Id}");
                break;
        }

        await _transactionRepository.UpdateCoinPaymentTransactionAsync(existingTransaction);
        _logger.LogInformation(
            $"[CoinPayService] | ReceiveCoinPayNotifications | Transaction status updated in the database | Reference: {existingTransaction.Reference}");

        return true;
    }

    public async Task<SendFundsDto?> SendFunds(WithDrawalRequest[] requests)
    {
        _logger.LogInformation(
            $"[CoinPayService] | SendFunds | Start processing {requests.Length} withdrawal requests.");

        if (requests.Length == 0)
        {
            _logger.LogInformation($"[CoinPayService] | SendFunds | No withdrawal requests to process.");
            return new SendFundsDto();
        }

        var response = new SendFundsDto();
        var successfulIds = new List<int>();

        foreach (var request in requests)
        {
            _logger.LogInformation(
                $"[CoinPayService] | SendFunds | Starting transaction for request : {request.ToJsonString()}");

            try
            {
                var withdrawalResponse = await ProcessWithdrawal(request);

                if (withdrawalResponse.StatusCode == 1)
                {
                    _logger.LogInformation(
                        $"[CoinPayService] | SendFunds | Successful Process Withdrawal for request ID: {request.Id}");
                    response.SuccessfulResponses.Add(withdrawalResponse);
                    successfulIds.Add(request.Id);
                }
                else
                {
                    _logger.LogInformation(
                        $"[CoinPayService] | SendFunds | Failed Process Withdrawal for request ID: {request.Id}");
                    response.FailedResponses.Add(withdrawalResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    $"[CoinPayService] | SendFunds | Error processing withdrawal for request ID: {request.Id} with error: {ex}");
            }
        }

        if (successfulIds.Count != Constants.EmptyValue)
        {
            await UpdateSuccessfulWithdrawals(successfulIds);
            _logger.LogInformation(
                $"[CoinPayService] | SendFunds | Successfully updated withdrawal records for IDs: {string.Join(", ", successfulIds)}");
        }

        _logger.LogInformation($"[CoinPayService] | SendFunds | Completed processing withdrawal requests.");

        return response;
    }

    private async Task<SendFundsResponse> ProcessWithdrawal(WithDrawalRequest request)
    {
        _logger.LogInformation(
            $"[CoinPayService] | ProcessWithdrawal | Starting withdrawal for Affiliate ID: {request.AffiliateId}");

        var user = await _accountServiceAdapter.GetUserInfo(request.AffiliateId);
        var trcResponse = await _accountServiceAdapter.GetAffiliateBtcByAffiliateId(request.AffiliateId);

        TrcAddressResponse? userTrcAddress = null;

        _logger.LogInformation(
            $"[CoinPayService] | ProcessWithdrawal | TRC response received for Affiliate ID: {request.AffiliateId}");

        var deserializeResponse = JsonConvert.DeserializeObject<ServicesResponse>(trcResponse.Content!);
        if (deserializeResponse!.Data != null)
        {
            userTrcAddress = JsonConvert.DeserializeObject<TrcAddressResponse>(deserializeResponse.Data.ToString()!);
            _logger.LogInformation(
                $"[CoinPayService] | ProcessWithdrawal | TRC address deserialized successfully for Affiliate ID: {request.AffiliateId}");
        }

        if (user == null)
        {
            _logger.LogWarning(
                $"[CoinPayService] | ProcessWithdrawal | User not found for Affiliate ID: {request.AffiliateId}");
            return new SendFundsResponse
            {
                StatusCode = (int)HttpStatusCode.NotFound,
                Message = "User not found"
            };
        }

        if (userTrcAddress == null)
        {
            _logger.LogWarning(
                $"[CoinPayService] | ProcessWithdrawal | No valid TRC address found for Affiliate ID: {request.AffiliateId}");
            return new SendFundsResponse
            {
                StatusCode = (int)HttpStatusCode.BadRequest,
                Message = "No valid TRC address found"
            };
        }

        var sendFundsRequest = new SendFundRequest
        {
            IdCurrency = Constants.UsdtIdCurrency,
            IdNetwork = Constants.UsdtIdNetwork,
            Address = userTrcAddress.Address,
            Amount = request.Amount,
            Details = Constants.SendFundsConcept,
            AmountPlusFee = false
        };

        _logger.LogInformation(
            $"[CoinPayService] | ProcessWithdrawal | Sending funds for Affiliate ID: {request.AffiliateId}");

        var response = await _coinPayAdapter.SendFunds(sendFundsRequest);
        if (response is { IsSuccessful: true, Content: not null })
        {
            var servicesResponse = JsonConvert.DeserializeObject<SendFundsResponse>(response.Content);
            var today = DateTime.Now;
            var walletWithdrawal = new WalletWithDrawalRequest
            {
                AffiliateId = user.Id,
                AffiliateUserName = user.UserName!,
                Amount = sendFundsRequest.Amount,
                IsProcessed = true,
                Observation = $"{Constants.SendFundsConcept} {request.Id}",
                AdminObservation =
                    $"{servicesResponse?.Data?.IdTransaction} - {servicesResponse?.Data?.Address ?? "No Address"}",
                Date = today,
                ResponseDate = today,
                RetentionPercentage = Constants.EmptyValue,
                Status = true
            };

            if (servicesResponse is { StatusCode: 1 })
            {
                _logger.LogInformation(
                    $"[CoinPayService] | ProcessWithdrawal | Funds sent successfully for Affiliate ID: {request.AffiliateId}");
                await _walletWithDrawalService.CreateWalletWithdrawalAsync(walletWithdrawal);
                return servicesResponse;
            }
            else
            {
                _logger.LogWarning(
                    $"[CoinPayService] | ProcessWithdrawal | Successful operation but invalid response format for Affiliate ID: {request.AffiliateId} {response.ToJsonString()}");
                return new SendFundsResponse
                {
                    StatusCode = (int)response.StatusCode,
                    Message = "The operation was successful, but the response format is invalid."
                };
            }
        }

        _logger.LogWarning(
            $"[CoinPayService] | ProcessWithdrawal | Failed to send funds for Affiliate ID: {request.AffiliateId} {response.ToJsonString()}");
        return new SendFundsResponse
        {
            StatusCode = (int)response.StatusCode,
            Message = "Failed to send funds"
        };
    }

    private async Task UpdateSuccessfulWithdrawals(List<int> successfulWithdrawalIds)
    {
        _logger.LogInformation(
            $"[WalletService] | UpdateSuccessfulWithdrawals | Starting update for {successfulWithdrawalIds.ToJsonString()} successful withdrawals.");
        var withdrawals = await _walletRequestRepository.GetWalletRequestsByIds(successfulWithdrawalIds);

        foreach (var withdrawal in withdrawals)
        {
            _logger.LogDebug(
                $"[WalletService] | UpdateSuccessfulWithdrawals | Updating status for withdrawal ID: {withdrawal.Id}");
            withdrawal.Status = (int)WithdrawalStatus.Completed;
            withdrawal.UpdatedAt = DateTime.Now;
        }

        await _walletRequestRepository.UpdateBulkWalletRequestsAsync(withdrawals);
        _logger.LogInformation($"[WalletService] | UpdateSuccessfulWithdrawals | Successfully updated withdrawals.");
    }

    public async Task<bool> GetTransactionByReference(string request)
    {
        if (string.IsNullOrEmpty(request))
            return false;
        
        var reference = await _transactionRepository.GetTransactionByReference(request);

        if (reference is null)
            return false;

        if (reference is { Acredited: true, Status: Constants.CompletedStatusCode })
        {
            return true;
        }

        return false;
    }

    #endregion
}