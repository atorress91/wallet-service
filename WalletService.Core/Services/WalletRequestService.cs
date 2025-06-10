using AutoMapper;
using Newtonsoft.Json;
using WalletService.Core.Caching;
using WalletService.Core.Caching.Extensions;
using WalletService.Core.PaymentStrategies.IPaymentStrategies;
using WalletService.Core.Services.IServices;
using WalletService.Data.Adapters.IAdapters;
using WalletService.Data.Database.Models;
using WalletService.Data.Repositories.IRepositories;
using WalletService.Models.Constants;
using WalletService.Models.DTO.WalletRequestDto;
using WalletService.Models.Enums;
using WalletService.Models.Requests.WalletRequest;
using WalletService.Models.Requests.WalletRequestRequest;
using WalletService.Models.Responses;
using WalletService.Utility.Extensions;

namespace WalletService.Core.Services;

public class WalletRequestService : BaseService, IWalletRequestService
{
    private readonly IWalletRequestRepository _walletRequestRepository;
    private readonly IAccountServiceAdapter _accountServiceAdapter;
    private readonly IInvoiceRepository _invoiceRepository;
    private readonly IInvoiceDetailRepository _invoiceDetailRepository;
    private readonly IWalletRepository _walletRepository;
    private readonly IWalletPeriodRepository _walletPeriodRepository;
    private readonly IBalancePaymentStrategy _balancePaymentStrategy;
    private readonly IBrandService _brandService;
    private readonly RedisCache _redisCache;

    public WalletRequestService(
        IMapper mapper,
        IWalletRequestRepository walletRequestRepository,
        IAccountServiceAdapter accountServiceAdapter,
        IInvoiceRepository invoiceRepository,
        IInvoiceDetailRepository invoicesDetails,
        IWalletRepository walletRepository,
        IWalletPeriodRepository walletPeriodRepository, IBalancePaymentStrategy balancePaymentStrategy,
        IBrandService brandService,
        RedisCache redisCache
    ) : base(mapper)
    {
        _walletRequestRepository = walletRequestRepository;
        _accountServiceAdapter = accountServiceAdapter;
        _invoiceRepository = invoiceRepository;
        _invoiceDetailRepository = invoicesDetails;
        _walletRepository = walletRepository;
        _walletPeriodRepository = walletPeriodRepository;
        _balancePaymentStrategy = balancePaymentStrategy;
        _brandService = brandService;
        _redisCache = redisCache;
    }

    public async Task<IEnumerable<WalletRequestDto>> GetAllWalletsRequests()
    {
        var response = await _walletRequestRepository.GetAllWalletsRequests(_brandService.BrandId);
        response.Reverse();

        return Mapper.Map<IEnumerable<WalletRequestDto>>(response);
    }

    public async Task<IEnumerable<WalletRequestDto>?> GetAllWalletRequestRevertTransaction()
    {
        var response = await _walletRequestRepository.GetAllWalletRequestRevertTransaction(_brandService.BrandId);

        return Mapper.Map<IEnumerable<WalletRequestDto>>(response);
    }

    public async Task<IEnumerable<WalletRequestDto>?> GetWalletRequestById(int id)
    {
        var response = await _walletRequestRepository.GetAllWalletRequestByAffiliateId(id);

        return Mapper.Map<IEnumerable<WalletRequestDto>>(response);
    }

