using System.Text.Json;
using AutoMapper;
using WalletService.Core.Factory;
using WalletService.Core.PaymentStrategies.IPaymentStrategies;
using WalletService.Core.Services.IServices;
using WalletService.Data.Adapters.IAdapters;
using WalletService.Data.Database.CustomModels;
using WalletService.Data.Database.Models;
using WalletService.Data.Repositories.IRepositories;
using WalletService.Models.Constants;
using WalletService.Models.DTO.BalanceInformationDto;
using WalletService.Models.DTO.WalletDto;
using WalletService.Models.Enums;
using WalletService.Models.Requests.TransferBalanceRequest;
using WalletService.Models.Requests.WalletRequest;
using WalletService.Models.Requests.WalletTransactionRequest;
using WalletService.Models.Responses;
using WalletService.Utility.Extensions;

namespace WalletService.Core.Services;

public class WalletService : BaseService, IWalletService
{
    private readonly IAccountServiceAdapter _accountServiceAdapter;
    private readonly IInvoiceDetailRepository _invoiceDetailRepository;
    private readonly IInvoiceRepository _invoiceRepository;
    private readonly INetworkPurchaseRepository _networkPurchaseRepository;
    private readonly IPaymentStrategyFactory _paymentStrategyFactory;
    private readonly IWalletRepository _walletRepository;
    private readonly IWalletRequestRepository _walletRequestRepository;

    public WalletService(
        IMapper                    mapper,
        IWalletRepository          walletRepository,
        IWalletRequestRepository   walletRequestRepository,
        IAccountServiceAdapter     accountServiceAdapter,
        IInvoiceRepository         invoiceRepository,
        IInvoiceDetailRepository   invoiceDetailRepository,
        IPaymentStrategyFactory    paymentStrategyFactory,
        INetworkPurchaseRepository networkPurchaseRepository) :
        base(mapper)
    {
        _walletRepository          = walletRepository;
        _walletRequestRepository   = walletRequestRepository;
        _invoiceRepository         = invoiceRepository;
        _invoiceDetailRepository   = invoiceDetailRepository;
        _accountServiceAdapter     = accountServiceAdapter;
        _paymentStrategyFactory    = paymentStrategyFactory;
        _networkPurchaseRepository = networkPurchaseRepository;
    }


    #region wallets

    public async Task<IEnumerable<WalletDto>> GetAllWallets()
    {
        var response = await _walletRepository.GetAllWallets();

        return Mapper.Map<IEnumerable<WalletDto>>(response);
    }

    public async Task<IEnumerable<WalletDto>> GetWalletsRequest(int userId)
    {
        var response = await _walletRepository.GetWalletsRequest(userId);

        return Mapper.Map<IEnumerable<WalletDto>>(response);
    }

    public async Task<WalletDto?> GetWalletById(int id)
    {
        var response = await _walletRepository.GetWalletById(id);

        return Mapper.Map<WalletDto>(response);
    }

    public async Task<IEnumerable<WalletDto?>> GetWalletByAffiliateId(int affiliateId)
    {
        var response   = await _walletRepository.GetWalletByAffiliateId(affiliateId);
        var mappedList = Mapper.Map<IEnumerable<WalletDto?>>(response).ToList();
        mappedList.Reverse();

        return mappedList;
    }

    public async Task<IEnumerable<WalletDto?>> GetWalletByUserId(int userId)
    {
        var response = await _walletRepository.GetWalletByUserId(userId);

        return Mapper.Map<IEnumerable<WalletDto?>>(response);
    }

    public async Task<WalletDto?> DeleteWalletAsync(int id)
    {
        var wallet = await _walletRepository.GetWalletById(id);

        if (wallet is null)
            return null;

        await _walletRepository.DeleteWalletAsync(wallet);

        return Mapper.Map<WalletDto>(wallet);
    }

    public async Task<BalanceInformationDto> GetBalanceInformationByAffiliateId(int affiliateId)
    {
        var amountRequests       = await _walletRequestRepository.GetTotalWalletRequestAmountByAffiliateId(affiliateId);
        var availableBalance     = await _walletRepository.GetAvailableBalanceByAffiliateId(affiliateId);
        var reverseBalance       = await _walletRepository.GetReverseBalanceByAffiliateId(affiliateId);
        var totalAcquisitions    = await _walletRepository.GetTotalAcquisitionsByAffiliateId(affiliateId);
        var totalCommissionsPaid = await _walletRepository.GetTotalCommissionsPaid(affiliateId);

        var response = new BalanceInformationDto
        {
            AvailableBalance     = availableBalance,
            ReverseBalance       = reverseBalance ?? 0,
            TotalAcquisitions    = Math.Round(totalAcquisitions ?? 0, 2),
            TotalCommissionsPaid = totalCommissionsPaid ?? 0
        };

        if (amountRequests == 0m && response.ReverseBalance == 0m) return response;

        response.AvailableBalance -= amountRequests;
        response.AvailableBalance -= response.ReverseBalance;

        return response;
    }

