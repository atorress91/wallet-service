using WalletService.Models.Requests.WalletRequest;

namespace WalletService.Core.PaymentStrategies.IPaymentStrategies;

public interface ICoinPayPaymentStrategy
{
    Task<bool> ExecuteEcoPoolPayment(WalletRequest request);
    Task<bool> ExecuteCoursePayment(WalletRequest request);
    Task<bool> ExecuteMembershipPayment(WalletRequest request);
}