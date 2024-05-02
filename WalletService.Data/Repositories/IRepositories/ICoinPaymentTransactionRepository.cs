using WalletService.Data.Database.Models;

namespace WalletService.Data.Repositories.IRepositories;

public interface ICoinPaymentTransactionRepository
{
    Task<List<PaymentTransaction>> GetAllCoinPaymentTransaction();
    Task<PaymentTransaction?>      GetCoinPaymentTransactionByIdTransaction(string idTransaction);
    Task<PaymentTransaction?>      CreateCoinPaymentTransaction(PaymentTransaction request);
    Task<PaymentTransaction?> UpdateCoinPaymentTransactionAsync(PaymentTransaction request);
    Task<List<PaymentTransaction>> GetAllUnconfirmedOrUnpaidTransactions();
    Task<List<PaymentTransaction>> GetAllWireTransfer();
    Task<PaymentTransaction?> GetPaymentTransactionById(int id);
    Task<int> GetLastTransactionId();
    Task<PaymentTransaction?> GetTransactionByReference(string reference);
}