    public async Task<BalanceInformationAdminDto> GetBalanceInformationAdmin()
    {
        var responseAffiliates = await _accountServiceAdapter.GetTotalActiveMembers();
        var response           = JsonSerializer.Deserialize<GetTotalActiveMembersResponse>(responseAffiliates.Content);

        var enabledAffiliates     = response.Data;
        var walletProfit          = await _walletRepository.GetAvailableBalanceAdmin();
        var amountRequests        = await _walletRequestRepository.GetTotalWalletRequestAmount();
        var paidCommissions       = 0m;
        var calculatedCommissions = 0m;

        var information = new BalanceInformationAdminDto
        {
            EnabledAffiliates     = enabledAffiliates,
            WalletProfit          = walletProfit,
            CommissionsPaid       = paidCommissions,
            CalculatedCommissions = calculatedCommissions
        };

        if (amountRequests == 0m) return information;

        information.WalletProfit -= amountRequests;

        return information;
    }

    public async Task<bool> PaymentHandler(WalletRequest request)
    {
        IPaymentStrategy?           paymentStrategy           = null;
        IBalancePaymentStrategy?    balancePaymentStrategy    = null;
        IMembershipPaymentStrategy? membershipPaymentStrategy = null;

        switch (request.PaymentMethod)
        {
            case 0:
                balancePaymentStrategy = _paymentStrategyFactory.GetBalancePaymentStrategy();
                break;
            case 1:
                paymentStrategy = _paymentStrategyFactory.GetStrategy(PaymentType.ReversedBalance);
                break;
            case 2:
                paymentStrategy = _paymentStrategyFactory.GetStrategy(PaymentType.PaymentToThirdParties);
                break;
            case 3:
                membershipPaymentStrategy = _paymentStrategyFactory.GetMembershipStrategy();
                break;
            default:
                throw new ArgumentException("No strategy available.");
        }

        if (paymentStrategy != null)
        {
            return await paymentStrategy.ExecutePayment(request);
        }
        else if (balancePaymentStrategy != null)
        {
            return await balancePaymentStrategy.ExecuteEcoPoolPayment(request);
        }
        else if (membershipPaymentStrategy != null)
        {
            return await membershipPaymentStrategy.ExecutePayment(request);
        }
        else
        {
            throw new InvalidOperationException("No valid payment strategy was determined.");
        }
    }
    
    public async Task<bool> CoursePaymentHandler(WalletRequest request)
    {
        IBalancePaymentStrategy balancePaymentStrategy = _paymentStrategyFactory.GetBalancePaymentStrategy();

        return await balancePaymentStrategy.ExecutePaymentCourses(request);
    }
    
    public async Task<bool> AdminPaymentHandler(WalletRequest request)
    {
        var balancePaymentStrategy = _paymentStrategyFactory.GetBalancePaymentStrategy();

        return await balancePaymentStrategy.ExecuteAdminPayment(request);
    }

    public async Task<bool> TransferBalanceForNewAffiliate(TransferBalanceRequest request)
    {
        var today       = DateTime.Now;
        var amount      = 10;
        var userInfo    = await _accountServiceAdapter.GetAffiliateByUserName(request.ToUserName);
        var currentUser = await _accountServiceAdapter.GetAffiliateByUserName(request.FromUserName);

        if (!userInfo.IsSuccessful)
            return false;

        if (string.IsNullOrEmpty(userInfo.Content))
            return false;

        var result      = JsonSerializer.Deserialize<UserAffiliateResponse>(userInfo.Content!);
        var userResult  = JsonSerializer.Deserialize<UserAffiliateResponse>(currentUser.Content!);
        var userBalance = await GetBalanceInformationByAffiliateId(request.FromAffiliateId);

        if (userResult?.Data?.VerificationCode != request.SecurityCode)
            return false;

        if (amount > userBalance.AvailableBalance)
            return false;

        if (result?.Data?.Status != 1)
            return false;

        if (result.Data?.ActivationDate != null)
            return false;

        var debitTransaction = new WalletTransactionRequest
        {
            Debit             = amount,
            Deferred          = Constants.None,
            Detail            = null,
            AffiliateId       = request.FromAffiliateId,
            AdminUserName     = Constants.AdminEcosystemUserName,
            Status            = true,
            UserId            = Constants.AdminUserId,
            Credit            = Constants.None,
            Concept           = $"{Constants.TransferForMembership} {request.ToUserName}",
            Support           = null!,
            Date              = today,
            Compression       = false,
            AffiliateUserName = request.FromUserName,
            ConceptType       = WalletConceptType.balance_transfer
        };

        var creditTransaction = new WalletTransactionRequest
        {
            Debit             = Constants.None,
            Deferred          = Constants.None,
            Detail            = null,
            AffiliateId       = result.Data!.Id,
            AdminUserName     = Constants.AdminEcosystemUserName,
            Status            = true,
            UserId            = Constants.AdminUserId,
            Credit            = amount,
            Concept           = $"{Constants.TransferToMembership} {request.FromUserName}",
            Support           = null!,
            Date              = today,
            Compression       = false,
            AffiliateUserName = result.Data.UserName,
            ConceptType       = WalletConceptType.balance_transfer
        };

        var debitWallet  = Mapper.Map<Wallets>(debitTransaction);
        var creditWallet = Mapper.Map<Wallets>(creditTransaction);

        var success = await _walletRepository.CreateTransferBalance(debitWallet, creditWallet);

        if (!success)
            return false;

        var confirmPurchase = await PurchaseMembershipForNewAffiliates(creditTransaction);

        if (!confirmPurchase)
            return false;

        return confirmPurchase;
    }

