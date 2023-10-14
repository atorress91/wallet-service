using Microsoft.Extensions.DependencyInjection;
using WalletService.Core.PaymentStrategies;
using WalletService.Core.PaymentStrategies.IPaymentStrategies;
using WalletService.Models.Enums;

namespace WalletService.Core.Factory;

public class PaymentStrategyFactory : IPaymentStrategyFactory
{
    private readonly IServiceProvider _serviceProvider;

    public PaymentStrategyFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public IPaymentStrategy GetStrategy(PaymentType type)
    {
        switch (type)
        {
            case PaymentType.ReversedBalance:
                return _serviceProvider.GetRequiredService<ReversedBalancePaymentStrategy>();
            case PaymentType.PaymentToThirdParties:
                return _serviceProvider.GetRequiredService<ToThirdPartiesPaymentStrategy>();
            case PaymentType.CoinPayments:
                return _serviceProvider.GetRequiredService<CoinPaymentsPaymentStrategy>();
            default:
                throw new ArgumentException($"No strategy available for {type}", nameof(type));
        }
    }

    public IMembershipPaymentStrategy GetMembershipStrategy()
    {
        return _serviceProvider.GetRequiredService<MembershipPaymentStrategy>();
    }

    public IBalancePaymentStrategy GetBalancePaymentStrategy()
    {
        return _serviceProvider.GetRequiredService<BalancePaymentStrategy>();
    }

}