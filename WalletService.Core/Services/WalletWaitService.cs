using AutoMapper;
using WalletService.Core.Services.IServices;
using WalletService.Data.Database.Models;
using WalletService.Data.Repositories.IRepositories;
using WalletService.Models.DTO.WalletWaitDto;
using WalletService.Models.Requests.WalletWaitRequest;

namespace WalletService.Core.Services;

public class WalletWaitService : BaseService, IWalletWaitService
{
    private readonly IWalletWaitRepository _walletWaitRepository;

    public WalletWaitService(IMapper mapper, IWalletWaitRepository walletWaitRepository) : base(mapper)
        => _walletWaitRepository = walletWaitRepository;

    public async Task<IEnumerable<WalletWaitDto>> GetAllWalletsWaits()
    {
        var response = await _walletWaitRepository.GetAllWalletsWaits();
        return Mapper.Map<IEnumerable<WalletWaitDto>>(response);
    }

    public async Task<WalletWaitDto?> GetWalletWaitById(int id)
    {
        var response = await _walletWaitRepository.GetWalletWaitById(id);
        return Mapper.Map<WalletWaitDto>(response);
    }

    public async Task<WalletWaitDto?> CreateWalletWaitAsync(WalletWaitRequest request)
    {
        var wallet = Mapper.Map<WalletsWait>(request);
        wallet = await _walletWaitRepository.CreateWalletWaitAsync(wallet);
        return Mapper.Map<WalletWaitDto>(wallet);
    }

    public async Task<WalletWaitDto?> UpdateWalletWaitAsync(int id, WalletWaitRequest request)
    {
        var wallet = await _walletWaitRepository.GetWalletWaitById(id);

        if (wallet is null)
            return null;
        wallet.AffiliateId   = request.AffiliateId;
        wallet.Credit        = request.Credit;
        wallet.PaymentMethod = request.PaymentMethod!;
        wallet.Bank          = request.Bank;
        wallet.Support       = request.Support;
        wallet.DepositDate   = request.DepositDate;
        wallet.Status        = request.Status;
        wallet.Attended      = request.Attended;
        wallet.Date          = request.Date;
        wallet.Order         = request.Order;
        wallet.UpdatedAt     = DateTime.Now;

        wallet = await _walletWaitRepository.UpdateWalletWaitAsync(wallet);

        return Mapper.Map<WalletWaitDto>(wallet);
    }

    public async Task<WalletWaitDto?> DeleteWalletWaitAsync(int id)
    {
        var wallet = await _walletWaitRepository.GetWalletWaitById(id);

        if (wallet is null)
            return null;

        await _walletWaitRepository.DeleteWalletWaitAsync(wallet);
        return Mapper.Map<WalletWaitDto>(wallet);
    }
}