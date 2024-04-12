using AutoMapper;
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
    private readonly IAccountServiceAdapter   _accountServiceAdapter;
    private readonly IInvoiceRepository       _invoiceRepository;
    private readonly IInvoiceDetailRepository _invoiceDetailRepository;
    private readonly IWalletRepository        _walletRepository;
    private readonly IWalletPeriodRepository  _walletPeriodRepository;
    private readonly IBalancePaymentStrategy   _balancePaymentStrategy;



    public WalletRequestService(
        IMapper                  mapper,
        IWalletRequestRepository walletRequestRepository,
        IAccountServiceAdapter   accountServiceAdapter,
        IInvoiceRepository       invoiceRepository,
        IInvoiceDetailRepository invoicesDetails,
        IWalletRepository        walletRepository,
        IWalletPeriodRepository  walletPeriodRepository, IBalancePaymentStrategy balancePaymentStrategy
    ) : base(mapper)
    {
        _walletRequestRepository = walletRequestRepository;
        _accountServiceAdapter   = accountServiceAdapter;
        _invoiceRepository       = invoiceRepository;
        _invoiceDetailRepository = invoicesDetails;
        _walletRepository        = walletRepository;
        _walletPeriodRepository  = walletPeriodRepository;
        _balancePaymentStrategy  = balancePaymentStrategy;
    }

    public async Task<IEnumerable<WalletRequestDto>> GetAllWalletsRequests()
    {
        var response = await _walletRequestRepository.GetAllWalletsRequests();

        return Mapper.Map<IEnumerable<WalletRequestDto>>(response);
    }

    public async Task<IEnumerable<WalletRequestDto>?> GetAllWalletRequestRevertTransaction()
    {
        var response = await _walletRequestRepository.GetAllWalletRequestRevertTransaction();

        return Mapper.Map<IEnumerable<WalletRequestDto>>(response);
    }

    public async Task<IEnumerable<WalletRequestDto>?> GetWalletRequestById(int id)
    {
        var response = await _walletRequestRepository.GetAllWalletRequestByAffiliateId(id);

        return Mapper.Map<IEnumerable<WalletRequestDto>>(response);
    }

    public async Task<WalletRequestDto?> CreateWalletRequestAsync(WalletRequestRequest request)
    {
        var isDateValid  = await IsWithdrawalDateAllowed();
        var hasWalletBtc = await HasWalletAddress(request.AffiliateId);

        if (!isDateValid)
            return null;

        if (!hasWalletBtc)
            return null;

        var response =
            await _accountServiceAdapter.VerificationCode(request.VerificationCode, request.UserPassword, request.AffiliateId);
        var userAvailableBalance = await _walletRepository.GetAvailableBalanceByAffiliateId(request.AffiliateId);
        var userReverseBalance   = await _walletRepository.GetReverseBalanceByAffiliateId(request.AffiliateId);
        var isActivePool         = await _walletRepository.IsActivePoolGreaterThanOrEqualTo25(request.AffiliateId);

        if (!isActivePool)
            return null;

        if (!response.IsSuccessful)
            return null;

        if (string.IsNullOrEmpty(response.Content))
            return null;

        var result = response.Content.ToJsonObject<ServicesValidCodeAccountResponse>();
        if (result is { Data: false })
            return null;

        userAvailableBalance -= userReverseBalance ?? 0;

        if (request.Amount > userAvailableBalance)
            return null;

        var walletRequest = new WalletsRequests
        {
            Amount        = request.Amount,
            Concept       = request.Concept!,
            Status        = WalletRequestStatus.pending.ToByte(),
            AffiliateId   = request.AffiliateId,
            AdminUserName = request.AffiliateName,
            CreationDate  = DateTime.Now,
            AttentionDate = null,
            OrderNumber   = CommonExtensions.GenerateOrderNumber(request.AffiliateId),
            Type          = WalletRequestType.withdrawal_request.ToString(),
            InvoiceNumber = Constants.EmptyValue
        };

        walletRequest = await _walletRequestRepository.CreateWalletRequestAsync(walletRequest);

        return Mapper.Map<WalletRequestDto>(walletRequest);
    }

    public async Task<ServicesResponse?> ProcessOption(int pOption, List<int> ids)
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

    public async Task CancelWalletRequestsAsync(List<int> ids)
    {
        var idsList = await _walletRequestRepository.GetWalletRequestsByIds(ids);

        if (idsList is { Count: 0 })
            return;

        var today = DateTime.Now;

        Parallel.ForEach(idsList, item =>
        {
            item.AttentionDate = today;
            item.UpdatedAt     = today;
            item.Status        = WalletRequestStatus.cancel.ToByte();
        });

        await _walletRequestRepository.UpdateBulkWalletRequestsAsync(idsList.ToList());
    }

    public async Task<WalletRequestDto?> CreateWalletRequestRevert(WalletRequestRevertTransaction request)
    {
        var response         = await _invoiceRepository.GetInvoiceById(request.InvoiceId);
        var userInfoResponse = await _accountServiceAdapter.GetUserInfo(response!.AffiliateId);

        var amountReverted  = response.TotalInvoice * (decimal?)0.90;
        var leftOverBalance = response.TotalInvoice - amountReverted;

        var creditRequest = new CreditTransactionRequest
        {
            AffiliateId       = request.AffiliateId,
            UserId            = Constants.AdminUserId,
            Concept           = Constants.RevertEcoPoolConcept + $" Factura# {request.InvoiceId}",
            Credit            = Convert.ToDouble(amountReverted),
            AffiliateUserName = userInfoResponse!.UserName,
            AdminUserName     = Constants.AdminEcosystemUserName,
            ConceptType       = WalletConceptType.revert_pool.ToString()
        };

        var walletRequest = new WalletsRequests
        {
            AffiliateId   = response.AffiliateId,
            AdminUserName = userInfoResponse.UserName,
            OrderNumber   = CommonExtensions.GenerateOrderNumber(request.AffiliateId),
            InvoiceNumber = response.Id,
            Amount        = response.TotalInvoice ?? 0,
            Concept       = request.Concept,
            Type          = WalletRequestType.revert_invoice_request.ToString(),
            Status        = WalletRequestStatus.pending.ToByte(),
            CreationDate  = DateTime.Now,
            AttentionDate = null
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
            AffiliateId       = user.Id,
            AffiliateUserName = user.UserName!,
            PurchaseFor       = 0,
            Bank              = Constants.WalletBalance,
            PaymentMethod     = 0,
            SecretKey         = null,
            ReceiptNumber     = leftOverBalance.ToString(),
            ProductsList = new List<ProductsRequests>
            {
                new ProductsRequests
                {
                    Count     = 1,
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
            var invoiceResponse       = await _invoiceRepository.GetInvoiceById(invoiceNumber);
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
    private async Task UpdateWalletRequestAsync(WalletsRequests walletRequest)
    {
        walletRequest.AttentionDate = DateTime.Now;
        await _walletRequestRepository.UpdateWalletRequestsAsync(walletRequest);
    }
    public async Task<bool> AdministrativePaymentAsync(WalletsRequests[] requests)
    {
        if (requests.Length is 0)
            return false;

        var userIds = requests.Select(x => x.AffiliateId).Distinct().ToList();

        var tasks         = userIds.Select(id => _accountServiceAdapter.GetUserInfo(id)).ToArray();
        var userInfoArray = await Task.WhenAll(tasks);
        var userInfoList  = userInfoArray.Where(u => u != null).ToList();
        var today         = DateTime.Now;

        var walletsList = new List<Wallets>();
        var idsList     = new List<WalletsRequests>();

        foreach (var user in userInfoList)
        {
            var correspondingRequest = requests.FirstOrDefault(r => r.AffiliateId == user!.Id);
            if (correspondingRequest != null)
            {
                var walletEntry = new Wallets
                {
                    AffiliateId       = user!.Id,
                    AffiliateUserName = user.UserName,
                    AdminUserName     = Constants.AdminEcosystemUserName,
                    UserId            = Constants.AdminUserId,
                    Credit            = Constants.EmptyValue,
                    Debit             = Convert.ToDouble(correspondingRequest.Amount),
                    Deferred          = Constants.EmptyValue,
                    Status            = true,
                    Concept           = Constants.WithdrawalBalance,
                    ConceptType       = WalletConceptType.wallet_withdrawal_request.ToString(),
                    Support           = Constants.EmptyValue,
                    Date              = today,
                    Compression       = false,
                    Detail            = null,
                    CreatedAt         = today,
                    UpdatedAt         = today,
                    DeletedAt         = null
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
            item.UpdatedAt     = today;
            item.Status        = WalletRequestStatus.approved.ToByte();
        });

        await _walletRequestRepository.UpdateBulkWalletRequestsAsync(idsList);

        return true;
    }

    private async Task<bool> IsWithdrawalDateAllowed()
    {
        var defaultZone = Constants.DefaultWithdrawalZone;

        var timeZone      = TimeZoneInfo.FindSystemTimeZoneById(defaultZone);
        var localDateTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZone);

        if (localDateTime.TimeOfDay < new TimeSpan(8, 0, 0) || localDateTime.TimeOfDay > new TimeSpan(18, 0, 0))
            return false;

        var allowedDatesObjects = await _walletPeriodRepository.GetAllWalletsPeriods();

        var allowedDates = allowedDatesObjects.Select(wp => wp.Date.Date).ToList();

        return allowedDates.Contains(localDateTime.Date);
    }

    private async Task<bool> HasWalletAddress(int affiliateId)
    {
        var response = await _accountServiceAdapter.GetAffiliateBtcByAffiliateId(affiliateId);

        if (response.Content is null)
            return false;

        var userInfo = response.Content.ToJsonObject<AffiliateBtcResponse>();

        if (userInfo?.Data is null)
            return false;

        if (string.IsNullOrEmpty(userInfo.Data.Address))
            return false;

        return true;
    }
}