using WalletService.Models.Requests.WalletRequest;

namespace WalletService.Core.PaymentStrategies.IPaymentStrategies;

public interface IWireTransferStrategy
{
    Task<bool> ExecuteEcoPoolPayment(WalletRequest request);
    Task<bool> ExecutePaymentCourses(WalletRequest request);
}