using WalletService.Models.Requests.WalletRequest;

namespace WalletService.Core.PaymentStrategies.IPaymentStrategies;

public interface IBalancePaymentStrategyModel1B
{
    Task<bool> ExecuteEcoPoolPayment(WalletRequest request);
}