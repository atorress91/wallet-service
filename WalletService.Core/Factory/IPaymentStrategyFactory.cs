using WalletService.Core.PaymentStrategies.IPaymentStrategies;
using WalletService.Models.Enums;

namespace WalletService.Core.Factory;

public interface IPaymentStrategyFactory
{
    IPaymentStrategy GetStrategy(PaymentType type);
    IMembershipPaymentStrategy GetMembershipStrategy();
    IBalancePaymentStrategy GetBalancePaymentStrategy();
    ICoinPaymentStrategy GetCoinPaymentStrategy();
}