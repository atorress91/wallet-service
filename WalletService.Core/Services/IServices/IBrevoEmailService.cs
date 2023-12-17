using WalletService.Data.Database.CustomModels;
using WalletService.Models.Responses;

namespace WalletService.Core.Services.IServices;

public interface IBrevoEmailService
{
    Task<bool> SendEmailWelcome(UserInfoResponse                       user, InvoicesSpResponse spResponse);
    Task<bool> SendBonusConfirmation(UserInfoResponse                  user, string             userName);
    Task<bool> SendEmailPurchaseConfirm(UserInfoResponse user, Dictionary<string, byte[]> pdfDataDict,
        InvoicesSpResponse spResponse);
    Task<bool> SendEmailConfirmationEmailToThirdParty(UserInfoResponse user, string             nameOfPurchaser, List<string>       productNames);
    Task<bool> SendEmailMembershipConfirm(UserInfoResponse user, byte[] pdfData, InvoicesSpResponse spResponse);

    Task<bool> SendEmailPurchaseConfirmForAcademy(UserInfoResponse user, Dictionary<string, byte[]> pdfDataDict,
        InvoicesSpResponse                                         spResponse);

    Task<bool> SendInvitationsForTradingAcademy(UserAffiliateResponse user, string link, string code);
}