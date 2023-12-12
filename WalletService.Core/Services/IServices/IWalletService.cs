using WalletService.Data.Database.CustomModels;
using WalletService.Data.Database.Models;
using WalletService.Models.DTO.BalanceInformationDto;
using WalletService.Models.DTO.WalletDto;
using WalletService.Models.Requests;
using WalletService.Models.Requests.CoinPayRequest;
using WalletService.Models.Requests.TransferBalanceRequest;
using WalletService.Models.Requests.WalletPayRequest;
using WalletService.Models.Requests.WalletRequest;
using WalletService.Models.Responses;

namespace WalletService.Core.Services.IServices;

public interface IWalletService
{
    #region Wallet

    Task<IEnumerable<WalletDto?>> GetWalletByUserId(int userId);
    Task<IEnumerable<WalletDto?>> GetWalletByAffiliateId(int affiliateId);
    Task<IEnumerable<WalletDto>> GetAllWallets();
    Task<IEnumerable<WalletDto>> GetWalletsRequest(int userId);

    Task<WalletDto?> GetWalletById(int id);

    // Task<WalletDto?> CreateWalletAsync(WalletRequest                   request);
    // Task<WalletDto?> UpdateWalletAsync(int                             id, WalletRequest request);
    Task<WalletDto?> DeleteWalletAsync(int id);
    Task<BalanceInformationDto> GetBalanceInformationByAffiliateId(int affiliateId);
    Task<BalanceInformationAdminDto> GetBalanceInformationAdmin();
    Task<bool> PaymentHandler(WalletRequest request);
    Task<bool> TransferBalanceForNewAffiliate(TransferBalanceRequest name);
    Task<ServicesResponse> TransferBalance(string encrypted);
    Task<bool> HandleWalletRequestRevertTransactionAsync(int option, int invoiceId);

    Task<(List<PurchasesPerMonthDto> CurrentYearPurchases, List<PurchasesPerMonthDto> PreviousYearPurchases)?>
        GetPurchasesMadeInMyNetwork(int affiliateId);

    Task<bool> AdminPaymentHandler(WalletRequest request);
    Task<IEnumerable<AffiliateBalance>> GetAllAffiliatesWithPositiveBalance();
    Task<bool> CreateBalanceAdmin(CreditTransactionAdminRequest request);
    Task<bool> CoursePaymentHandler(WalletRequest request);

    #endregion
}