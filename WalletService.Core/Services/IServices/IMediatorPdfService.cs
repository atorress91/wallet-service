using WalletService.Data.Database.CustomModels;
using WalletService.Models.Requests.WalletRequest;
using WalletService.Models.Responses;

namespace WalletService.Core.Services.IServices;

public interface IMediatorPdfService
{
    Task<byte[]> GenerateInvoice(UserInfoResponse userInfo, DebitTransactionRequest invoice, InvoicesSpResponse spResponse);
}