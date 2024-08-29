using WalletService.Data.Database.CustomModels;
using WalletService.Data.Database.Models;
using WalletService.Models.Requests.WalletRequest;
using WalletService.Models.Responses;

namespace WalletService.Core.Services.IServices;

public interface IRecyCoinPdfService
{
    Task<byte[]> GenerateInvoice(UserInfoResponse userInfo, DebitTransactionRequest invoice, InvoicesSpResponse spResponse);
    Task<byte[]> RegenerateInvoice(UserInfoResponse userInfo, Invoices invoice);
}