    public async Task<ResultResponse<WalletRequestDto>> CreateWalletRequestAsync(WalletRequestRequest request)
    {
        bool isDateValid = _brandService.BrandId switch
        {
            1 => await IsWithdrawalDateAllowed(),
            3 => IsWithdrawalUtcDateAllowed(),
            _ => true
        };

        if (!isDateValid && (_brandService.BrandId == 1 || _brandService.BrandId == 3))
            return ResultResponse<WalletRequestDto>.Fail("La fecha de retiro no está permitida");

        if (!await HasWalletAddress(request.AffiliateId))
            return ResultResponse<WalletRequestDto>.Fail("No se encontró dirección de wallet válida");
        
        var response = await _accountServiceAdapter.VerificationCode(request.VerificationCode, request.UserPassword, request.AffiliateId, _brandService.BrandId);
        if (!response.IsSuccessful)
            return ResultResponse<WalletRequestDto>.Fail("Error en la verificación del código");
        if (string.IsNullOrEmpty(response.Content))
            return ResultResponse<WalletRequestDto>.Fail("Respuesta de verificación vacía");

        var resultValid = JsonConvert.DeserializeObject<ServicesValidCodeAccountResponse>(response.Content);
        if (resultValid is { Data: false })
            return ResultResponse<WalletRequestDto>.Fail("Código de verificación inválido");
        
        var available = await _walletRepository.GetAvailableBalanceByAffiliateId(request.AffiliateId, _brandService.BrandId);
        var reverse = await _walletRepository.GetReverseBalanceByAffiliateId(request.AffiliateId, _brandService.BrandId) ?? 0;
        available -= reverse;

        if (request.Amount > available)
            return ResultResponse<WalletRequestDto>.Fail("El monto excede el saldo disponible");
        
        if (_brandService.BrandId == 2 && !await CheckFor10PercentPurchaseEarnings(request.AffiliateId))
            return ResultResponse<WalletRequestDto>.Fail("Necesita tener el 10% de lo que haya ganado en compras para retirar dinero");
        
        var walletRequest = new WalletsRequest
        {
            Amount = request.Amount,
            Concept = request.Concept!,
            Status = WalletRequestStatus.pending.ToByte(),
            AffiliateId = request.AffiliateId,
            AdminUserName = request.AffiliateName,
            CreationDate = DateTime.Now,
            OrderNumber = CommonExtensions.GenerateOrderNumber(request.AffiliateId),
            Type = nameof(WalletRequestType.withdrawal_request),
            InvoiceNumber = Constants.EmptyValue,
            BrandId = _brandService.BrandId
        };

        var created = await _walletRequestRepository.CreateWalletRequestAsync(walletRequest);
        
        await _redisCache.InvalidateBalanceAsync(request.AffiliateId);

        var dto = Mapper.Map<WalletRequestDto>(created);
        return ResultResponse<WalletRequestDto>.Ok(dto);
    }

    public async Task<ServicesResponse?> ProcessOption(int pOption, List<long> ids)
    {
        var response = new ServicesResponse();

        switch (pOption)
        {
            case 1:
                await CancelWalletRequestsAsync(ids);
                response.Success = true;
                response.Message = "The request has been processed correctly";
                break;

            case 2:
                // Lógica para procesar el pago con CoinPayment
                response.Success = true;

                break;

            case 3:
                // Lógica para procesar el pago con ConPayCR
                response.Success = true;

                break;

            case 4:
                // Lógica para procesar el pago completo
                response.Success = true;

                break;
        }

        return response;
    }

    public async Task CancelWalletRequestsAsync(List<long> ids)
    {
        var idsList = await _walletRequestRepository.GetWalletRequestsByIds(ids);

        if (idsList is { Count: 0 })
            return;

        var today = DateTime.Now;

        Parallel.ForEach(idsList, item =>
        {
            item.AttentionDate = today;
            item.UpdatedAt = today;
            item.CreatedAt = today;
            item.CreationDate = today;
            item.Status = WalletRequestStatus.cancel.ToByte();
        });

        foreach (var userId in ids)
        {
            await _redisCache.InvalidateBalanceAsync((int)userId);
        }

        await _walletRequestRepository.UpdateBulkWalletRequestsAsync(idsList.ToList());
    }