    public async Task<ServicesResponse> TransferBalance(string encrypted)
    {
        var data = CommonExtensions.DecryptObject<TransferBalanceRequest>(encrypted);
        
        var today               = DateTime.Now;
        var amount               = data.Amount;
        var currentUser      = await _accountServiceAdapter.GetAffiliateByUserName(data.FromUserName);
        var userInfo         = await _accountServiceAdapter.GetAffiliateByUserName(data.ToUserName);
        var isActivePool                 = await _walletRepository.IsActivePoolGreaterThanOrEqualTo25(data.FromAffiliateId);
        
        if (!isActivePool)
            return new ServicesResponse { Success = false, Message = "No tiene un Pool activo", Code = 400 };

        if (!userInfo.IsSuccessful)
            return new ServicesResponse { Success = false, Message = "Error", Code = 400 };

        if (string.IsNullOrEmpty(userInfo.Content))
            return new ServicesResponse { Success = false, Message = "Error", Code = 400 };

        var currentUserResult = JsonSerializer.Deserialize<UserAffiliateResponse>(currentUser.Content!);
        var result            = JsonSerializer.Deserialize<UserAffiliateResponse>(userInfo.Content!);
        var userBalance       = await GetBalanceInformationByAffiliateId(data.FromAffiliateId);

        if (currentUserResult?.Data?.VerificationCode != data.SecurityCode)
            return new ServicesResponse { Success = false, Message = "El código de seguridad no coincidec.", Code = 400 };
        
        if (amount > userBalance.AvailableBalance)
            return new ServicesResponse { Success = false, Message = "El monto es mayor al saldo disponible.", Code = 400 };

        if (result?.Data?.Status != 1)
            return new ServicesResponse { Success = false, Message = "El estatus del afiliado a transferir es inactivo.", Code = 400 };
        
        var debitTransaction = new WalletTransactionRequest
        {
            Debit             = amount,
            Deferred          = 0,
            Detail            = null,
            AffiliateId       = data.FromAffiliateId,
            AdminUserName     = Constants.AdminEcosystemUserName,
            Status            = true,
            UserId            = 1,
            Credit            = 0,
            Concept           = "Transferencia de saldo al afiliado " + data.ToUserName,
            Support           = null!,
            Date              = today,
            Compression       = false,
            AffiliateUserName = data.FromUserName,
            ConceptType       = WalletConceptType.balance_transfer
        };

        var creditTransaction = new WalletTransactionRequest
        {
            Debit             = 0,
            Deferred          = 0,
            Detail            = null,
            AffiliateId       = result.Data.Id,
            AdminUserName     = Constants.AdminEcosystemUserName,
            Status            = true,
            UserId            = 1,
            Credit            = amount,
            Concept           = "Transferencia de saldo del afiliado " + data.FromUserName,
            Support           = null!,
            Date              = today,
            Compression       = false,
            AffiliateUserName = result.Data.UserName,
            ConceptType       = WalletConceptType.balance_transfer
        };

        var debitWallet  = Mapper.Map<Wallets>(debitTransaction);
        var creditWallet = Mapper.Map<Wallets>(creditTransaction);

        var success = await _walletRepository.CreateTransferBalance(debitWallet, creditWallet);
        
        if(!success)
            return new ServicesResponse { Success = false, Message = "No se pudo crear la transferencia.", Code = 400 };

        return new ServicesResponse { Success = true, Message = "La transferencia se ha creado correctamente.", Code = 200 };
    }

