using System.Net;
using AutoMapper;
using Newtonsoft.Json;
using WalletService.Core.Services.IServices;
using WalletService.Data.Adapters.IAdapters;
using WalletService.Data.Database.Models;
using WalletService.Data.Repositories.IRepositories;
using WalletService.Models.Requests.MatrixRequest;
using WalletService.Models.Responses;

namespace WalletService.Core.Services;

public class MatrixService : BaseService, IMatrixService
{
    private readonly IConfigurationAdapter _configurationAdapter;
    private readonly IBrandService _brandService;
    private readonly IMatrixQualificationRepository _matrixQualificationRepository;
    private readonly IMatrixEarningsRepository _matrixEarningsRepository;
    private readonly IAccountServiceAdapter _accountServiceAdapter;

    public MatrixService(IMapper mapper, IConfigurationAdapter configurationAdapter, IBrandService brandService,
        IMatrixQualificationRepository matrixQualificationRepository,
        IMatrixEarningsRepository matrixEarningsRepository,
        IAccountServiceAdapter accountServiceAdapter) : base(mapper)
    {
        _brandService = brandService;
        _configurationAdapter = configurationAdapter;
        _matrixQualificationRepository = matrixQualificationRepository;
        _matrixEarningsRepository = matrixEarningsRepository;
        _accountServiceAdapter = accountServiceAdapter;
    }

    public async Task<bool> CheckQualificationAsync(long userId, int matrixType)
    {
        var matrixConfig = await _configurationAdapter.GetMatrixConfiguration(_brandService.BrandId, matrixType);

        if (matrixConfig.Content == null || matrixConfig.StatusCode != HttpStatusCode.OK)
        {
            throw new ApplicationException($"Error retrieving matrix configuration: {matrixConfig.StatusCode}");
        }

        var matrixConfigResponse = JsonConvert.DeserializeObject<MatrixConfigurationResponse>(matrixConfig.Content);
        var qualification = await _matrixQualificationRepository.GetByUserAndMatrixTypeAsync(userId, matrixType);

        if (qualification == null)
        {
            qualification = new MatrixQualification
            {
                UserId = userId,
                MatrixType = matrixType,
                TotalEarnings = 0,
                WithdrawnAmount = 0,
                AvailableBalance = 0,
                IsQualified = false
            };

            await _matrixQualificationRepository.CreateAsync(qualification);
            return false;
        }

        if (qualification.IsQualified)
            return true;

        var totalEarnings = await _matrixEarningsRepository.GetTotalEarningsAsync(userId, matrixType);
        qualification.TotalEarnings = totalEarnings;

        if (totalEarnings >= matrixConfigResponse!.Data!.Threshold)
        {
            qualification.IsQualified = true;
            qualification.QualificationDate = DateTime.Now;
            await _matrixQualificationRepository.UpdateAsync(qualification);
            return true;
        }

        await _matrixQualificationRepository.UpdateAsync(qualification);
        return false;
    }

    public async Task<bool> ProcessQualificationAsync(int userId, int matrixType)
    {
        var isQualified = await CheckQualificationAsync(userId, matrixType);

        if (!isQualified)
            return false;

        var matrixConfig = await _configurationAdapter.GetMatrixConfiguration(_brandService.BrandId, matrixType);

        var response = JsonConvert.DeserializeObject<MatrixConfigurationResponse>(matrixConfig!.Content!);

        var qualification = await _matrixQualificationRepository.GetByUserAndMatrixTypeAsync(userId, matrixType);

        if (qualification!.AvailableBalance < response!.Data!.FeeAmount)
            return false;

        qualification.AvailableBalance -= response.Data!.FeeAmount;
        qualification.WithdrawnAmount += response.Data!.FeeAmount;
        await _matrixQualificationRepository.UpdateAsync(qualification);

        var request = new MatrixRequest
        {
            UserId = userId,
            MatrixType = matrixType
        };
        
        var accountResponse = await _accountServiceAdapter.PlaceUserInMatrix(request, _brandService.BrandId);
    }

    public async Task<bool> ProcessMatrixCommissionsAsync(int userId, int matrixType)
    {
        var matrixConfigResponse = await _configurationAdapter.GetMatrixConfiguration(_brandService.BrandId, matrixType);
        var matrixConfig = JsonConvert.DeserializeObject<MatrixConfigurationResponse>(matrixConfigResponse.Content!);
        
        if (matrixConfigResponse.Content == null || matrixConfigResponse.StatusCode != HttpStatusCode.OK)
            throw new ApplicationException($"Error retrieving matrix configuration: {matrixConfigResponse.StatusCode}");
        
        if(matrixConfig?.Data is null)
            throw new ApplicationException($"Error deserialize matrix configuration: {matrixConfigResponse.StatusCode}");
        
        var position = await _accountServiceAdapter.GetByUserAndMatrixTypeAsync(new MatrixRequest { UserId = userId, MatrixType = matrixType }, _brandService.BrandId);

        var commissionAmount = matrixConfig.Data.FeeAmount * 0.1m;
        
    }
}