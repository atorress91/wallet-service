namespace WalletService.Data.Repositories.IRepositories;

public interface IApiClientRepository
{
    Task<bool> ValidateApiClient(string token);
}