using System.Data;
using System.Text.Json;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using WalletService.Data.Database;
using WalletService.Data.Database.CustomModels;
using WalletService.Data.Repositories.IRepositories;
using WalletService.Models.Configuration;
using WalletService.Models.Constants;
using WalletService.Models.Enums;
using WalletService.Models.Requests.WalletRequest;
using WalletService.Utility.Extensions;

namespace WalletService.Data.Repositories;

public class WalletModel1BRepository : BaseRepository, IWalletModel1BRepository
{
    private readonly ApplicationConfiguration _appSettings;

    public WalletModel1BRepository(IOptions<ApplicationConfiguration> appSettings, WalletServiceDbContext context) :
        base(context)
        => _appSettings = appSettings.Value;


    public async Task<bool> CreditTransaction(CreditTransactionRequest request)
    {
        try
        {
            await using var sql = new SqlConnection(_appSettings.ConnectionStrings?.SqlServerConnection);
            await using var cmd = new SqlCommand(Constants.CreditTransactionSpModel1B, sql);

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


    public async Task<InvoicesSpResponse?> DebitEcoPoolTransactionSp(DebitTransactionRequest request)
    {
        try
        {
            await using var sql = new SqlConnection(_appSettings.ConnectionStrings?.SqlServerConnection);
            await using var cmd = new SqlCommand(Constants.DebitEcoPoolTransactionSpModel1B, sql);

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

    public async Task<InvoicesSpResponse?> DebitTransaction(DebitTransactionRequest request)
    {
        try
        {
            await using var sql = new SqlConnection(_appSettings.ConnectionStrings?.SqlServerConnection);
            await using var cmd = new SqlCommand(Constants.DebitTransactionSpModel1B, sql);

            var invoicesDetails = CommonExtensions.ConvertToDataTable(request.invoices);

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

    public async Task<decimal> GetAvailableBalanceByAffiliateId(int affiliateId)
    {
        var list = await Context.WalletsModel1B
                       .Where(x => x.AffiliateId == affiliateId && x.Status == true).ToListAsync();

        var result = list.Sum(x => x.Credit - x.Debit);
        return result.ToDecimal();
    }

    public Task<decimal?> GetTotalAcquisitionsByAffiliateId(int affiliateId)
        => Context.InvoicesDetails.Include(x => x.Invoice).AsNoTracking()
            .Where(x => x.Invoice.AffiliateId == affiliateId && x.PaymentGroupId == 8)
            .SumAsync(s => s.BaseAmount);
    
    public async Task<decimal?> GetReverseBalanceByAffiliateId(int affiliateId)
    {
        var totalCredits = await Context.WalletsModel1B
                               .Where(x => x.AffiliateId == affiliateId && x.ConceptType == WalletConceptType.revert_pool.ToString())
                               .Select(x => x.Credit)
                               .SumAsync();

        var totalDebits = await Context.WalletsModel1B
                              .Where(x => x.AffiliateId == affiliateId &&
                                          x.ConceptType == WalletConceptType.purchase_with_reverse_balance.ToString())
                              .Select(x => x.Debit)
                              .SumAsync();

        var reverseBalance = totalCredits - totalDebits;
        return Convert.ToDecimal(reverseBalance);
    }
    
    public Task<double?> GetTotalServiceBalance(int affiliateId)
    {
        return Context.WalletsServiceModel1B
            .Where(x => x.AffiliateId == affiliateId)
            .Select(s => s.Credit - s.Debit)
            .SumAsync();
    }

    public async Task<InvoicesSpResponse?> DebitServiceBalanceTransaction(DebitTransactionRequest request)
    {
        try
        {
            await using var sql = new SqlConnection(_appSettings.ConnectionStrings?.SqlServerConnection);
            await using var cmd = new SqlCommand(Constants.DebitEcoPoolTransactionServiceSpModel1B, sql);

            var invoicesDetails = CommonExtensions.ConvertToDataTable(request.invoices);

            CreateDebitServiceBalanceListParameters(request, invoicesDetails, cmd);

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
    
    private void CreateDebitServiceBalanceListParameters(DebitTransactionRequest request, DataTable dataTableDetails, SqlCommand cmd)
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

        cmd.Parameters.Add(new SqlParameter("@Debit", SqlDbType.Decimal)
        {
            Value = request.Debit
        });

        cmd.Parameters.Add(new SqlParameter("@AffiliateUserName", SqlDbType.VarChar)
        {
            Value = request.AffiliateUserName,
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
        
        cmd.Parameters.Add(new SqlParameter("@InvoicesDetails", SqlDbType.Structured)
        {
            Value    = dataTableDetails,
            TypeName = "dbo.InvoicesDetailsType"
        });
    }

    
    public Task<InvoicesSpResponse?> DebitServiceBalanceEcoPoolTransactionSp(DebitTransactionRequest request)
    {
        throw new NotImplementedException();
    }

    public Task<bool> CreditServiceBalanceTransaction(CreditTransactionRequest                             request)
    {
        throw new NotImplementedException();
    }
}