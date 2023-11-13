using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Data;
using System.Text.Json;
using WalletService.Data.Database;
using WalletService.Data.Database.CustomModels;
using WalletService.Data.Database.Models;
using WalletService.Data.Repositories.IRepositories;
using WalletService.Models.Configuration;
using WalletService.Models.Constants;
using WalletService.Models.Enums;
using WalletService.Models.Requests.WalletRequest;
using WalletService.Utility.Extensions;
using static WalletService.Utility.Extensions.CommonExtensions;

namespace WalletService.Data.Repositories;

public class WalletRepository : BaseRepository, IWalletRepository
{
    private readonly ApplicationConfiguration _appSettings;

    public WalletRepository(IOptions<ApplicationConfiguration> appSettings, WalletServiceDbContext context) : base(context)
        => _appSettings = appSettings.Value;

    public Task<List<Wallets>> GetWalletByAffiliateId(int affiliateId)
        => Context.Wallets.Where(x => x.AffiliateId == affiliateId).ToListAsync();

    public async Task<decimal> GetAvailableBalanceByAffiliateId(int affiliateId)
    {
        var list = await Context.Wallets
            .Where(x => x.AffiliateId == affiliateId && x.Status == true).ToListAsync();

        var result = list.Sum(x => x.Credit - x.Debit);
        return result.ToDecimal();
    }

    public async Task<IEnumerable<AffiliateBalance>> GetAllAffiliatesWithPositiveBalance()
    {
        var list = await Context.Wallets
            .Where(x => x.Status == true)
            .GroupBy(x => x.AffiliateId)
            .Select(g => new AffiliateBalance 
            {
                AffiliateId = g.Key,
                AffiliateUserName = g.FirstOrDefault()!.AffiliateUserName,
                Balance = g.Sum(x => x.Credit - x.Debit)
            })
            .Where(ab => ab.Balance > 0)
            .ToListAsync();

        return list;
    }

    public async Task<decimal> GetAvailableBalanceAdmin()
    {
        var result = await Context.Wallets
            .Where(x => x.Status == true)
            .Select(x => x.Credit - x.Debit).SumAsync();

        return (decimal)result;
    }

    public async Task<decimal?> GetReverseBalanceByAffiliateId(int affiliateId)
    {
        var totalCredits = await Context.Wallets
            .Where(x => x.AffiliateId == affiliateId && x.ConceptType == WalletConceptType.revert_pool.ToString())
            .Select(x => x.Credit)
            .SumAsync();

        var totalDebits = await Context.Wallets
            .Where(x => x.AffiliateId == affiliateId && x.ConceptType == WalletConceptType.purchase_with_reverse_balance.ToString())
            .Select(x => x.Debit)
            .SumAsync();

        var reverseBalance = totalCredits - totalDebits;
        return Convert.ToDecimal(reverseBalance);
    }

    public Task<decimal?> GetTotalAcquisitionsByAffiliateId(int affiliateId)
        => Context.InvoicePacks.Include(x => x.Invoice)
            .Where(x => x.Invoice.AffiliateId == affiliateId && x.Status == "1")
            .GroupBy(x => x.Invoice.AffiliateId)
            .Select(g => (decimal?)g.Sum(x => x.BaseAmount))
            .FirstOrDefaultAsync();


    public async Task<double?> GetTotalCommissionsPaid(int affiliateId)
    {
        var total = await Context.Wallets
            .Where(x => x.AffiliateId == affiliateId &&
                        (x.ConceptType == "commission_passed_wallet" || x.ConceptType == "purchasing_pool") &&
                        x.Status == true)
            .SumAsync(x => x.Credit);

        return total;
    }

