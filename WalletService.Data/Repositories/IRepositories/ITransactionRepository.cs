using WalletService.Data.Database.Models;

namespace WalletService.Data.Repositories.IRepositories;

public interface ITransactionRepository
{
    Task<List<Transaction>> GetAllTransaction(long brandId);
    Task<Transaction?> GetTransactionByIdTransaction(string idTransaction, long brandId);
    Task<Transaction?> CreateTransaction(Transaction request);
    Task<Transaction?> UpdateTransactionAsync(Transaction request);
    Task<List<Transaction>> GetAllUnconfirmedOrUnpaidTransactions(long brandId);
    Task<List<Transaction>> GetAllWireTransfer(long brandId);
    Task<Transaction?> GetPaymentTransactionById(int id, long brandId);
    Task<int> GetLastTransactionId(long brandId);
    Task<Transaction?> GetTransactionByReference(string reference);
    Task<Transaction?> GetTransactionByTxnId(string idTransaction);
}