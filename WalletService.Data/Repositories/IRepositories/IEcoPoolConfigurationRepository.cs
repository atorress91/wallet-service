using WalletService.Data.Database.Models;

namespace WalletService.Data.Repositories.IRepositories;

public interface IEcoPoolConfigurationRepository
{
    Task<ModelConfiguration?> GetConfigurationByType(string modelType);
    Task<ModelConfiguration?> GetConfiguration();
    Task<ModelConfiguration> CreateConfiguration(ModelConfiguration poolConfiguration);
    Task CreateConfigurationLevels(IEnumerable<ModelConfigurationLevels>           levels);
    Task<ModelConfiguration> UpdateConfiguration(ModelConfiguration poolConfiguration);
    Task DeleteAllLevelsConfiguration(int                               configurationId);
    Task<ModelConfiguration> GetProgressPercentage(int                configurationId);
}