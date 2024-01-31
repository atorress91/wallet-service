using WalletService.Data.Database.CustomModels;
using WalletService.Data.Database.Models;
using WalletService.Models.Requests.WalletRequest;

namespace WalletService.Data.Repositories.IRepositories;

public interface IWalletModel1BRepository
{
    Task<InvoicesSpResponse?> DebitTransaction(DebitTransactionRequest request);

    Task<InvoicesSpResponse?> DebitEcoPoolTransactionSp(DebitTransactionRequest request);

    Task<bool>     CreditTransaction(CreditTransactionRequest request);
    Task<decimal>  GetAvailableBalanceByAffiliateId(int       affiliateId);
    Task<decimal?> GetTotalAcquisitionsByAffiliateId(int      affiliateId);
    Task<decimal?> GetReverseBalanceByAffiliateId(int         affiliateId);
    
    Task<double?> GetTotalServiceBalance(int affiliateId);
    
    Task<InvoicesSpResponse?> DebitServiceBalanceTransaction(DebitTransactionRequest request);

    Task<InvoicesSpResponse?> DebitServiceBalanceEcoPoolTransactionSp(DebitTransactionRequest request);

    Task<bool> CreditServiceBalanceTransaction(CreditTransactionRequest request);

}