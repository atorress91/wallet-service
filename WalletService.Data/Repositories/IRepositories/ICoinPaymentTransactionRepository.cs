using WalletService.Data.Database.Models;

namespace WalletService.Data.Repositories.IRepositories;

public interface ICoinPaymentTransactionRepository
{
    Task<List<PaymentTransaction>> GetAllCoinPaymentTransaction(int brandId);
    Task<PaymentTransaction?> GetCoinPaymentTransactionByIdTransaction(string idTransaction, int brandId);
    Task<PaymentTransaction?> CreateCoinPaymentTransaction(PaymentTransaction request);
    Task<PaymentTransaction?> UpdateCoinPaymentTransactionAsync(PaymentTransaction request);
    Task<List<PaymentTransaction>> GetAllUnconfirmedOrUnpaidTransactions(int brandId);
    Task<List<PaymentTransaction>> GetAllWireTransfer(int brandId);
    Task<PaymentTransaction?> GetPaymentTransactionById(int id, int brandId);
    Task<int> GetLastTransactionId(int brandId);
    Task<PaymentTransaction?> GetTransactionByReference(string reference);
    Task<PaymentTransaction?> GetTransactionByTxnId(string idTransaction);
}