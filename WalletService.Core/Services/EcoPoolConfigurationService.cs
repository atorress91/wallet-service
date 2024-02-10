using AutoMapper;
using iText.StyledXmlParser.Jsoup.Select;
using WalletService.Core.Services.IServices;
using WalletService.Data.Database.Models;
using WalletService.Data.Repositories.IRepositories;
using WalletService.Models.DTO.ProcessGradingDto;
using WalletService.Models.Requests.EcoPoolConfigurationRequest;
using WalletService.Utility.Extensions;

namespace WalletService.Core.Services;

public class EcoPoolConfigurationService : BaseService, IEcoPoolConfigurationService
{
    private readonly IEcoPoolConfigurationRepository _poolConfigurationRepository;

    public EcoPoolConfigurationService(
        IEcoPoolConfigurationRepository poolConfigurationRepository,
        IMapper                         mapper) : base(mapper)
        => _poolConfigurationRepository = poolConfigurationRepository;


    public async Task<ModelConfigurationDto?> GetEcoPoolDefaultConfiguration()
    {
        var configuration = await _poolConfigurationRepository.GetConfiguration();

        return configuration is null ? null : Mapper.Map<ModelConfigurationDto>(configuration);
    }

    public async Task<ModelConfigurationDto> CreateOrUpdateEcoPoolConfiguration(EcoPoolConfigurationRequest request)
    {
        var configuration = await _poolConfigurationRepository.GetConfiguration();
        
        if (configuration is null)
            configuration = await CreateEcoPoolModel(request);
        else
            configuration = await UpdateEcoPoolModel(request, configuration);

        return Mapper.Map<ModelConfigurationDto>(configuration);
    }
    

    private async Task<ModelConfiguration> CreateEcoPoolModel(EcoPoolConfigurationRequest request)
    {
        var listLevels = new List<ModelConfigurationLevels>();
        var today      = DateTime.Today;

        var configuration = new ModelConfiguration
        {
            CompanyPercentageLevels = request.CompanyPercentageLevels,
            CompanyPercentage       = request.CompanyPercentage,
            ModelPercentage       = request.EcoPoolPercentage,
            MaxGainLimit            = request.MaxGainLimit,
            DateInit                = request.DateInit,
            DateEnd                 = request.DateEnd,
            Case                    = request.Case,
            CreatedAt               = today,
            UpdatedAt               = today,
        };

        configuration = await _poolConfigurationRepository.CreateConfiguration(configuration);

        var index = 0;
        foreach (var level in request.Levels)
        {
            index++;
            var ecoPoolLevels = new ModelConfigurationLevels
            {
                Level                  = index,
                EcoPoolConfigurationId = configuration.Id,
                CreatedAt              = today,
                UpdatedAt              = today,
                Percentage             = level.Percentage
            };

            listLevels.Add(ecoPoolLevels);
        }

        await _poolConfigurationRepository.CreateConfigurationLevels(listLevels);

        return configuration;
    }


    private async Task<ModelConfiguration> UpdateEcoPoolModel(EcoPoolConfigurationRequest request, ModelConfiguration configuration)
    {
        var listLevels = new List<ModelConfigurationLevels>();
        var today      = DateTime.Today;
        
        configuration.CompanyPercentageLevels = request.CompanyPercentageLevels;
        configuration.CompanyPercentage       = request.CompanyPercentage;
        configuration.ModelPercentage       = request.EcoPoolPercentage;
        configuration.MaxGainLimit            = request.MaxGainLimit;
        configuration.DateInit                = request.DateInit;
        configuration.DateEnd                 = request.DateEnd;
        configuration.Case                    = request.Case;
        configuration.CreatedAt               = today;
        configuration.UpdatedAt               = today;

        configuration = await _poolConfigurationRepository.UpdateConfiguration(configuration);
        await _poolConfigurationRepository.DeleteAllLevelsConfiguration(configuration.Id);

        var index = 0;
        foreach (var level in request.Levels)
        {
            index++;
            var ecoPoolLevels = new ModelConfigurationLevels
            {
                Level                  = index,
                EcoPoolConfigurationId = configuration.Id,
                CreatedAt              = today,
                UpdatedAt              = today,
                Percentage             = level.Percentage
            };

            listLevels.Add(ecoPoolLevels);
        }

        await _poolConfigurationRepository.CreateConfigurationLevels(listLevels);

        return configuration;
    }

    public async Task<int> GetProgressPercentage(int configurationId)
    {
        var result = await _poolConfigurationRepository.GetProgressPercentage(configurationId);
        
        if (result is null || result.Processed is 0)
            return 0;

        if (result.Totals is 0)
            return 0;
        var valueResult = (result.Processed * 100) / result.Totals;
        return valueResult ?? 0;
    }

}