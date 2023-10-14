using WalletService.Models.Requests.WalletRequest;

namespace WalletService.Core.PaymentStrategies.IPaymentStrategies;

public interface IPaymentStrategy
{
    Task<bool> ExecutePayment(WalletRequest request);
}