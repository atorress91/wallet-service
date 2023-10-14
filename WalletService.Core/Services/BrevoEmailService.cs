using Microsoft.Extensions.Options;
using WalletService.Core.Services.IServices;
using WalletService.Models.Configuration;
using sib_api_v3_sdk.Api;
using sib_api_v3_sdk.Client;
using sib_api_v3_sdk.Model;
using System.Reflection;
using System.Text;
using WalletService.Data.Database.CustomModels;
using WalletService.Models.Constants;
using WalletService.Models.Responses;
using WalletService.Utility.Extensions;
using Task = System.Threading.Tasks.Task;


namespace WalletService.Core.Services;

public class BrevoEmailService : IBrevoEmailService
{
    private readonly ApplicationConfiguration _appSettings;

    public BrevoEmailService(IOptions<ApplicationConfiguration> appSettings)
    {
        _appSettings = appSettings.Value;
        Configuration.Default.AddApiKey("api-key", _appSettings.SendingBlue.ApiKey);
    }

    private async Task<bool> SendEmail(string toEmail, string subject, string body)
    {
        var apiInstance = new TransactionalEmailsApi();
        var sendSmtpEmail = new SendSmtpEmail
        {
            To          = new List<SendSmtpEmailTo> { new SendSmtpEmailTo(toEmail) },
            Subject     = subject,
            HtmlContent = body,
            Sender      = new SendSmtpEmailSender("Ecosystem Sharing Evolution", _appSettings.EmailCredentials!.From)
        };

        try
        {
            var result = await apiInstance.SendTransacEmailAsync(sendSmtpEmail);
            Console.WriteLine($"Email sent successfully! Message ID: {result.MessageId}");
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error sending email: {ex.Message}");
            return false;
        }
    }
    private async Task<bool> SendEmailWithInvoice(string toEmail, string subject, string body, Dictionary<string, byte[]> pdfDataDict)
    {
        var apiInstance = new TransactionalEmailsApi();

        var attachments = pdfDataDict.Select(kvp => new SendSmtpEmailAttachment
        {
            Content = kvp.Value,
            Name = kvp.Key
        }).ToList();

        var sendSmtpEmail = new SendSmtpEmail
        {
            To = new List<SendSmtpEmailTo> { new SendSmtpEmailTo(toEmail) },
            Subject = subject,
            HtmlContent = body,
            Sender = new SendSmtpEmailSender("Ecosystem Sharing Evolution", _appSettings.EmailCredentials!.From),
            Attachment = attachments
        };

        try
        {
            var result = await apiInstance.SendTransacEmailAsync(sendSmtpEmail);
            Console.WriteLine($"Email with attachments sent successfully! Message ID: {result.MessageId}");
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error sending email with attachments: {ex.Message}");
            return false;
        }
    }


    public async Task<bool> SendEmailWelcome(UserInfoResponse user, InvoicesSpResponse spResponse)
    {
        var dictionary = new Dictionary<string, string>();

        var workingDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        var separator        = Path.DirectorySeparatorChar;
        var pathFile         = $"{workingDirectory}{separator}EmailTemplates{separator}welcome.html";
        var bodyString       = await File.ReadAllTextAsync(pathFile, Encoding.UTF8);

        if (string.IsNullOrEmpty(bodyString))
            return false;

        var fullName = $"{user.Name} {user.LastName}";
        var date     = DateTime.Now.ToString("MM/dd/yyyy");
        dictionary.Add("{0}", fullName);
        dictionary.Add("{1}", user.UserName ?? "");
        dictionary.Add("{2}", date);
        dictionary.Add("{4}", spResponse.PaymentMethod ?? "");

        var body = bodyString.ReplaceHtml(dictionary);

        return await SendEmail(user.Email!, Constants.SubjectConfirmAffiliation, body);
    }

    public async Task<bool> SendBonusConfirmation(UserInfoResponse user, string userName)
    {
        var dictionary = new Dictionary<string, string>();

        var workingDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        var separator        = Path.DirectorySeparatorChar;
        var pathFile         = $"{workingDirectory}{separator}EmailTemplates{separator}confirm-bonus.html";
        var bodyString       = await File.ReadAllTextAsync(pathFile, Encoding.UTF8);

        if (string.IsNullOrEmpty(bodyString))
            return false;

        var fullName = $"{user.Name} {user.LastName}";
        dictionary.Add("{0}", fullName);
        dictionary.Add("{1}", userName);

        var body = bodyString.ReplaceHtml(dictionary);

        return await SendEmail(user.Email!, Constants.SubjectConfirmBonus, body);
    }

