using WalletService.Models.Requests.WalletRequest;

namespace WalletService.Core.PaymentStrategies.IPaymentStrategies;

public interface ICoinPaymentStrategy
{
    Task<bool> ExecuteEcoPoolPayment(WalletRequest request);
    Task<bool> ExecuteCoursePayment(WalletRequest request);
}