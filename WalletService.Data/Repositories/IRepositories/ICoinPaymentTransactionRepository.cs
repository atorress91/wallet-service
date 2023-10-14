using WalletService.Data.Database.Models;

namespace WalletService.Data.Repositories.IRepositories;

public interface ICoinPaymentTransactionRepository
{
    Task<List<PaymentTransaction>> GetAllCoinPaymentTransaction();
    Task<PaymentTransaction?> GetCoinPaymentTransactionByIdTransaction(string         idTransaction);
    Task<PaymentTransaction> CreateCoinPaymentTransaction(PaymentTransaction      request);
    Task<PaymentTransaction> UpdateCoinPaymentTransactionAsync(PaymentTransaction request);
    Task<List<PaymentTransaction>> GetAllUnconfirmedOrUnpaidTransactions();
}