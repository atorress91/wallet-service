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


    public async Task<EcoPoolConfigurationDto?> GetEcoPoolDefaultConfiguration()
    {
        var configuration = await _poolConfigurationRepository.GetConfiguration();

        return configuration is null ? null : Mapper.Map<EcoPoolConfigurationDto>(configuration);
    }

    public async Task<EcoPoolConfigurationDto> CreateOrUpdateEcoPoolConfiguration(EcoPoolConfigurationRequest request)
    {
        var configuration = await _poolConfigurationRepository.GetConfiguration();
        
        if (configuration is null)
            configuration = await CreateEcoPoolModel(request);
        else
            configuration = await UpdateEcoPoolModel(request, configuration);

        return Mapper.Map<EcoPoolConfigurationDto>(configuration);
    }
    

    private async Task<EcoPoolConfiguration> CreateEcoPoolModel(EcoPoolConfigurationRequest request)
    {
        var listLevels = new List<EcoPoolLevels>();
        var today      = DateTime.Today;

        var configuration = new EcoPoolConfiguration
        {
            CompanyPercentageLevels = request.CompanyPercentageLevels,
            CompanyPercentage       = request.CompanyPercentage,
            EcoPoolPercentage       = request.EcoPoolPercentage,
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
            var ecoPoolLevels = new EcoPoolLevels
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


    private async Task<EcoPoolConfiguration> UpdateEcoPoolModel(EcoPoolConfigurationRequest request, EcoPoolConfiguration configuration)
    {
        var listLevels = new List<EcoPoolLevels>();
        var today      = DateTime.Today;
        
        configuration.CompanyPercentageLevels = request.CompanyPercentageLevels;
        configuration.CompanyPercentage       = request.CompanyPercentage;
        configuration.EcoPoolPercentage       = request.EcoPoolPercentage;
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
            var ecoPoolLevels = new EcoPoolLevels
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