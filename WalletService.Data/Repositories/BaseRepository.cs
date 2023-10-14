using WalletService.Data.Database;

namespace WalletService.Data.Repositories;

public class BaseRepository
{
    protected readonly WalletServiceDbContext Context;

    protected BaseRepository(WalletServiceDbContext context)
        => Context = context;
}