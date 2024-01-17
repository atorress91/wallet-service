using WalletService.Models.Requests.WalletRequest;

namespace WalletService.Core.PaymentStrategies.IPaymentStrategies;

public interface IBalancePaymentStrategyModel2
{
    Task<bool> ExecutePayment(WalletRequest request);
}