    public async Task<bool> SendEmailPurchaseConfirm(UserInfoResponse user, Dictionary<string, byte[]> pdfDataDict, InvoicesSpResponse spResponse)
    {
        var dictionary = new Dictionary<string, string>();

        var workingDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        var separator = Path.DirectorySeparatorChar;
        var pathFile = $"{workingDirectory}{separator}EmailTemplates{separator}confirm-purchase.html";
        var bodyString = await File.ReadAllTextAsync(pathFile, Encoding.UTF8);

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

        return await SendEmailWithInvoice(user.Email!, Constants.SubjectConfirmPurchase, body, pdfDataDict);
    }



    public async Task<bool> SendEmailConfirmationEmailToThirdParty(UserInfoResponse user, string nameOfPurchaser, List<string> productNames)
    {
        var           dictionary          = new Dictionary<string, string>();
        StringBuilder productNamesBuilder = new StringBuilder();

        var workingDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        var separator        = Path.DirectorySeparatorChar;
        var pathFile         = $"{workingDirectory}{separator}EmailTemplates{separator}PaymentsToThirdParties.html";
        var bodyString       = await File.ReadAllTextAsync(pathFile, Encoding.UTF8);

        if (string.IsNullOrEmpty(bodyString))
            return false;

        var fullName = $"{user.Name} {user.LastName}";
        var date     = DateTime.Now.ToString("MM/dd/yyyy");
        dictionary.Add("{0}", fullName);
        dictionary.Add("{1}", nameOfPurchaser);
        dictionary.Add("{2}", date);

        foreach (var productName in productNames)
        {
            productNamesBuilder.AppendLine(productName + "<br>");
        }

        dictionary.Add("{3}", productNamesBuilder.ToString());

        var body = bodyString.ReplaceHtml(dictionary);

        return await SendEmail(user.Email!, Constants.SubjectConfirmPurchase, body);
    }

    private async Task<bool> SendEmailForMembership(string toEmail, string subject, string body, byte[] pdfData)
    {
        var apiInstance = new TransactionalEmailsApi();
        var sendSmtpEmail = new SendSmtpEmail
        {
            To          = new List<SendSmtpEmailTo> { new SendSmtpEmailTo(toEmail) },
            Subject     = subject,
            HtmlContent = body,
            Sender      = new SendSmtpEmailSender("Ecosystem Sharing Evolution", _appSettings.EmailCredentials!.From),
            Attachment = new List<SendSmtpEmailAttachment>
            {
                new SendSmtpEmailAttachment
                {
                    Content = pdfData,
                    Name    = "Invoice.pdf"
                }
            }
        };

        try
        {
            var result = await apiInstance.SendTransacEmailAsync(sendSmtpEmail);
            Console.WriteLine($"Email with attachment sent successfully! Message ID: {result.MessageId}");
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error sending email with attachment: {ex.Message}");
            return false;
        }
    }
    
    public async Task<bool> SendEmailMembershipConfirm(UserInfoResponse user, byte[] pdfData, InvoicesSpResponse spResponse)
    {
        var dictionary = new Dictionary<string, string>();

        var workingDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        var separator        = Path.DirectorySeparatorChar;
        var pathFile         = $"{workingDirectory}{separator}EmailTemplates{separator}confirm-purchase.html";
        var bodyString       = await File.ReadAllTextAsync(pathFile, Encoding.UTF8);

        if (string.IsNullOrEmpty(bodyString))
            return false;

        var fullName = $"{user.Name} {user.LastName}";
        var date     = DateTime.Now.ToString("MM/dd/yyyy");
        dictionary.Add("{0}", fullName);
        dictionary.Add("{1}", date);
        dictionary.Add("{2}", spResponse.PaymentMethod ?? "");
        dictionary.Add("{3}", spResponse.InvoiceNumber.ToString());
        dictionary.Add("{4}", spResponse.TotalInvoice.ToString() ?? "");

        var body = bodyString.ReplaceHtml(dictionary);

        return await SendEmailForMembership(user.Email!, Constants.SubjectConfirmPurchase, body, pdfData);
    }

}