    public async Task<bool> HandleWalletRequestRevertTransactionAsync(int option, int invoiceId)
    {
        var walletRequest = await _walletRequestRepository.GetWalletRequestsByInvoiceNumber(invoiceId);

        if (walletRequest == null)
            return false;

        var creditRequest = CreateCreditTransactionRequest(walletRequest);

        switch (option)
        {
            case 0:
                walletRequest.Status = WalletRequestStatus.cancel.ToByte();
                await UpdateWalletRequestAsync(walletRequest);

                break;
            case 1:
                if (!await DeleteInvoiceAndDetails(walletRequest.InvoiceNumber ?? 0))
                    return false;

                if (!await _walletRepository.CreditTransaction(creditRequest))
                    return false;

                walletRequest.Status = WalletRequestStatus.approved.ToByte();
                await UpdateWalletRequestAsync(walletRequest);
                break;
            default:
                walletRequest.Status = WalletRequestStatus.cancel.ToByte();
                break;
        }

        return true;
    }

    private CreditTransactionRequest CreateCreditTransactionRequest(WalletsRequests walletRequest)
    {
        return new CreditTransactionRequest
        {
            AffiliateId       = walletRequest.AffiliateId,
            UserId            = Constants.AdminUserId,
            Concept           = Constants.RevertEcoPoolConcept + $" Factura# {walletRequest.InvoiceNumber}",
            Credit            = Convert.ToDouble(walletRequest.Amount),
            AffiliateUserName = walletRequest.AdminUserName!,
            AdminUserName     = Constants.AdminEcosystemUserName,
            ConceptType       = WalletConceptType.revert_pool.ToString()
        };
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

    public async Task<(List<PurchasesPerMonthDto> CurrentYearPurchases, List<PurchasesPerMonthDto> PreviousYearPurchases)?>
        GetPurchasesMadeInMyNetwork(int affiliateId)
    {
        var networkResult = await _accountServiceAdapter.GetPersonalNetwork(affiliateId);

        if (networkResult.Content == null || !networkResult.Content.Any())
            return null;

        var result = JsonSerializer.Deserialize<UserPersonalNetworkResponse>(networkResult.Content!);

        if (result?.Data == null || !result.Data.Any())
            return null;

        var idsInMyNetwork = new HashSet<int>(result.Data.Select(affiliate => affiliate.id));

        idsInMyNetwork.Add(affiliateId);

        var purchasesSummary = await _networkPurchaseRepository.GetPurchasesMadeInMyNetwork(idsInMyNetwork);

        var groupedPurchases = purchasesSummary
            .GroupBy(p => p.Year)
            .ToDictionary(g => g.Key, g => g.Select(p => new PurchasesPerMonthDto
            {
                Year           = p.Year,
                Month          = p.Month,
                TotalPurchases = p.TotalPurchases
            }).ToList());

        var currentYear  = DateTime.Now.Year;
        var previousYear = currentYear - 1;

        groupedPurchases.TryGetValue(currentYear, out var currentYearPurchases);
        groupedPurchases.TryGetValue(previousYear, out var previousYearPurchases);

        return (currentYearPurchases ?? new List<PurchasesPerMonthDto>(), previousYearPurchases ?? new List<PurchasesPerMonthDto>());
    }

    private async Task<bool> PurchaseMembershipForNewAffiliates(WalletTransactionRequest request)
    {
        var membershipPaymentStrategy = _paymentStrategyFactory.GetMembershipStrategy();

        var membership = new ProductsRequests
        {
            IdProduct = 1,
            Count     = 1
        };

        var walletRequest = new WalletRequest
        {
            AffiliateId       = request.AffiliateId,
            AffiliateUserName = request.AffiliateUserName!,
            PurchaseFor       = Constants.None,
            Bank              = Constants.WalletBalance,
            PaymentMethod     = 0,
            SecretKey         = null,
            ReceiptNumber     = null,
            ProductsList      = new List<ProductsRequests> { membership }
        };

        await membershipPaymentStrategy.ExecutePayment(walletRequest);

        return true;
    }

    public async Task<IEnumerable<AffiliateBalance>> GetAllAffiliatesWithPositiveBalance()
    {
        var result = await _walletRepository.GetAllAffiliatesWithPositiveBalance();

        return result;
    }
    public async Task<bool> CreateBalanceAdmin(CreditTransactionAdminRequest request)
    {
        if (request.Amount == 0)
            return false;

        var user = await _accountServiceAdapter.GetUserInfo(request.AffiliateId);

        if (user is null)
            return false;

        var credit = new CreditTransactionRequest
        {
            AdminUserName     = Constants.AdminEcosystemUserName,
            AffiliateId       = user.Id,
            Concept           = Constants.AdminCredit,
            Credit            = request.Amount,
            AffiliateUserName = user.UserName,
            ConceptType       = WalletConceptType.balance_transfer.ToString(),
            UserId            = Constants.AdminUserId
        };
        
        var result = await _walletRepository.CreditTransaction(credit);
        if (!result)
            return false;

        return true;
    }
    
    #endregion

}