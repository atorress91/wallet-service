using System.Data;
using System.Text.Json;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using WalletService.Data.Database;
using WalletService.Data.Database.CustomModels;
using WalletService.Data.Repositories.IRepositories;
using WalletService.Models.Configuration;
using WalletService.Models.Constants;
using WalletService.Models.Requests.WalletRequest;
using WalletService.Utility.Extensions;

namespace WalletService.Data.Repositories;

public class WalletModel1ARepository : BaseRepository, IWalletModel1ARepository
{

    private readonly ApplicationConfiguration _appSettings;

    public WalletModel1ARepository(IOptions<ApplicationConfiguration> appSettings, WalletServiceDbContext context) :
        base(context)
        => _appSettings = appSettings.Value;


    public async Task<bool> CreditTransaction(CreditTransactionRequest request)
    {
        try
        {
            await using var sql = new SqlConnection(_appSettings.ConnectionStrings?.SqlServerConnection);
            await using var cmd = new SqlCommand(Constants.CreditTransactionSp, sql);

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
            await using var cmd = new SqlCommand(Constants.DebitEcoPoolTransactionSpModel1A, sql);

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
            await using var cmd = new SqlCommand(Constants.DebitTransactionSpModel1A, sql);

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


}