using System.Reflection;
using System.Text;
using Microsoft.Extensions.Options;
using sib_api_v3_sdk.Api;
using sib_api_v3_sdk.Client;
using sib_api_v3_sdk.Model;
using WalletService.Models.Configuration;
using WalletService.Models.Constants;

namespace WalletService.Core.Services;

public abstract class BaseEmailService
{
    private readonly ApplicationConfiguration _appSettings;

    protected BaseEmailService(IOptions<ApplicationConfiguration> appSettings)
    {
        _appSettings = appSettings.Value;
        Configuration.Default.AddApiKey("api-key", _appSettings.SendingBlue!.ApiKey);
    }

    private string GetSenderName(int brandId)
    {
        return brandId switch
        {
            1 => Constants.EcosystemSenderName,
            2 => Constants.RecyCoinSenderName,
            _ => throw new ArgumentException("Invalid brandId", nameof(brandId))
        };
    }

    private string GetBrandFolderName(int brandId)
    {
        return brandId switch
        {
            1 => "Ecosystem",
            2 => "RecyCoin",
            _ => throw new ArgumentException("Invalid brandId", nameof(brandId))
        };
    }

    protected async Task<string> GetEmailTemplate(string templateName, int brandId)
    {
        var brandFolder = GetBrandFolderName(brandId);
        var workingDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        var separator = Path.DirectorySeparatorChar;
        var pathFile = $"{workingDirectory}{separator}EmailTemplates{separator}{brandFolder}{separator}{templateName}";

        return await File.ReadAllTextAsync(pathFile, Encoding.UTF8);
    }

    protected async Task<bool> SendEmail(string toEmail, string subject, string body, int brandId)
    {
        var apiInstance = new TransactionalEmailsApi();
        var nameSender = GetSenderName(brandId);

        var sendSmtpEmail = new SendSmtpEmail
        {
            To = new List<SendSmtpEmailTo> { new SendSmtpEmailTo(toEmail) },
            Subject = subject,
            HtmlContent = body,
            Sender = new SendSmtpEmailSender(nameSender, _appSettings.EmailCredentials!.From)
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

    protected async Task<bool> SendEmailWithInvoice(string toEmail, string subject, string body,
        Dictionary<string, byte[]> pdfDataDict, int brandId)
    {
        var apiInstance = new TransactionalEmailsApi();
        var nameSender = GetSenderName(brandId);

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
            Sender = new SendSmtpEmailSender(nameSender, _appSettings.EmailCredentials!.From),
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

    protected async Task<bool> SendEmailForMembership(string toEmail, string subject, string body, byte[] pdfData,
        int brandId)
    {
        var apiInstance = new TransactionalEmailsApi();
        var senderName = GetSenderName(brandId);
        var sendSmtpEmail = new SendSmtpEmail
        {
            To = new List<SendSmtpEmailTo> { new SendSmtpEmailTo(toEmail) },
            Subject = subject,
            HtmlContent = body,
            Sender = new SendSmtpEmailSender(senderName, _appSettings.EmailCredentials!.From),
            Attachment = new List<SendSmtpEmailAttachment>
            {
                new SendSmtpEmailAttachment
                {
                    Content = pdfData,
                    Name = "Invoice.pdf"
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
}