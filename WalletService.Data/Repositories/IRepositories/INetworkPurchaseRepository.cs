using WalletService.Data.Database.Models;

namespace WalletService.Data.Repositories.IRepositories;

public interface INetworkPurchaseRepository
{
    Task<List<NetworkPurchases>> GetNetworkPurchasesEcoPool(DateTime from, DateTime to);
    Task<List<(int Year, int Month, int TotalPurchases)>> GetPurchasesMadeInMyNetwork(HashSet<int> ids);
}