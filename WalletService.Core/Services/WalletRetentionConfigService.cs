using AutoMapper;
using WalletService.Core.Services.IServices;
using WalletService.Data.Database.Models;
using WalletService.Data.Repositories;
using WalletService.Data.Repositories.IRepositories;
using WalletService.Models.DTO.WalletRetentionConfigDto;
using WalletService.Models.Requests.WalletRetentionConfigRequest;

namespace WalletService.Core.Services;

public class WalletRetentionConfigService : BaseService, IWalletRetentionConfigService
{
    private readonly IWalletRetentionConfigRepository _walletRetentionConfigRepository;

    public WalletRetentionConfigService(IMapper mapper, IWalletRetentionConfigRepository walletRetentionConfigRepository) : base(mapper)
        => _walletRetentionConfigRepository = walletRetentionConfigRepository;

    public async Task<IEnumerable<WalletRetentionConfigDto>> GetAllWalletsRetentionConfig()
    {
        var response = await _walletRetentionConfigRepository.GetAllWalletsRetentionConfig();
        return Mapper.Map<IEnumerable<WalletRetentionConfigDto>>(response);
    }

    public async Task<WalletRetentionConfigDto?> GetWalletRetentionConfigById(int id)
    {
        var response = await _walletRetentionConfigRepository.GetWalletRetentionConfigById(id);
        return Mapper.Map<WalletRetentionConfigDto>(response);
    }

    public async Task<IEnumerable<WalletRetentionConfigDto>> CreateWalletRetentionConfigAsync(
        IEnumerable<WalletRetentionConfigRequest> request)
    {
        var createdPeriods = new List<WalletRetentionConfigDto>();

        foreach (var periodData in request)
        {
            var existingPeriodRetention = await _walletRetentionConfigRepository.GetWalletRetentionConfigById(periodData.Id);

            if (existingPeriodRetention != null)
            {
                existingPeriodRetention.WithdrawalTo   = periodData.WithdrawalTo;
                existingPeriodRetention.WithdrawalFrom = periodData.WithdrawalFrom;
                existingPeriodRetention.DisableDate    = periodData.DisableDate;
                existingPeriodRetention.Percentage     = periodData.Percentage;
                existingPeriodRetention.Date           = periodData.Date;
                existingPeriodRetention.Status         = periodData.Status;
                existingPeriodRetention.UpdatedAt      = DateTime.Now;

                await _walletRetentionConfigRepository.UpdateWalletRetentionConfigAsync(new List<WalletsRetentionsConfig>
                    { existingPeriodRetention });
                createdPeriods.Add(Mapper.Map<WalletRetentionConfigDto>(existingPeriodRetention));
            }
            else
            {
                var newPeriodRetention = new WalletsRetentionsConfig
                {
                    WithdrawalFrom = periodData.WithdrawalFrom,
                    WithdrawalTo   = periodData.WithdrawalTo,
                    Percentage     = periodData.Percentage,
                    DisableDate    = periodData.DisableDate,
                    Date           = periodData.Date,
                    Status         = periodData.Status,
                    CreatedAt      = DateTime.Now,
                    UpdatedAt      = DateTime.Now
                };

                await _walletRetentionConfigRepository.CreateWalletRetentionConfigAsync(new List<WalletsRetentionsConfig>
                    { newPeriodRetention });
                createdPeriods.Add(Mapper.Map<WalletRetentionConfigDto>(newPeriodRetention));
            }
        }

        return createdPeriods;
    }

    public async Task<WalletRetentionConfigDto?> DeleteWalletRetentionConfigAsync(int id)
    {
        var retentionConfig = await _walletRetentionConfigRepository.GetWalletRetentionConfigById(id);

        if (retentionConfig is null)
            return null;

        await _walletRetentionConfigRepository.DeleteWalletRetentionConfigAsync(retentionConfig);
        return Mapper.Map<WalletRetentionConfigDto>(retentionConfig);
    }
}