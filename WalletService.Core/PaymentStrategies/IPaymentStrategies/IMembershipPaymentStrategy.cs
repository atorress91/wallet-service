using WalletService.Models.Requests.WalletRequest;

namespace WalletService.Core.PaymentStrategies.IPaymentStrategies;

public interface IMembershipPaymentStrategy
{
    Task<bool> ExecutePayment(WalletRequest           request);
    Task<bool> ExecuteMembershipPayment(WalletRequest request);
}