    public async Task<WalletRequestDto?> CreateWalletRequestRevert(WalletRequestRevertTransaction request)
    {
        var response = await _invoiceRepository.GetInvoiceById(request.InvoiceId, _brandService.BrandId);
        var userInfoResponse = await _accountServiceAdapter.GetUserInfo(response!.AffiliateId, _brandService.BrandId);

        var amountReverted = response.TotalInvoice * (decimal?)0.90;
        var leftOverBalance = response.TotalInvoice - amountReverted;

        var creditRequest = new CreditTransactionRequest
        {
            AffiliateId = request.AffiliateId,
            UserId = Constants.AdminUserId,
            Concept = Constants.RevertEcoPoolConcept + $" Factura# {request.InvoiceId}",
            Credit = Convert.ToDouble(amountReverted),
            AffiliateUserName = userInfoResponse!.UserName,
            AdminUserName = Constants.AdminEcosystemUserName,
            ConceptType = nameof(WalletConceptType.revert_pool),
            BrandId = _brandService.BrandId
        };

        var walletRequest = new WalletsRequest
        {
            AffiliateId = response.AffiliateId,
            AdminUserName = userInfoResponse.UserName,
            OrderNumber = CommonExtensions.GenerateOrderNumber(request.AffiliateId),
            InvoiceNumber = response.Id,
            Amount = response.TotalInvoice ?? 0,
            Concept = request.Concept,
            Type = nameof(WalletRequestType.revert_invoice_request),
            Status = WalletRequestStatus.pending.ToByte(),
            CreationDate = DateTime.Now,
            AttentionDate = null,
            BrandId = _brandService.BrandId
        };

        walletRequest = await _walletRequestRepository.CreateWalletRequestAsync(walletRequest);

        var walletMovement = await _walletRequestRepository.GetWalletRequestsByInvoiceNumber(request.InvoiceId);

        if (walletMovement == null)
            return null;

        if (walletRequest != null)
        {
            await DeleteInvoiceAndDetails(request.InvoiceId);
            await _walletRepository.CreditTransaction(creditRequest);
            await CreateCustomEcoPool(userInfoResponse, leftOverBalance);
            walletMovement.Status = WalletRequestStatus.approved.ToByte();
            await UpdateWalletRequestAsync(walletMovement);
        }


        return Mapper.Map<WalletRequestDto>(walletRequest);
    }

    private async Task<bool> CreateCustomEcoPool(UserInfoResponse user, decimal? leftOverBalance)
    {
        if (user is null)
            return false;

        var walletRequest = new WalletRequest
        {
            AffiliateId = user.Id,
            AffiliateUserName = user.UserName!,
            PurchaseFor = 0,
            Bank = Constants.WalletBalance,
            PaymentMethod = 0,
            SecretKey = null,
            ReceiptNumber = leftOverBalance.ToString(),
            BrandId = _brandService.BrandId,
            ProductsList = new List<ProductsRequests>
            {
                new ProductsRequests
                {
                    Count = 1,
                    IdProduct = 23
                }
            },
        };

        return await _balancePaymentStrategy.ExecuteCustomPayment(walletRequest);
    }

    private async Task<bool> DeleteInvoiceAndDetails(int invoiceNumber)
    {
        try
        {
            var invoiceResponse = await _invoiceRepository.GetInvoiceById(invoiceNumber, _brandService.BrandId);
            var invoiceDetailResponse = await _invoiceDetailRepository.GetInvoiceDetailByInvoiceIdAsync(invoiceNumber);

            if (invoiceResponse is null || invoiceDetailResponse.Any() == false)
                return false;

            await _invoiceRepository.DeleteInvoiceAsync(invoiceResponse);
            await _invoiceDetailRepository.DeleteBulkInvoiceDetailAsync(invoiceDetailResponse);

            return true;
        }

        catch (Exception)
        {
            return false;
        }
    }

    private async Task UpdateWalletRequestAsync(WalletsRequest walletRequest)
    {
        walletRequest.AttentionDate = DateTime.Now;
        await _walletRequestRepository.UpdateWalletRequestsAsync(walletRequest);
    }

