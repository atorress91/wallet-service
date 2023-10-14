using AutoMapper;
using WalletService.Core.Services.IServices;
using WalletService.Data.Database.Models;
using WalletService.Data.Repositories.IRepositories;
using WalletService.Models.DTO.WalletPeriodDto;
using WalletService.Models.Requests.WalletPeriodRequest;

namespace WalletService.Core.Services;

public class WalletPeriodService:BaseService,IWalletPeriodService
{
    private readonly IWalletPeriodRepository _walletPeriodRepository;

    public WalletPeriodService(IMapper mapper,IWalletPeriodRepository walletPeriodRepository) : base(mapper)
    {
        _walletPeriodRepository = walletPeriodRepository;
    }

    public async Task<IEnumerable<WalletPeriodDto>> GetAllWalletsPeriods()
    {
        var response = await _walletPeriodRepository.GetAllWalletsPeriods();
        return Mapper.Map<IEnumerable<WalletPeriodDto>>(response);
    }

    public async Task<WalletPeriodDto?> GetWalletPeriodById(int id)
    {
        var response = await _walletPeriodRepository.GetWalletPeriodById(id);
        return Mapper.Map<WalletPeriodDto>(response);
    }
    
    public async Task<IEnumerable<WalletPeriodDto>> CreateWalletPeriodAsync(IEnumerable<WalletPeriodRequest> request)
    {
        var createdPeriods = new List<WalletPeriodDto>();

        foreach (var periodData in request)
        {
            var existingPeriod = await _walletPeriodRepository.GetWalletPeriodById(periodData.Id);

            if (existingPeriod != null)
            {
                existingPeriod.Date      = periodData.Date;
                existingPeriod.Status    = periodData.Status;
                existingPeriod.UpdatedAt = DateTime.Now;

                await _walletPeriodRepository.UpdateWalletPeriodsAsync(new List<WalletsPeriods> { existingPeriod });

                createdPeriods.Add(Mapper.Map<WalletPeriodDto>(existingPeriod));
            }
            else
            {
                var newPeriod = new WalletsPeriods
                {
                    Date      = periodData.Date,
                    Status    = periodData.Status,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };

                await _walletPeriodRepository.CreateWalletPeriodAsync(new List<WalletsPeriods> { newPeriod });

                createdPeriods.Add(Mapper.Map<WalletPeriodDto>(newPeriod));
            }
        }

        return createdPeriods;
    }
    
    
    public async Task<WalletPeriodDto?> DeleteWalletPeriodAsync(int id)
    {
        var period = await _walletPeriodRepository.GetWalletPeriodById(id);

        if (period is null)
            return null;

        await _walletPeriodRepository.DeleteWalletPeriodAsync(period);
        return Mapper.Map<WalletPeriodDto>(period);
    }
}