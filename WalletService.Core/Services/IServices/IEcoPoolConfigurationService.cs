using WalletService.Models.DTO.ProcessGradingDto;
using WalletService.Models.Requests.EcoPoolConfigurationRequest;

namespace WalletService.Core.Services.IServices;

public interface IEcoPoolConfigurationService
{
    Task<ModelConfigurationDto?> GetEcoPoolDefaultConfiguration();
    Task<ModelConfigurationDto> CreateOrUpdateEcoPoolConfiguration(EcoPoolConfigurationRequest request);
    Task<int> GetProgressPercentage(int                                                          configurationId);
}