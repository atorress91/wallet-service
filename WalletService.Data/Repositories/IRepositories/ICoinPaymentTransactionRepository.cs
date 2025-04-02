using WalletService.Data.Database.Models;

namespace WalletService.Data.Repositories.IRepositories;

public interface ICoinPaymentTransactionRepository
{
    Task<List<CoinpaymentTransaction>> GetAllCoinPaymentTransaction(long brandId);
    Task<CoinpaymentTransaction?> GetCoinPaymentTransactionByIdTransaction(string idTransaction, long brandId);
    Task<CoinpaymentTransaction?> CreateCoinPaymentTransaction(CoinpaymentTransaction request);
    Task<CoinpaymentTransaction?> UpdateCoinPaymentTransactionAsync(CoinpaymentTransaction request);
    Task<List<CoinpaymentTransaction>> GetAllUnconfirmedOrUnpaidTransactions(long brandId);
    Task<List<CoinpaymentTransaction>> GetAllWireTransfer(long brandId);
    Task<CoinpaymentTransaction?> GetPaymentTransactionById(int id, long brandId);
    Task<int> GetLastTransactionId(long brandId);
    Task<CoinpaymentTransaction?> GetTransactionByReference(string reference);
    Task<CoinpaymentTransaction?> GetTransactionByTxnId(string idTransaction);
}