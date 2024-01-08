using WalletService.Data.Database;
using WalletService.Data.Database.CustomModels;
using WalletService.Data.Database.Models;
using WalletService.Data.Repositories.IRepositories;
using WalletService.Models.Requests.WalletRequest;

namespace WalletService.Data.Repositories;

public class WalletModel1ARepository : BaseRepository, IWalletModel1ARepository
{
    
    public WalletModel1ARepository(WalletServiceDbContext context) : base(context) { }

    public Task<InvoicesSpResponse?> DebitTransaction(DebitTransactionRequest request)
    {
        throw new NotImplementedException();
    }

    public Task<bool> CreditTransaction(CreditTransactionRequest              request)
    {
        throw new NotImplementedException();
    }

    public Task<bool> CreateTransferBalance(Wallets                                 debitTransaction, Wallets creditTransaction)
    {
        throw new NotImplementedException();
    }


}