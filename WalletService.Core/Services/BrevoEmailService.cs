using Microsoft.Extensions.Options;
using WalletService.Core.Services.IServices;
using WalletService.Models.Configuration;
using System.Text;
using WalletService.Data.Database.CustomModels;
using WalletService.Models.Constants;
using WalletService.Models.Responses;
using WalletService.Utility.Extensions;

namespace WalletService.Core.Services;

public class BrevoEmailService : BaseEmailService, IBrevoEmailService
{
    public BrevoEmailService(IOptions<ApplicationConfiguration> appSettings) : base(appSettings) { }

    public async Task<bool> SendEmailWelcome(UserInfoResponse user, InvoicesSpResponse spResponse, long brandId)
    {
        var dictionary = new Dictionary<string, string>();

        var bodyString = await GetEmailTemplate("welcome.html", brandId);

        if (string.IsNullOrEmpty(bodyString))
            return false;

        var fullName = $"{user.Name} {user.LastName}";
        var date = DateTime.Now.ToString("MM/dd/yyyy");
        dictionary.Add("{0}", fullName);
        dictionary.Add("{1}", user.UserName ?? "");
        dictionary.Add("{2}", date);
        dictionary.Add("{4}", spResponse.PaymentMethod ?? "");

        var body = bodyString.ReplaceHtml(dictionary);

        return await SendEmail(user.Email!, Constants.SubjectConfirmAffiliation, body, brandId);
    }

    public async Task<bool> SendBonusConfirmation(UserInfoResponse user, string userName, long brandId)
    {
        var dictionary = new Dictionary<string, string>();

        var bodyString = await GetEmailTemplate("confirm-bonus.html", brandId);

        if (string.IsNullOrEmpty(bodyString))
            return false;

        var fullName = $"{user.Name} {user.LastName}";
        dictionary.Add("{0}", fullName);
        dictionary.Add("{1}", userName);

        var body = bodyString.ReplaceHtml(dictionary);

        return await SendEmail(user.Email!, Constants.SubjectConfirmBonus, body, brandId);
    }

    public async Task<bool> SendEmailPurchaseConfirm(UserInfoResponse user, Dictionary<string, byte[]> pdfDataDict,
        InvoicesSpResponse spResponse, long brandId)
    {
        var dictionary = new Dictionary<string, string>();

        var bodyString = await GetEmailTemplate("confirm-purchase.html", brandId);

        if (string.IsNullOrEmpty(bodyString))
            return false;

        var fullName = $"{user.Name} {user.LastName}";
        var date = DateTime.Now.ToString("MM/dd/yyyy");
        dictionary.Add("{0}", fullName);
        dictionary.Add("{1}", date);
        dictionary.Add("{2}", spResponse.PaymentMethod ?? "");
        dictionary.Add("{3}", spResponse.Id.ToString());
        dictionary.Add("{4}", spResponse.TotalInvoice.ToString() ?? "");

        var body = bodyString.ReplaceHtml(dictionary);

        return await SendEmailWithInvoice(user.Email!, Constants.SubjectConfirmPurchase, body, pdfDataDict, brandId);
    }

    public async Task<bool> SendEmailConfirmationEmailToThirdParty(UserInfoResponse user, string nameOfPurchaser,
        List<string> productNames, long brandId)
    {
        var dictionary = new Dictionary<string, string>();
        StringBuilder productNamesBuilder = new StringBuilder();

        var bodyString = await GetEmailTemplate("PaymentsToThirdParties.html", brandId);

        if (string.IsNullOrEmpty(bodyString))
            return false;

        var fullName = $"{user.Name} {user.LastName}";
        var date = DateTime.Now.ToString("MM/dd/yyyy");
        dictionary.Add("{0}", fullName);
        dictionary.Add("{1}", nameOfPurchaser);
        dictionary.Add("{2}", date);

        foreach (var productName in productNames)
        {
            productNamesBuilder.AppendLine(productName + "<br>");
        }

        dictionary.Add("{3}", productNamesBuilder.ToString());

        var body = bodyString.ReplaceHtml(dictionary);

        return await SendEmail(user.Email!, Constants.SubjectConfirmPurchase, body, brandId);
    }

    public async Task<bool> SendEmailMembershipConfirm(UserInfoResponse user, byte[] pdfData,
        InvoicesSpResponse spResponse, long brandId)
    {
        var dictionary = new Dictionary<string, string>();

        var bodyString = await GetEmailTemplate("confirm-purchase.html", brandId);

        if (string.IsNullOrEmpty(bodyString))
            return false;

        var fullName = $"{user.Name} {user.LastName}";
        var date = DateTime.Now.ToString("MM/dd/yyyy");
        dictionary.Add("{0}", fullName);
        dictionary.Add("{1}", date);
        dictionary.Add("{2}", spResponse.PaymentMethod ?? "");
        dictionary.Add("{3}", spResponse.InvoiceNumber.ToString());
        dictionary.Add("{4}", spResponse.TotalInvoice.ToString() ?? "");

        var body = bodyString.ReplaceHtml(dictionary);

        return await SendEmailForMembership(user.Email!, Constants.SubjectConfirmPurchase, body, pdfData, brandId);
    }

    public async Task<bool> SendEmailPurchaseConfirmForAcademy(UserInfoResponse user,
        Dictionary<string, byte[]> pdfDataDict,
        InvoicesSpResponse spResponse, long brandId)
    {
        var dictionary = new Dictionary<string, string>();

        var bodyString = await GetEmailTemplate("confirm-purchase-academy.html", brandId);

        if (string.IsNullOrEmpty(bodyString))
            return false;

        var fullName = $"{user.Name} {user.LastName}";
        var date = DateTime.Now.ToString("MM/dd/yyyy");
        dictionary.Add("{0}", fullName);
        dictionary.Add("{1}", date);

        var body = bodyString.ReplaceHtml(dictionary);

        return await SendEmailWithInvoice(user.Email!, Constants.SubjectConfirmPurchase, body, pdfDataDict, brandId);
    }

    public async Task<bool> SendInvitationsForTradingAcademy(UserAffiliateResponse user, string link, string code,
        long brandId)
    {
        var dictionary = new Dictionary<string, string>();

        var bodyString = await GetEmailTemplate("invitation-trading-academy.html", brandId);

        if (string.IsNullOrEmpty(bodyString))
            return false;

        var fullName = $"{user.Data!.Name} {user.Data.LastName}";

        dictionary.Add("{0}", fullName);
        dictionary.Add("{1}", link);
        dictionary.Add("{2}", code);

        var body = bodyString.ReplaceHtml(dictionary);

        return await SendEmail(user.Data.Email!, Constants.SubjectInvitationForAcademy, body, brandId);
    }
}