using Microsoft.EntityFrameworkCore;
using WalletService.Data.Database;
using WalletService.Data.Repositories.IRepositories;

namespace WalletService.Data.Repositories;

public class ApiClientRepository : BaseRepository, IApiClientRepository
{
    public ApiClientRepository(WalletServiceDbContext context) : base(context) { }


    public Task<bool> ValidateApiClient(string token)
        => Context.ApiClient.AnyAsync(x => x.Token == token);

}