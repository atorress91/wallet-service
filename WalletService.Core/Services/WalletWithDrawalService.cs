using AutoMapper;
using WalletService.Core.Services.IServices;
using WalletService.Data.Database.Models;
using WalletService.Data.Repositories.IRepositories;
using WalletService.Models.DTO.WalletWithDrawalDto;
using WalletService.Models.Requests.WalletWithDrawalRequest;

namespace WalletService.Core.Services;

public class WalletWithDrawalService : BaseService, IWalletWithdrawalService
{
    private readonly IWalletWithDrawalRepository _walletWithDrawalRepository;

    public WalletWithDrawalService(IMapper mapper, IWalletWithDrawalRepository walletWithDrawalRepository) : base(mapper)
        => _walletWithDrawalRepository = walletWithDrawalRepository;

    public async Task<IEnumerable<WalletWithDrawalDto>> GetAllWalletsWithdrawals()
    {
        var response = await _walletWithDrawalRepository.GetAllWalletsWithdrawals();

        return Mapper.Map<IEnumerable<WalletWithDrawalDto>>(response);
    }

    public async Task<WalletWithDrawalDto?> GetWalletWithdrawalById(int id)
    {
        var response = await _walletWithDrawalRepository.GetWalletWithdrawalById(id);

        return Mapper.Map<WalletWithDrawalDto>(response);
    }

    public async Task<WalletWithDrawalDto?> CreateWalletWithdrawalAsync(WalletWithDrawalRequest request)
    {
        var wallet = Mapper.Map<WalletsWithdrawals>(request);
        wallet = await _walletWithDrawalRepository.CreateWalletWithdrawalAsync(wallet);
        return Mapper.Map<WalletWithDrawalDto>(wallet);
    }

    public async Task<WalletWithDrawalDto?> UpdateWalletWithdrawalAsync(int id, WalletWithDrawalRequest request)
    {
        var wallet = await _walletWithDrawalRepository.GetWalletWithdrawalById(id);

        if (wallet is null)
            return null;
        wallet.AffiliateId = request.AffiliateId;
        wallet.Amount      = request.Amount;

        wallet.Observation         = request.Observation;
        wallet.AdminObservation    = request.AdminObservation;
        wallet.Date                = request.Date;
        wallet.ResponseDate        = request.ResponseDate;
        wallet.RetentionPercentage = request.RetentionPercentage;
        wallet.Status              = request.Status;
        wallet                     = await _walletWithDrawalRepository.UpdateWalletWithdrawalAsync(wallet);

        return Mapper.Map<WalletWithDrawalDto>(wallet);
    }

    public async Task<WalletWithDrawalDto?> DeleteWalletWithdrawalAsync(int id)
    {
        var wallet = await _walletWithDrawalRepository.GetWalletWithdrawalById(id);

        if (wallet is null)
            return null;

        await _walletWithDrawalRepository.DeleteWalletWithdrawalAsync(wallet);
        return Mapper.Map<WalletWithDrawalDto>(wallet);
    }
}