    public async Task<InvoicesSpResponse?> DebitTransaction(DebitTransactionRequest request)
    {
        try
        {
            await using var sql = new SqlConnection(_appSettings.ConnectionStrings?.SqlServerConnection);
            await using var cmd = new SqlCommand(Constants.DebitTransationSP, sql);

            var invoicesDetails = ConvertToDataTable(request.invoices);

            CreateDebitListParameters(request, invoicesDetails, cmd);

            await sql.OpenAsync();
            await using var oReader    = await cmd.ExecuteReaderAsync();
            var             dd         = oReader.ToDynamicList();
            var             jsonString = dd.FirstOrDefault()!.ToJsonString();
            var             response   = JsonSerializer.Deserialize<InvoicesSpResponse>(jsonString);


            await sql.CloseAsync();

            return response;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }    
    
    public async Task<InvoicesSpResponse?> DebitEcoPoolTransactionSP(DebitTransactionRequest request)
    {
        try
        {
            await using var sql = new SqlConnection(_appSettings.ConnectionStrings?.SqlServerConnection);
            await using var cmd = new SqlCommand(Constants.DebitEcoPoolTransationSP, sql);
            
            CreateDebitEcoPoolListParameters(request, cmd);

            await sql.OpenAsync();
            await using var oReader    = await cmd.ExecuteReaderAsync();
            var             dd         = oReader.ToDynamicList();
            var             jsonString = dd.FirstOrDefault()!.ToJsonString();
            var             response   = JsonSerializer.Deserialize<InvoicesSpResponse>(jsonString);


            await sql.CloseAsync();

            return response;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
    public async Task<InvoicesSpResponse?> AdminDebitTransaction(DebitTransactionRequest request)
    {
        try
        {
            await using var sql = new SqlConnection(_appSettings.ConnectionStrings?.SqlServerConnection);
            await using var cmd = new SqlCommand(Constants.AdminDebitTransactionSp, sql);

            var invoicesDetails = ConvertToDataTable(request.invoices);

            CreateDebitListParameters(request, invoicesDetails, cmd);

            await sql.OpenAsync();
            await using var oReader    = await cmd.ExecuteReaderAsync();
            var             dd         = oReader.ToDynamicList();
            var             jsonString = dd.FirstOrDefault()!.ToJsonString();
            var             response   = JsonSerializer.Deserialize<InvoicesSpResponse>(jsonString);


            await sql.CloseAsync();

            return response;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
    public async Task<bool> CreditTransaction(CreditTransactionRequest request)
    {
        try
        {
            await using var sql = new SqlConnection(_appSettings.ConnectionStrings?.SqlServerConnection);
            await using var cmd = new SqlCommand(Constants.CreditTransationSP, sql);

            CreateCreditListParameters(request, cmd);

            await sql.OpenAsync();
            await cmd.ExecuteNonQueryAsync();
            await sql.CloseAsync();

            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return false;
        }
    }


    public async Task<bool> CreateEcoPoolSP(EcoPoolTransactionRequest request)
    {
        try
        {
            await using var sqlConnection = new SqlConnection(_appSettings.ConnectionStrings?.SqlServerConnection);

            await using var cmd = new SqlCommand(Constants.CreateEcoPoolSP, sqlConnection);

            var levelTypeDataTable   = ConvertToDataTable(request.LevelsType);
            var ecoPoolTypeDataTable = ConvertToDataTable(request.EcoPoolsType);

            CreateEcoPoolListParameters(request, levelTypeDataTable, ecoPoolTypeDataTable, cmd);

            await sqlConnection.OpenAsync();
            await using var oReader = await cmd.ExecuteReaderAsync();

            await sqlConnection.CloseAsync();

            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

        private void CreateDebitEcoPoolListParameters(DebitTransactionRequest request, SqlCommand cmd)
    {
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.Add(new SqlParameter("@AffiliateId", SqlDbType.Int)
        {
            Value = request.AffiliateId
        });

        cmd.Parameters.Add(new SqlParameter("@UserId", SqlDbType.Int)
        {
            Value = request.UserId
        });

        cmd.Parameters.Add(new SqlParameter("@Concept", SqlDbType.VarChar)
        {
            Value = request.Concept,
            Size  = 255
        });

        cmd.Parameters.Add(new SqlParameter("@Points", SqlDbType.Decimal)
        {
            Value = request.Points
        });

        cmd.Parameters.Add(new SqlParameter("@Commissionable", SqlDbType.Decimal)
        {
            Value = request.Commissionable
        });

        cmd.Parameters.Add(new SqlParameter("@Bank", SqlDbType.VarChar)
        {
            Value      = string.IsNullOrEmpty(request.Bank) ? null : request.Bank,
            IsNullable = true,
            Size       = 250
        });

        cmd.Parameters.Add(new SqlParameter("@PaymentMethod", SqlDbType.VarChar)
        {
            Value = request.PaymentMethod,
            Size  = 50
        });

        cmd.Parameters.Add(new SqlParameter("@Origin", SqlDbType.TinyInt)
        {
            Value = request.Origin
        });

        cmd.Parameters.Add(new SqlParameter("@Level", SqlDbType.Int)
        {
            Value = request.Level
        });

        cmd.Parameters.Add(new SqlParameter("@Debit", SqlDbType.Decimal)
        {
            Value = request.Debit
        });

        cmd.Parameters.Add(new SqlParameter("@AffiliateUserName", SqlDbType.VarChar)
        {
            Value = request.AffiliateUserName,
            Size  = 50
        });

        cmd.Parameters.Add(new SqlParameter("@AdminUserName", SqlDbType.VarChar)
        {
            Value = request.AdminUserName,
            Size  = 50
        });

        cmd.Parameters.Add(new SqlParameter("@ReceiptNumber", SqlDbType.VarChar)
        {
            Value      = string.IsNullOrEmpty(request.ReceiptNumber) ? null : request.ReceiptNumber,
            IsNullable = true,
            Size       = 100
        });

        cmd.Parameters.Add(new SqlParameter("@Type", SqlDbType.Bit)
        {
            Value = request.Type
        });

        cmd.Parameters.Add(new SqlParameter("@SecretKey", SqlDbType.VarChar)
        {
            Value      = string.IsNullOrEmpty(request.SecretKey) ? null : request.SecretKey,
            IsNullable = true,
            Size       = 40
        });

        cmd.Parameters.Add(new SqlParameter("@ConceptType", SqlDbType.VarChar)
        {
            Value = request.ConceptType,
            Size  = 50
        });
    }

    private void CreateDebitListParameters(DebitTransactionRequest request, DataTable dataTableDetails, SqlCommand cmd)
    {
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.Add(new SqlParameter("@AffiliateId", SqlDbType.Int)
        {
            Value = request.AffiliateId
        });

        cmd.Parameters.Add(new SqlParameter("@UserId", SqlDbType.Int)
        {
            Value = request.UserId
        });

        cmd.Parameters.Add(new SqlParameter("@Concept", SqlDbType.VarChar)
        {
            Value = request.Concept,
            Size  = 255
        });

        cmd.Parameters.Add(new SqlParameter("@Points", SqlDbType.Decimal)
        {
            Value = request.Points
        });

        cmd.Parameters.Add(new SqlParameter("@Commissionable", SqlDbType.Decimal)
        {
            Value = request.Commissionable
        });

        cmd.Parameters.Add(new SqlParameter("@Bank", SqlDbType.VarChar)
        {
            Value      = string.IsNullOrEmpty(request.Bank) ? null : request.Bank,
            IsNullable = true,
            Size       = 250
        });

        cmd.Parameters.Add(new SqlParameter("@PaymentMethod", SqlDbType.VarChar)
        {
            Value = request.PaymentMethod,
            Size  = 50
        });

        cmd.Parameters.Add(new SqlParameter("@Origin", SqlDbType.TinyInt)
        {
            Value = request.Origin
        });

        cmd.Parameters.Add(new SqlParameter("@Level", SqlDbType.Int)
        {
            Value = request.Level
        });

        cmd.Parameters.Add(new SqlParameter("@Debit", SqlDbType.Decimal)
        {
            Value = request.Debit
        });

        cmd.Parameters.Add(new SqlParameter("@AffiliateUserName", SqlDbType.VarChar)
        {
            Value = request.AffiliateUserName,
            Size  = 50
        });

        cmd.Parameters.Add(new SqlParameter("@AdminUserName", SqlDbType.VarChar)
        {
            Value = request.AdminUserName,
            Size  = 50
        });

        cmd.Parameters.Add(new SqlParameter("@ReceiptNumber", SqlDbType.VarChar)
        {
            Value      = string.IsNullOrEmpty(request.ReceiptNumber) ? null : request.ReceiptNumber,
            IsNullable = true,
            Size       = 100
        });

        cmd.Parameters.Add(new SqlParameter("@Type", SqlDbType.Bit)
        {
            Value = request.Type
        });

        cmd.Parameters.Add(new SqlParameter("@SecretKey", SqlDbType.VarChar)
        {
            Value      = string.IsNullOrEmpty(request.SecretKey) ? null : request.SecretKey,
            IsNullable = true,
            Size       = 40
        });

        cmd.Parameters.Add(new SqlParameter("@ConceptType", SqlDbType.VarChar)
        {
            Value = request.ConceptType,
            Size  = 50
        });

        cmd.Parameters.Add(new SqlParameter("@InvoicesDetails", SqlDbType.Structured)
        {
            Value    = dataTableDetails,
            TypeName = "dbo.InvoicesDetailsType"
        });
    }

    private void CreateEcoPoolListParameters(EcoPoolTransactionRequest request, DataTable levels, DataTable ecoPools, SqlCommand cmd)
    {
        cmd.CommandType = CommandType.StoredProcedure;

        cmd.Parameters.Add(new SqlParameter("@EcoPoolConfigurationId", SqlDbType.Int)
        {
            Value = request.EcoPoolConfigurationId
        });

        cmd.Parameters.Add(new SqlParameter("@TotalPercentageLevels", SqlDbType.Decimal)
        {
            Value = request.TotalPercentageLevels
        });

        cmd.Parameters.Add(new SqlParameter("@EcoPoolPercentage", SqlDbType.Decimal)
        {
            Value = request.EcoPoolPercentage
        });

        cmd.Parameters.Add(new SqlParameter("@CompanyPercentage", SqlDbType.Decimal)
        {
            Value = request.CompanyPercentage
        });

        cmd.Parameters.Add(new SqlParameter("@CompanyPercentageLevels", SqlDbType.Decimal)
        {
            Value = request.CompanyPercentageLevels
        });

        cmd.Parameters.Add(new SqlParameter("@Case", SqlDbType.Int)
        {
            Value = request.Case
        });

        cmd.Parameters.Add(new SqlParameter("@Points", SqlDbType.Int)
        {
            Value = request.Points
        });

        cmd.Parameters.Add(new SqlParameter("@Levels", SqlDbType.Structured)
        {
            Value    = levels,
            TypeName = "dbo.LevelsType"
        });

        cmd.Parameters.Add(new SqlParameter("@Pools", SqlDbType.Structured)
        {
            Value    = ecoPools,
            TypeName = "dbo.EcoPoolType"
        });
    }
    private void CreateCreditListParameters(CreditTransactionRequest request, SqlCommand cmd)
    {
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.Add(new SqlParameter("@AffiliateId", SqlDbType.Int)
        {
            Value = request.AffiliateId
        });

        cmd.Parameters.Add(new SqlParameter("@UserId", SqlDbType.Int)
        {
            Value = request.UserId
        });

        cmd.Parameters.Add(new SqlParameter("@Concept", SqlDbType.VarChar)
        {
            Value = request.Concept,
            Size  = 255
        });

        cmd.Parameters.Add(new SqlParameter("@Credit", SqlDbType.Decimal)
        {
            Value = request.Credit
        });

        cmd.Parameters.Add(new SqlParameter("@AffiliateUserName", SqlDbType.VarChar)
        {
            Value = request.AffiliateUserName,
            Size  = 50
        });

        cmd.Parameters.Add(new SqlParameter("@AdminUserName", SqlDbType.VarChar)
        {
            Value = request.AdminUserName,
            Size  = 50
        });

        cmd.Parameters.Add(new SqlParameter("@ConceptType", SqlDbType.VarChar)
        {
            Value = request.ConceptType,
            Size  = 50
        });
    }


    public Task<List<Wallets>> GetWalletByUserId(int userId)
        => Context.Wallets.Where(x => x.UserId == userId).ToListAsync();

    public Task<List<Wallets>> GetWalletsRequest(int userId)
        => Context.Wallets.AsNoTracking().Where(x
            => x.ConceptType == WalletConceptType.wallet_withdrawal_request.ToString()
               && x.AffiliateId == userId).ToListAsync();

    public Task<List<Wallets>> GetAllWallets()
        => Context.Wallets.AsNoTracking().ToListAsync();

    public Task<List<ModelFourStatistics>> GetUserModelFour(int[] affiliateIds)
    {
        return Context.ModelFourStatistics.AsNoTracking().Where(x 
            => affiliateIds.Contains(x.AffiliateId)).ToListAsync();
    }

    public Task<Wallets?> GetWalletById(int id)
        => Context.Wallets.FirstOrDefaultAsync(x => x.Id == id);

    public async Task<Wallets> CreateWalletAsync(Wallets request)
    {
        var today = DateTime.Now;
        request.CreatedAt = today;
        request.UpdatedAt = today;

        await Context.AddAsync(request);
        await Context.SaveChangesAsync();

        return request;
    }

    public async Task<Wallets> UpdateWalletAsync(Wallets request)
    {
        var today = DateTime.Now;
        request.UpdatedAt = today;
        Context.Wallets.Update(request);
        await Context.SaveChangesAsync();

        return request;
    }

    public async Task<Wallets> DeleteWalletAsync(Wallets request)
    {
        request.DeletedAt = DateTime.Now;

        Context.Wallets.Update(request);
        await Context.SaveChangesAsync();

        return request;
    }

    public Task<List<InvoicePacks>> GetDebitsEcoPoolWithinMonth(DateTime from, DateTime to)
    {
        return Context.InvoicePacks.Include(i => i.Invoice).AsNoTracking()
            .Where(x => x.Status == "1" && x.Invoice.Status == true && x.Invoice.CancellationDate == null && x.StartDate >= from && x.StartDate <= to)
            .ToListAsync();
    }

    public async Task<List<InvoicePacks>> GetDebitsEcoPoolOutsideMonth(DateTime date)
    {
        var list = await Context.InvoicePacks.Include(i => i.Invoice).AsNoTracking()
            .Where(x => x.StartDate < date && x.Status == "1" && x.Invoice.Status == true && x.Invoice.CancellationDate == null).ToListAsync();

        return list;
    }

    public async Task<bool> CreateTransferBalance(Wallets debitTransaction, Wallets creditTransaction)
    {
        await using var transaction = await Context.Database.BeginTransactionAsync();
        var             today       = DateTime.Now;
        try
        {
            debitTransaction.CreatedAt  = today;
            debitTransaction.UpdatedAt  = today;
            creditTransaction.CreatedAt = today;
            creditTransaction.UpdatedAt = today;
            Context.Wallets.Add(debitTransaction);
            Context.Wallets.Add(creditTransaction);

            await Context.SaveChangesAsync();

            await transaction.CommitAsync();
            return true;
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            return false;
        }
    }

    public async Task RevertDebitTransactionAsync(int id)
    {
        await using var sql = new SqlConnection(_appSettings.ConnectionStrings?.SqlServerConnection);
        await using var cmd = new SqlCommand(Constants.RevertDebitTransaction, sql);

        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.Add(new SqlParameter("@InvoiceNumber", SqlDbType.Int)
        {
            Value = id
        });

        await sql.OpenAsync();
        await cmd.ExecuteNonQueryAsync();
        await sql.CloseAsync();
    }

    public async Task<bool> IsActivePoolGreaterThanOrEqualTo25(int affiliateId)
    {
        var result = await Context.InvoicePacks.Include(x => x.Invoice)
            .Where(x => x.Status == "1" && x.Invoice.AffiliateId == affiliateId && x.Invoice.Status == true && x.Invoice.CancellationDate == null).ToListAsync();

        if (result is { Count: 0 })
            return false;

        return result.Sum(x => x.BaseAmount) >= 25;
    }

    public async Task<InvoicesSpResponse?> HandleMembershipTransaction(DebitTransactionRequest request)
    {
        try
        {
            await using var sql = new SqlConnection(_appSettings.ConnectionStrings?.SqlServerConnection);
            await using var cmd = new SqlCommand(Constants.HandleMembershipTransactions, sql);

            var invoicesDetails = ConvertToDataTable(request.invoices);

            CreateDebitListParameters(request, invoicesDetails, cmd);

            await sql.OpenAsync();
            await using var oReader    = await cmd.ExecuteReaderAsync();
            var             dd         = oReader.ToDynamicList();
            var             jsonString = dd.FirstOrDefault()!.ToJsonString();
            var             response   = JsonSerializer.Deserialize<InvoicesSpResponse>(jsonString);


            await sql.CloseAsync();

            return response;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task<InvoicesSpResponse?> MembershipDebitTransaction(DebitTransactionRequest request)
    {
        try
        {
            await using var sql = new SqlConnection(_appSettings.ConnectionStrings?.SqlServerConnection);
            await using var cmd = new SqlCommand(Constants.MembershipDebitTransactions, sql);

            var invoicesDetails = ConvertToDataTable(request.invoices);

            CreateDebitListParameters(request, invoicesDetails, cmd);

            await sql.OpenAsync();
            await using var oReader    = await cmd.ExecuteReaderAsync();
            var             dd         = oReader.ToDynamicList();
            var             jsonString = dd.FirstOrDefault()!.ToJsonString();
            var             response   = JsonSerializer.Deserialize<InvoicesSpResponse>(jsonString);


            await sql.CloseAsync();

            return response;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    private DataTable ConvertToWalletsDataTable(Wallets[] requests)
    {
        var today     = DateTime.Now;
        var dataTable = new DataTable();
        dataTable.Columns.Add("AffiliateId", typeof(int));
        dataTable.Columns.Add("UserId", typeof(int));
        dataTable.Columns.Add("Credit", typeof(decimal));
        dataTable.Columns.Add("Debit", typeof(decimal));
        dataTable.Columns.Add("Deferred", typeof(decimal));
        dataTable.Columns.Add("Status", typeof(bool));
        dataTable.Columns.Add("Concept", typeof(string));
        dataTable.Columns.Add("Support", typeof(int));
        dataTable.Columns.Add("Date", typeof(DateTime));
        dataTable.Columns.Add("Compression", typeof(bool));
        dataTable.Columns.Add("Detail", typeof(string));
        dataTable.Columns.Add("CreatedAt", typeof(DateTime));
        dataTable.Columns.Add("UpdatedAt", typeof(DateTime));
        dataTable.Columns.Add("DeletedAt", typeof(DateTime));
        dataTable.Columns.Add("AffiliateUserName", typeof(string));
        dataTable.Columns.Add("AdminUserName", typeof(string));
        dataTable.Columns.Add("ConceptType", typeof(string));

        foreach (var request in requests)
        {
            dataTable.Rows.Add(request.AffiliateId, request.UserId, 0, request.Debit, 0, true,
                request.Concept, null, today, false, "", today,
                today, null, request.AffiliateUserName, request.AdminUserName,
                request.ConceptType);
        }

        return dataTable;
    }
    public async Task<bool> BulkAdministrativeDebitTransaction(Wallets[] requests)
    {
        try
        {
            await using var sql = new SqlConnection(_appSettings.ConnectionStrings?.SqlServerConnection);
            await using var cmd = new SqlCommand(Constants.BulkAdministrativeDebitSp, sql);

            var walletsDataTable = ConvertToWalletsDataTable(requests);

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add(new SqlParameter("@WalletsData", SqlDbType.Structured)
            {
                Value    = walletsDataTable,
                TypeName = "dbo.WalletType"
            });
            
            SqlParameter successParameter = new SqlParameter("@Success", SqlDbType.Bit)
            {
                Direction = ParameterDirection.Output
            };
            cmd.Parameters.Add(successParameter);

            await sql.OpenAsync();
            await cmd.ExecuteNonQueryAsync();

            var isSuccess = (bool)successParameter.Value;  
        
            await sql.CloseAsync();

            return isSuccess;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public Task TransactionPoints(
        int affiliateId, decimal debitLeft, decimal debitRight, decimal creditLeft, decimal creditRight)
    {
        var debit = new ModelFourStatistics()
        {
            AffiliateId        = affiliateId,
            Concept            = string.Empty,
            AffiliateNetworkId = affiliateId,
            DebitLeft          = debitLeft,
            DebitRight         = debitRight,
            CreditLeft         = creditLeft,
            CreditRight        = creditRight,
            Compression        = false,
            Date               = DateTime.Now,
            InvoiceId          = 0
        };
        Context.ModelFourStatistics.Add(debit);
        return Context.SaveChangesAsync();
    }
}