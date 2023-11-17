using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Data;
using System.Linq;
using System.Text.Json;
using WalletService.Data.Database;
using WalletService.Data.Database.CustomModels;
using WalletService.Data.Database.Models;
using WalletService.Data.Repositories.IRepositories;
using WalletService.Models.Configuration;
using WalletService.Models.Constants;
using WalletService.Models.Requests.WalletRequest;
using WalletService.Utility.Extensions;
using static WalletService.Utility.Extensions.CommonExtensions;

namespace WalletService.Data.Repositories;

public class InvoiceRepository : BaseRepository, IInvoiceRepository
{
    private readonly ApplicationConfiguration _appSettings;
    public InvoiceRepository(IOptions<ApplicationConfiguration> appSettings, WalletServiceDbContext context) : base(context)
        => _appSettings = appSettings.Value;

    public Task<List<Invoices>> GetAllInvoicesUser(int id)
        => Context.Invoices.Include(x => x.InvoiceDetail).Where(x => x.AffiliateId == id).AsNoTracking().ToListAsync();

    public Task<List<Invoices>> GetAllInvoices()
        => Context.Invoices.Include(x => x.InvoiceDetail).AsNoTracking().ToListAsync();

    public Task<Invoices?> GetInvoiceById(int id)
        => Context.Invoices.FirstOrDefaultAsync(x => x.Id == id);
    public Task<Invoices?> GetInvoiceByReceiptNumber(string idTransaction)
        => Context.Invoices
                .FirstOrDefaultAsync(e => e.ReceiptNumber == idTransaction);
	public Task<bool> InvoiceExistsByReceiptNumber(string idTransaction)
	    => Context.Invoices
			.AnyAsync(e => e.ReceiptNumber == idTransaction);

	public async Task<Invoices> CreateInvoiceAsync(Invoices invoice)
    {
        var today = DateTime.Now;
        invoice.CreatedAt = today;
        invoice.UpdatedAt = today;

        await Context.AddAsync(invoice);
        await Context.SaveChangesAsync();

        return invoice;
    }

    public async Task<Invoices> DeleteInvoiceAsync(Invoices invoice)
    {
        var today = DateTime.Now;
        invoice.DeletedAt        = today;
        invoice.UpdatedAt        = today;
        invoice.CancellationDate = today;
        invoice.Status           = false;

        Context.Update(invoice);
        await Context.SaveChangesAsync();

        return invoice;
    }
    public async Task<InvoicesSpResponse?> HandleDebitTransaction(DebitTransactionRequest request)
    {
        try
        {
            await using var sql = new SqlConnection(_appSettings.ConnectionStrings?.SqlServerConnection);
            await using var cmd = new SqlCommand(Constants.HandleDebitTransationSP, sql);

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
    
    public async Task<InvoicesSpResponse?> HandleDebitTransactionForCourse(DebitTransactionRequest request)
    {
        try
        {
            await using var sql = new SqlConnection(_appSettings.ConnectionStrings?.SqlServerConnection);
            await using var cmd = new SqlCommand(Constants.HandleDebitTransactionCourse, sql);

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

    public async Task RevertCoinPaymentTransactions(List<InvoiceNumber> invoiceNumbers)
    {
        var table = new DataTable();
        table.Columns.Add("InvoiceNumber", typeof(int));
        foreach (var invoiceNumber in invoiceNumbers)
        {
            table.Rows.Add(invoiceNumber.InvoiceNumberValue);
        }

        var param = new SqlParameter("@InvoiceNumbers", SqlDbType.Structured)
        {
            TypeName = "dbo.InvoiceNumbersType",
            Value    = table
        };

        await Context.Database.ExecuteSqlRawAsync("EXEC " + Constants.CoinPaymentRevertTransactions + " @InvoiceNumbers", param);
    }

    public Task<List<Invoices>> GetInvoicesByReceiptNumber(ICollection<string> transactionIds)
        => Context.Invoices.Where(e => e.ReceiptNumber != null && transactionIds.Contains(e.ReceiptNumber))
                .ToListAsync();
}