using WalletService.Models.Requests.EcoPoolConfigurationRequest;

namespace WalletService.Core.Services.IServices;

public interface IProcessGradingService
{
    Task EcoPoolProcess();
    Task PaymentProcess();
}