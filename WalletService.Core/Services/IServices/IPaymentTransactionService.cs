using WalletService.Models.DTO.PaymentTransactionDto;
using WalletService.Models.Requests.PaymentTransaction;

namespace WalletService.Core.Services.IServices;

public interface IPaymentTransactionService
{
    Task<PaymentTransactionDto?>             CreatePaymentTransactionAsync(PaymentTransactionRequest request);
    Task<IEnumerable<PaymentTransactionDto>> GetAllWireTransfer();
    Task<bool>                               ConfirmPayment(ConfirmPaymentTransactionRequest request);
}