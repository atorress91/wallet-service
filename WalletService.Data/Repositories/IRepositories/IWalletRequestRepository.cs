﻿using WalletService.Data.Database.Models;

namespace WalletService.Data.Repositories.IRepositories;

public interface IWalletRequestRepository
{
    Task<List<WalletsRequest>> GetAllWalletsRequests(long brandId);
    Task<decimal>               GetTotalWalletRequestAmount(long brandId);
	Task<List<WalletsRequest>> GetAllWalletRequestByAffiliateId(int id);
    Task<List<WalletsRequest>> GetWalletRequestsByIds(List<long>     ids);
    Task<WalletsRequest?> CreateWalletRequestAsync(WalletsRequest   request);
    Task UpdateBulkWalletRequestsAsync(List<WalletsRequest>         request);
    Task<WalletsRequest> DeleteWalletRequestAsync(WalletsRequest   request);
    Task<decimal> GetTotalWalletRequestAmountByAffiliateId(int       id, long brandId);
    Task<List<WalletsRequest>> GetAllWalletRequestRevertTransaction(long brandId);
    Task<WalletsRequest?> GetWalletRequestsByInvoiceNumber(int     invoiceId);
    Task<WalletsRequest> UpdateWalletRequestsAsync(WalletsRequest requests);
    Task<decimal?> GetTotalWithdrawnByAffiliateId(long affiliateId);
}