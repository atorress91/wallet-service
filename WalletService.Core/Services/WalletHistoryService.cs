using AutoMapper;
using WalletService.Core.Services.IServices;
using WalletService.Data.Database.Models;
using WalletService.Data.Repositories.IRepositories;
using WalletService.Models.DTO.WalletHistoryDto;
using WalletService.Models.Requests.WalletHistoryRequest;

namespace WalletService.Core.Services;

public class WalletHistoryService : BaseService, IWalletHistoryService
{
    private readonly IWalletHistoryRepository _walletHistoryRepository;

    public WalletHistoryService(IMapper mapper, IWalletHistoryRepository walletHistoryRepository) : base(mapper)
        => _walletHistoryRepository = walletHistoryRepository;

    #region walletsHistory

    public async Task<IEnumerable<WalletHistoryDto>> GetAllWalletsHistoriesAsync()
    {
        var response = await _walletHistoryRepository.GetAllWalletsHistoriesAsync();

        return Mapper.Map<IEnumerable<WalletHistoryDto>>(response);
    }

    public async Task<WalletHistoryDto?> GetWalletHistoriesByIdAsync(int id)
    {
        var response = await _walletHistoryRepository.GetWalletHistoriesByIdAsync(id);
        return Mapper.Map<WalletHistoryDto>(response);
    }

    public async Task<WalletHistoryDto?> CreateWalletHistoriesAsync(WalletHistoryRequest request)
    {
        var history = Mapper.Map<WalletsHistories>(request);
        history = await _walletHistoryRepository.CreateWalletHistoriesAsync(history);

        return Mapper.Map<WalletHistoryDto>(history);
    }

    public async Task<WalletHistoryDto?> UpdateWalletHistoriesAsync(int id, WalletHistoryRequest request)
    {
        var history = await _walletHistoryRepository.GetWalletHistoriesByIdAsync(id);

        if (history is null)
            return null;
        history.AffiliateId = request.AffiliateId;
        history.UserId      = request.UserId;
        history.Credit      = request.Credit;
        history.Debit       = request.Debit;
        history.Deferred    = request.Deferred;
        history.Status      = request.Status;
        history.Concept     = request.Concept;
        history.Support     = request.Support;
        history.Date        = request.Date;
        history.UpdatedAt   = DateTime.Now;


        history = await _walletHistoryRepository.UpdateWalletHistoriesAsync(history);

        return Mapper.Map<WalletHistoryDto>(history);
    }

    public async Task<WalletHistoryDto?> DeleteWalletHistoriesAsync(int id)
    {
        var history = await _walletHistoryRepository.GetWalletHistoriesByIdAsync(id);

        if (history is null)
            return null;

        await _walletHistoryRepository.DeleteWalletHistoriesAsync(history);

        return Mapper.Map<WalletHistoryDto>(history);
    }

    #endregion

}