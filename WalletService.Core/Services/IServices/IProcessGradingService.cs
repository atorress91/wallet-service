using WalletService.Models.Requests.EcoPoolConfigurationRequest;

namespace WalletService.Core.Services.IServices;

public interface IProcessGradingService
{
    Task ExecuteFirstProcess();
    Task ExecuteSecondProcess();
}