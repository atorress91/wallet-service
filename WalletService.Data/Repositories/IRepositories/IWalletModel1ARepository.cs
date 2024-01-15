using WalletService.Data.Database.CustomModels;
using WalletService.Data.Database.Models;
using WalletService.Models.Requests.WalletRequest;

namespace WalletService.Data.Repositories.IRepositories;

public interface IWalletModel1ARepository
{
    Task<InvoicesSpResponse?> DebitTransaction(DebitTransactionRequest request);

    Task<InvoicesSpResponse?> DebitEcoPoolTransactionSp(DebitTransactionRequest request);

    Task<bool> CreditTransaction(CreditTransactionRequest request);
}