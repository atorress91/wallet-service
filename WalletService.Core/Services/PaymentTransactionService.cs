using AutoMapper;
using Newtonsoft.Json;
using WalletService.Core.PaymentStrategies.IPaymentStrategies;
using WalletService.Core.Services.IServices;
using WalletService.Data.Adapters.IAdapters;
using WalletService.Data.Database.Models;
using WalletService.Data.Repositories.IRepositories;
using WalletService.Models.Constants;
using WalletService.Models.DTO.PaymentTransactionDto;
using WalletService.Models.Enums;
using WalletService.Models.Requests.ConPaymentRequest;
using WalletService.Models.Requests.PaymentTransaction;
using WalletService.Models.Requests.WalletRequest;
using WalletService.Models.Responses;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace WalletService.Core.Services;

public class PaymentTransactionService : BaseService, IPaymentTransactionService
{
    private readonly ICoinPaymentTransactionRepository _paymentTransactionRepository;
    private readonly IAccountServiceAdapter            _accountServiceAdapter;
    private readonly IInventoryServiceAdapter          _inventoryServiceAdapter;
    private readonly IWireTransferStrategy             _wireTransferStrategy;

    public PaymentTransactionService(IMapper mapper,                ICoinPaymentTransactionRepository paymentTransactionRepository,
        IAccountServiceAdapter               accountServiceAdapter, IInventoryServiceAdapter          inventoryServiceAdapter,
        IWireTransferStrategy                wireTransferStrategy) :
        base(mapper)
    {
        _paymentTransactionRepository = paymentTransactionRepository;
        _accountServiceAdapter        = accountServiceAdapter;
        _inventoryServiceAdapter      = inventoryServiceAdapter;
        _wireTransferStrategy         = wireTransferStrategy;
    }

    public async Task<PaymentTransactionDto?> CreatePaymentTransactionAsync(PaymentTransactionRequest request)
    {
        var user = await _accountServiceAdapter.GetUserInfo(request.AffiliateId);

        if (user is null)
            return null;

        var transaction = Mapper.Map<PaymentTransaction>(request);

        transaction.Acredited      = false;
        transaction.Status         = 0;
        transaction.AmountReceived = 0;
        transaction.PaymentMethod  = "wire_transfer";

        var response = await _paymentTransactionRepository.CreateCoinPaymentTransaction(transaction);

        if (response is null)
            return null;

        var transactionResponse = new PaymentTransactionDto
        {
            Id             = response.Id,
            IdTransaction  = response.IdTransaction,
            AffiliateId    = response.AffiliateId,
            Amount         = response.Amount,
            AmountReceived = response.AmountReceived,
            Products       = response.Products,
            Status         = response.Status,
            Acredited      = response.Acredited,
            CreatedAt      = response.CreatedAt,
            UpdatedAt      = response.UpdatedAt,
            DeletedAt      = response.DeletedAt,
            PaymentMethod  = response.PaymentMethod
        };

        return transactionResponse;
    }

    public async Task<IEnumerable<PaymentTransactionDto>> GetAllWireTransfer()
    {
        var transactions       = await _paymentTransactionRepository.GetAllWireTransfer();
        var uniqueAffiliateIds = transactions.Select(trans => trans.AffiliateId).Distinct();

        var userTasks     = uniqueAffiliateIds.Select(id => _accountServiceAdapter.GetUserInfo(id)).ToList();
        var userResponses = await Task.WhenAll(userTasks);

        var usersInfo = userResponses
            .Where(user => user != null)
            .ToDictionary(user => user!.Id, user => user);

        if (!usersInfo.Any())
            return Enumerable.Empty<PaymentTransactionDto>();

        var transactionsDto = transactions.Select(trans => new PaymentTransactionDto
        {
            Id             = trans.Id,
            IdTransaction  = trans.IdTransaction,
            AffiliateId    = trans.AffiliateId,
            Amount         = trans.Amount,
            AmountReceived = trans.AmountReceived,
            Products       = trans.Products,
            Status         = trans.Status,
            Acredited      = trans.Acredited,
            CreatedAt      = trans.CreatedAt,
            UpdatedAt      = trans.UpdatedAt,
            DeletedAt      = trans.DeletedAt,
            PaymentMethod  = trans.PaymentMethod,
            UserName       = usersInfo.TryGetValue(trans.AffiliateId, out var value) ? value!.UserName : null
        }).ToList();

        return transactionsDto;
    }

    public async Task<bool> ConfirmPayment(ConfirmPaymentTransactionRequest request)
    {
        var paymentTransaction = await _paymentTransactionRepository.GetPaymentTransactionById(request.Id);

        if (paymentTransaction is null)
            return false;

        var products = JsonConvert.DeserializeObject<List<ProductRequest>>(paymentTransaction.Products);
        if (products is null) return false;

        var walletRequest = BuildWalletRequest(paymentTransaction, request);

        var  productType = await GetProductType(products);
        bool paymentResult;

        switch (productType)
        {
            case ProductType.EcoPool:
                paymentResult = await ExecuteEcoPoolPayment(walletRequest);
                break;
            default:
                paymentResult = await ExecuteCoursePayment(walletRequest);
                break;
        }

        if (paymentResult)
        {
            return await UpdatePaymentTransaction(paymentTransaction);
        }

        return false;
    }

    private WalletRequest BuildWalletRequest(PaymentTransaction payment, ConfirmPaymentTransactionRequest request)
    {
        var products = JsonConvert.DeserializeObject<List<ProductRequest>>(payment.Products);

        return new WalletRequest
        {
            AffiliateId       = payment.AffiliateId,
            AffiliateUserName = request.UserName,
            PurchaseFor       = Constants.None,
            Bank              = Constants.WireTransfer,
            PaymentMethod     = Constants.None,
            SecretKey         = null,
            ReceiptNumber     = payment.IdTransaction,
            ProductsList = products?.Select(p => new ProductsRequests
            {
                IdProduct = p.ProductId,
                Count     = p.Quantity
            }).ToList() ?? new List<ProductsRequests>()
        };
    }

    private async Task<ProductType> GetProductType(List<ProductRequest> request)
    {
        var productIds   = request.Select(p => p.ProductId).ToArray();
        var responseList = await _inventoryServiceAdapter.GetProductsIds(productIds);

        var result = JsonSerializer.Deserialize<ProductsResponse>(responseList.Content!);

        var firstProductCategory = result!.Data.First().PaymentGroup;

        switch (firstProductCategory)
        {
            case 1: return ProductType.Membership;
            case 2: return ProductType.EcoPool;
            default:
                return ProductType.Course;
        }
    }

    private async Task<bool> ExecuteEcoPoolPayment(WalletRequest walletRequest)
    {
        
        try
        {
            await _wireTransferStrategy.ExecuteEcoPoolPayment(walletRequest);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    private async Task<bool> ExecuteCoursePayment(WalletRequest walletRequest)
    {
        try
        {
            await _wireTransferStrategy.ExecutePaymentCourses(walletRequest);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    private async Task<bool> UpdatePaymentTransaction(PaymentTransaction payment)
    {
        payment.Acredited      = true;
        payment.Status         = 100;
        payment.AmountReceived = payment.Amount;

        var transaction = await _paymentTransactionRepository.UpdateCoinPaymentTransactionAsync(payment);

        return transaction is not null;
    }
}