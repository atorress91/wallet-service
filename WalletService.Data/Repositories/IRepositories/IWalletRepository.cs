using WalletService.Data.Database.CustomModels;
using WalletService.Data.Database.Models;
using WalletService.Models.Requests.WalletRequest;

namespace WalletService.Data.Repositories.IRepositories;

public interface IWalletRepository
{
    Task<List<Wallets>> GetWalletByAffiliateId(int affiliateId, int brandId);
    Task<List<InvoicesDetails>> GetDebitsModel2WithinMonth(DateTime from, DateTime to);
    Task<List<InvoicesDetails>> GetDebitsModel1AWithinMonth(DateTime from, DateTime to);
    Task<List<InvoicesDetails>> GetDebitsModel1BWithinMonth(DateTime from, DateTime to);
    Task<InvoicesSpResponse?> DebitEcoPoolTransactionSp(DebitTransactionRequest request);
    Task<List<InvoicesDetails>> GetDebitsModel2OutsideMonth(DateTime date);
    Task<List<InvoicesDetails>> GetDebitsModel1AOutsideMonth(DateTime date);
    Task<List<InvoicesDetails>> GetDebitsModel1BOutsideMonth(DateTime date);
    Task<List<InvoicesDetails>> GetInvoicesDetailsItemsForModel3(DateTime from, DateTime to);
    Task<List<Wallets>> GetWalletByUserId(int userId, int brandId);
    Task<List<Wallets>> GetWalletsRequest(int userId, int brandId);
    Task<Wallets?> GetWalletById(int id, int brandId);
    Task<bool> CreateModel2Sp(Model2TransactionRequest request);
    Task<bool> CreateModel1ASp(Model1ATransactionRequest request);
    Task<bool> CreateModel1BSp(Model1BTransactionRequest request);
    Task<bool> CreateModel3Sp(Model3TransactionRequest request);
    Task<Wallets> CreateWalletAsync(Wallets request);
    Task<Wallets> UpdateWalletAsync(Wallets request);
    Task<Wallets> DeleteWalletAsync(Wallets request);
    Task<decimal> GetAvailableBalanceByAffiliateId(int userId, int brandId);
    Task<decimal> GetAvailableBalanceAdmin(int brandId);
    Task<decimal?> GetReverseBalanceByAffiliateId(int userId, int brandId);
    Task<decimal?> GetTotalAcquisitionsByAffiliateId(int userId, int brandId);
    Task<InvoicesSpResponse?> DebitTransaction(DebitTransactionRequest request);
    Task<bool> CreditTransaction(CreditTransactionRequest request);
    Task<bool> CreateTransferBalance(Wallets debitTransaction, Wallets creditTransaction);
    Task<List<Wallets>> GetAllWallets(int brandId);
    Task<List<ModelFourStatistics>> GetUserModelFour(int[] affiliateIds);
    Task<double?> GetTotalCommissionsPaid(int affiliateId, int brandId);
    Task<decimal?> GetTotalServiceBalance(int affiliateId, int brandId);
    Task<bool> IsActivePoolGreaterThanOrEqualTo25(int affiliateId, int brandId);
    Task<InvoicesSpResponse?> HandleMembershipTransaction(DebitTransactionRequest request);
    Task<InvoicesSpResponse?> MembershipDebitTransaction(DebitTransactionRequest request);
    Task<InvoicesSpResponse?> AdminDebitTransaction(DebitTransactionRequest request);
    Task<bool> BulkAdministrativeDebitTransaction(Wallets[] requests);

    Task TransactionPoints(int affiliateId, decimal debitLeft, decimal debitRight, decimal creditLeft,
        decimal creditRight);

    Task<IEnumerable<AffiliateBalance>> GetAllAffiliatesWithPositiveBalance(int brandId);
    Task<InvoicesSpResponse?> CoursesDebitTransaction(DebitTransactionRequest request);
    Task<decimal> GetTotalReverseBalance(int brandId);

    Task<InvoicesSpResponse?> DebitServiceBalanceTransaction(DebitTransactionRequest request);

    Task<InvoicesSpResponse?> DebitServiceBalanceEcoPoolTransactionSp(DebitTransactionRequest request);

    Task<bool> CreditServiceBalanceTransaction(CreditTransactionRequest request);
    Task<bool> DistributeCommissionsPerPurchaseAsync(DistributeCommissionsRequest request);
}