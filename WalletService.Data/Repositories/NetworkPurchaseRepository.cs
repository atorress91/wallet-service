using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;
using WalletService.Data.Database;
using WalletService.Data.Database.CustomModels;
using WalletService.Data.Database.Models;
using WalletService.Data.Repositories.IRepositories;
using WalletService.Models.Constants;

namespace WalletService.Data.Repositories;

public class NetworkPurchaseRepository : BaseRepository, INetworkPurchaseRepository
{
    public NetworkPurchaseRepository(WalletServiceDbContext context) : base(context) { }

    public Task<List<NetworkPurchase>> GetNetworkPurchasesEcoPool(DateTime from, DateTime to)
        => Context.NetworkPurchases
            .Where(x => x.CreatedAt >= from && x.CreatedAt <= to
                                            && x.Origin == Constants.OriginEcoPoolPurchase).ToListAsync();


    public async Task<List<(int Year, int Month, int TotalPurchases)>> GetPurchasesMadeInMyNetwork(HashSet<int> ids)
    {
        if (ids.Count == 0)
            return new List<(int, int, int)>();
  
        DataTable dt = new DataTable();
        dt.Columns.Add("AffiliateId", typeof(int));
        foreach (var id in ids)
        {
            dt.Rows.Add(id);
        }

        var parameter = new SqlParameter("@AffiliateIds", SqlDbType.Structured)
        {
            TypeName = Constants.TypeTableAffiliateId,
            Value    = dt
        };

        var results = await Context.Set<PurchasesPerMonth>()
            .FromSqlRaw("EXEC " + Constants.GetTotalPurchasesInMyNetworkSp + " @AffiliateIds", parameter)
            .ToListAsync();

        return results.Select(r => (r.Year, r.Month, r.TotalPurchases)).ToList();
    }







}