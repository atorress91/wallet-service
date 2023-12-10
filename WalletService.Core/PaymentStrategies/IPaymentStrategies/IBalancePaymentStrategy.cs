using WalletService.Models.Requests.WalletRequest;
using Task = sib_api_v3_sdk.Model.Task;

namespace WalletService.Core.PaymentStrategies.IPaymentStrategies;

public interface IBalancePaymentStrategy
{
    Task<bool> ExecuteEcoPoolPayment(WalletRequest request);
    Task<bool> ExecuteAdminPayment(WalletRequest   request);
    Task<bool> ExecutePaymentCourses(WalletRequest request);
    Task<bool> ExecuteCustomPayment(WalletRequest  request);
}