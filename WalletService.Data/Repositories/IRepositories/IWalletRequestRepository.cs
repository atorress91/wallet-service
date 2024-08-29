using WalletService.Data.Database.Models;

namespace WalletService.Data.Repositories.IRepositories;

public interface IWalletRequestRepository
{
    Task<List<WalletsRequests>> GetAllWalletsRequests();
    Task<decimal>               GetTotalWalletRequestAmount(int brandId);
	Task<List<WalletsRequests>> GetAllWalletRequestByAffiliateId(int id);
    Task<List<WalletsRequests>> GetWalletRequestsByIds(List<int>     ids);
    Task<WalletsRequests?> CreateWalletRequestAsync(WalletsRequests   request);
    Task UpdateBulkWalletRequestsAsync(List<WalletsRequests>         request);
    Task<WalletsRequests> DeleteWalletRequestAsync(WalletsRequests   request);
    Task<decimal> GetTotalWalletRequestAmountByAffiliateId(int       id, int brandId);
    Task<List<WalletsRequests>> GetAllWalletRequestRevertTransaction();
    Task<WalletsRequests?> GetWalletRequestsByInvoiceNumber(int     invoiceId);
    Task<WalletsRequests> UpdateWalletRequestsAsync(WalletsRequests requests);
}