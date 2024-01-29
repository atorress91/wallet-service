using WalletService.Data.Database.CustomModels;
using WalletService.Data.Database.Models;
using WalletService.Models.Requests.WalletRequest;

namespace WalletService.Data.Repositories.IRepositories;

public interface IWalletRepository
{
    Task<List<Wallets>> GetWalletByAffiliateId(int                              affiliateId);
    Task<List<InvoicesDetails>> GetDebitsEcoPoolWithinMonth(DateTime            from, DateTime to);
    Task<InvoicesSpResponse?> DebitEcoPoolTransactionSp(DebitTransactionRequest request);
    Task<List<InvoicesDetails>> GetDebitsEcoPoolOutsideMonth(DateTime           date);
    Task<List<InvoicesDetails>> GetInvoicesDetailsItemsForModelTwo(int          month, int year);
    Task<List<Wallets>> GetWalletByUserId(int                                   userId);
    Task<List<Wallets>> GetWalletsRequest(int                                   userId);
    Task<Wallets?> GetWalletById(int                                            id);
    Task<bool> CreateModelThreeSp(ModelThreeTransactionRequest                  request);
    Task<bool> CreateModelTwoSp(ModelTwoTransactionRequest                      request);
    Task<Wallets> CreateWalletAsync(Wallets                                     request);
    Task<Wallets> UpdateWalletAsync(Wallets                                     request);
    Task<Wallets> DeleteWalletAsync(Wallets                                     request);
    Task<decimal> GetAvailableBalanceByAffiliateId(int                          userId);
    Task<decimal> GetAvailableBalanceAdmin();
    Task<decimal?> GetReverseBalanceByAffiliateId(int                  userId);
    Task<decimal?> GetTotalAcquisitionsByAffiliateId(int               userId);
    Task<InvoicesSpResponse?> DebitTransaction(DebitTransactionRequest request);
    Task<bool> CreditTransaction(CreditTransactionRequest              request);
    Task<bool> CreateTransferBalance(Wallets                           debitTransaction, Wallets creditTransaction);
    Task<List<Wallets>> GetAllWallets();
    Task<List<ModelFourStatistics>> GetUserModelFour(int[]                        affiliateIds);
    Task<double?> GetTotalCommissionsPaid(int                                     affiliateId);
    Task<double?> GetTotalServiceBalance(int                                     affiliateId);
    Task<bool> IsActivePoolGreaterThanOrEqualTo25(int                             affiliateId);
    Task<InvoicesSpResponse?> HandleMembershipTransaction(DebitTransactionRequest request);
    Task<InvoicesSpResponse?> MembershipDebitTransaction(DebitTransactionRequest  request);
    Task<InvoicesSpResponse?> AdminDebitTransaction(DebitTransactionRequest       request);
    Task<bool> BulkAdministrativeDebitTransaction(Wallets[]                       requests);
    Task TransactionPoints(int affiliateId, decimal debitLeft, decimal debitRight, decimal creditLeft, decimal creditRight);
    Task<IEnumerable<AffiliateBalance>> GetAllAffiliatesWithPositiveBalance();
    Task<InvoicesSpResponse?> CoursesDebitTransaction(DebitTransactionRequest request);
    Task<decimal> GetTotalReverseBalance();
    
    Task<InvoicesSpResponse?> DebitServiceBalanceTransaction(DebitTransactionRequest request);

    Task<InvoicesSpResponse?> DebitServiceBalanceEcoPoolTransactionSp(DebitTransactionRequest request);

    Task<bool> CreditServiceBalanceTransaction(CreditTransactionRequest request);
}