    public async Task<bool> AdministrativePaymentAsync(WalletsRequest[] requests)
    {
        if (requests.Length is 0)
            return false;

        var userIds = requests.Select(x => x.AffiliateId).Distinct().ToList();

        var tasks = userIds.Select(id => _accountServiceAdapter.GetUserInfo(id, _brandService.BrandId)).ToArray();
        var userInfoArray = await Task.WhenAll(tasks);
        var userInfoList = userInfoArray.Where(u => u != null).ToList();
        var today = DateTime.Now;

        var walletsList = new List<Wallet>();
        var idsList = new List<WalletsRequest>();

        foreach (var user in userInfoList)
        {
            var correspondingRequest = requests.FirstOrDefault(r => r.AffiliateId == user!.Id);
            if (correspondingRequest != null)
            {
                var walletEntry = new Wallet
                {
                    AffiliateId = user!.Id,
                    AffiliateUserName = user.UserName,
                    AdminUserName = Constants.AdminEcosystemUserName,
                    UserId = Constants.AdminUserId,
                    Credit = Constants.EmptyValue,
                    Debit = correspondingRequest.Amount,
                    Deferred = Constants.EmptyValue,
                    Status = true,
                    Concept = Constants.WithdrawalBalance,
                    ConceptType = nameof(WalletConceptType.wallet_withdrawal_request),
                    Support = Constants.EmptyValue,
                    Date = today,
                    Compression = false,
                    Detail = null,
                    CreatedAt = today,
                    UpdatedAt = today,
                    DeletedAt = null,
                    BrandId = _brandService.BrandId,
                };

                walletsList.Add(walletEntry);
                idsList.Add(correspondingRequest);
            }
        }

        if (!idsList.Any())
            return false;

        var result = await _walletRepository.BulkAdministrativeDebitTransaction(walletsList.ToArray());

        if (!result)
            return false;

        Parallel.ForEach(idsList, item =>
        {
            item.AttentionDate = today;
            item.UpdatedAt = today;
            item.Status = WalletRequestStatus.approved.ToByte();
        });

        await _walletRequestRepository.UpdateBulkWalletRequestsAsync(idsList);

        return true;
    }

    private async Task<bool> IsWithdrawalDateAllowed()
    {
        var defaultZone = Constants.DefaultWithdrawalZone;

        var timeZone = TimeZoneInfo.FindSystemTimeZoneById(defaultZone);
        var localDateTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZone);

        if (localDateTime.TimeOfDay < new TimeSpan(8, 0, 0) || localDateTime.TimeOfDay > new TimeSpan(18, 0, 0))
            return false;

        var allowedDatesObjects = await _walletPeriodRepository.GetAllWalletsPeriods();

        var allowedDates = allowedDatesObjects.Select(wp => wp.Date).ToList();

        var localDateOnly = DateOnly.FromDateTime(localDateTime.Date);
        return allowedDates.Contains(localDateOnly);
    }

    private bool IsWithdrawalUtcDateAllowed()
    {
        var utcNow = DateTime.UtcNow;

        if (utcNow.DayOfWeek != DayOfWeek.Friday)
            return false;

        var startTime = new TimeSpan(0, 0, 0);
        var endTime = new TimeSpan(24, 0, 0);


        var currentTime = utcNow.TimeOfDay;
        return currentTime >= startTime && currentTime <= endTime;
    }

    private async Task<bool> HasWalletAddress(int affiliateId)
    {
        var response = await _accountServiceAdapter.GetAffiliateBtcByAffiliateId(affiliateId, _brandService.BrandId);

        if (response.Content is null)
            return false;

        var userInfo = JsonConvert.DeserializeObject<AffiliateBtcResponse>(response.Content);

        if (userInfo?.Data is null)
            return false;

        if (userInfo.Data.Length == 0)
            return false;

        return true;
    }

    private async Task<bool> CheckFor10PercentPurchaseEarnings(int affiliateId)
    {
        var commissions = await _walletRepository.GetTotalCommissionsPaid(affiliateId, _brandService.BrandId);
        var purchases = await _walletRepository.GetTotalAcquisitionsByAffiliateId(affiliateId, _brandService.BrandId);

        var minimumPurchaseRequired = commissions * 0.10m;

        return purchases >= minimumPurchaseRequired;
    }
}