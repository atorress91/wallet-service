using WalletService.Data.Database.CustomModels;
using WalletService.Models.Responses;

namespace WalletService.Core.Services.IServices;

public interface IBrevoEmailService
{
    Task<bool> SendEmailWelcome(UserInfoResponse user, InvoicesSpResponse spResponse, int brandId);
    Task<bool> SendBonusConfirmation(UserInfoResponse user, string userName, int brandId);

    Task<bool> SendEmailPurchaseConfirm(UserInfoResponse user, Dictionary<string, byte[]> pdfDataDict,
        InvoicesSpResponse spResponse, int brandId);

    Task<bool> SendEmailConfirmationEmailToThirdParty(UserInfoResponse user, string nameOfPurchaser,
        List<string> productNames, int brandId);

    Task<bool> SendEmailMembershipConfirm(UserInfoResponse user, byte[] pdfData, InvoicesSpResponse spResponse, int brandId);

    Task<bool> SendEmailPurchaseConfirmForAcademy(UserInfoResponse user, Dictionary<string, byte[]> pdfDataDict,
        InvoicesSpResponse spResponse, int brandId);

    Task<bool> SendInvitationsForTradingAcademy(UserAffiliateResponse user, string link, string code, int brandId);
}