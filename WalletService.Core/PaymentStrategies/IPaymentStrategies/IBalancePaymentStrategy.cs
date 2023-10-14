using WalletService.Models.Requests.WalletRequest;

namespace WalletService.Core.PaymentStrategies.IPaymentStrategies;

public interface IBalancePaymentStrategy
{
    Task<bool> ExecutePayment(WalletRequest      request);
    Task<bool> ExecuteAdminPayment(WalletRequest request);
}