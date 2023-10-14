using WalletService.Data.Database.Models;

namespace WalletService.Data.Repositories.IRepositories;

public interface IEcoPoolConfigurationRepository
{
    Task<EcoPoolConfiguration?> GetConfiguration();
    Task<EcoPoolConfiguration> CreateConfiguration(EcoPoolConfiguration poolConfiguration);
    Task CreateConfigurationLevels(IEnumerable<EcoPoolLevels>           levels);
    Task<EcoPoolConfiguration> UpdateConfiguration(EcoPoolConfiguration poolConfiguration);
    Task DeleteAllLevelsConfiguration(int                               configurationId);
    Task<EcoPoolConfiguration> GetProgressPercentage(int                configurationId);
}