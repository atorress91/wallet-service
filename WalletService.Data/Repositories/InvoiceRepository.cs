using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Data;
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

    public InvoiceRepository(IOptions<ApplicationConfiguration> appSettings, WalletServiceDbContext context) :
        base(context)
        => _appSettings = appSettings.Value;

    public Task<int> CountDetailsByPaymentGroup(int paymentGroupId, int userId, int brandId)
        => Context.InvoicesDetails.Where(x 
            => !x.Invoice.CancellationDate.HasValue && x.Invoice.Status &&
               x.Invoice.AffiliateId == userId && 
               x.PaymentGroupId == paymentGroupId && x.BrandId == brandId).CountAsync();
    
    public Task<int> CountDetailsModel3ByPaymentGroup(int userId, int brandId)
        => Context.InvoicesDetails.Where(x 
            => !x.Invoice.CancellationDate.HasValue && x.Invoice.Status &&
               x.Invoice.AffiliateId == userId && 
               new []{5,6}.Contains(x.PaymentGroupId) && x.BrandId == brandId).CountAsync();

    public Task<List<ModelFourStatistics>> Model4StatisticsByUser(int userId)
        => Context.ModelFourStatistics.Where(x => x.AffiliateId == userId).ToListAsync();

    public Task<List<Invoices>> GetAllInvoicesUser(int id, int brandId)
        => Context.Invoices.Include(x => x.InvoiceDetail).Where(x => x.AffiliateId == id && x.BrandId == brandId).AsNoTracking().ToListAsync();

    public Task<List<Invoices>> GetAllInvoices(int brandId)
        => Context.Invoices.Where(x=>x.BrandId==brandId).Include(x => x.InvoiceDetail).AsNoTracking().ToListAsync();

    public Task<Invoices?> GetInvoiceById(int id, int brandId)
        => Context.Invoices.Include(e=>e.InvoiceDetail).FirstOrDefaultAsync(x => x.Id == id && x.BrandId == brandId);

    public Task<Invoices?> GetInvoiceByReceiptNumber(string receiptNumber, int brandId)
        => Context.Invoices.Include(x=> x.InvoiceDetail).FirstOrDefaultAsync(e => e.ReceiptNumber == receiptNumber && e.BrandId == brandId);
    
    public Task<bool> InvoiceExistsByReceiptNumber(string idTransaction, int brandId)
        => Context.Invoices
            .AnyAsync(e => e.ReceiptNumber == idTransaction && e.BrandId == brandId);

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
    public async Task<List<Invoices>> DeleteMultipleInvoicesAndDetailsAsync(int[] invoiceIds, int brandId)
    {
        var today = DateTime.Now;
        
        var invoices = await Context.Invoices
                           .Include(i => i.InvoiceDetail)
                           .Where(i => invoiceIds.Contains(i.Id) && i.BrandId == brandId)
                           .ToListAsync();

        if (!invoices.Any())
        {
            return null;
        }

        foreach (var invoice in invoices)
        {
            invoice.DeletedAt        = today;
            invoice.UpdatedAt        = today;
            invoice.CancellationDate = today;
            invoice.Status           = false;
            
            foreach (var detail in invoice.InvoiceDetail)
            {
                detail.DeletedAt = today;
                detail.UpdatedAt = today;
            }
        }
        
        await Context.SaveChangesAsync();

        return invoices;
    }
    
    public async Task<InvoicesSpResponse?> HandleDebitTransaction(DebitTransactionRequest request)
    {
        try
        {
            await using var sql = new SqlConnection(_appSettings.ConnectionStrings?.SqlServerConnection);
            await using var cmd = new SqlCommand(Constants.HandleDebitTransactionSp, sql);

            var invoicesDetails = ConvertToDataTable(request.invoices);

            CreateDebitListParameters(request, invoicesDetails, cmd);

            await sql.OpenAsync();
            await using var oReader    = await cmd.ExecuteReaderAsync();
            var             dd         = oReader.ToDynamicList();
            var             jsonString = dd.FirstOrDefault()!.ToJsonString();
            var             response   = jsonString.ToJsonObject<InvoicesSpResponse>();


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
            var             response   = jsonString.ToJsonObject<InvoicesSpResponse>();


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
            TypeName = "dbo.InvoicesDetailsTypeWithBrand"
        });
        
        cmd.Parameters.Add(new SqlParameter("@Reason", SqlDbType.VarChar)
        {
            Value      = string.IsNullOrEmpty(request.Reason) ? null : request.Reason,
            IsNullable = true,
            Size       = 250
        });
        
        cmd.Parameters.Add(new SqlParameter("@BrandId", SqlDbType.Int)
        {
            Value = request.BrandId
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

        await Context.Database.ExecuteSqlRawAsync(
            "EXEC " + Constants.CoinPaymentRevertTransactions + " @InvoiceNumbers", param);
    }

    public Task<List<Invoices>> GetInvoicesByReceiptNumber(ICollection<string> transactionIds)
        => Context.Invoices.Where(e => e.ReceiptNumber != null && transactionIds.Contains(e.ReceiptNumber))
            .ToListAsync();

    public Task<bool> GetInvoicesForTradingAcademyPurchases(int affiliateId)
        => Context.Invoices
            .Include(x => x.InvoiceDetail)
            .AnyAsync(x => x.AffiliateId == affiliateId && x.InvoiceDetail.Any(d => d.PaymentGroupId == 6));

    public async Task<List<InvoicesTradingAcademyResponse>?> GetAllInvoicesForTradingAcademyPurchases()
    {
        await using var command = Context.Database.GetDbConnection().CreateCommand();
        command.CommandText = Constants.GetTradingAcademyDetailsSp;
        command.CommandType = CommandType.StoredProcedure;

        await Context.Database.OpenConnectionAsync();

        await using var result             = await command.ExecuteReaderAsync();
        var             invoiceDetailsList = new List<InvoicesTradingAcademyResponse>();

        while (await result.ReadAsync())
        {
            invoiceDetailsList.Add(new InvoicesTradingAcademyResponse
            {
                ProductId    = (int)result["ProductId"],
                UserName     = result["UserName"].ToString() ?? "",
                InvoiceId    = (int)result["InvoiceId"],
                ProductName  = result["ProductName"].ToString() ?? "",
                ProductPrice = (decimal)result["ProductPrice"],
                CreatedAt    = (DateTime)result["CreatedAt"]
            });
        }

        return invoiceDetailsList;
    }

    public async Task<List<InvoiceModelOneAndTwoResponse>?> GetAllInvoicesModelOneAndTwo()
    {
        await using var command = Context.Database.GetDbConnection().CreateCommand();
        command.CommandText = Constants.GetAllDetailsModelOneAndTwo;
        command.CommandType = CommandType.StoredProcedure;

        await Context.Database.OpenConnectionAsync();

        await using var result = await command.ExecuteReaderAsync();
        var             invoiceDetailsList = new List<InvoiceModelOneAndTwoResponse>();

        while (await result.ReadAsync())
        {
            invoiceDetailsList.Add(new InvoiceModelOneAndTwoResponse
            {
                UserName       = result["UserName"].ToString() ?? "",
                InvoiceId      = (int)result["InvoiceId"],
                ProductName    = result["ProductName"].ToString() ?? "",
                BaseAmount     = (decimal)result["BaseAmount"],
                PaymentGroupId = (int)result["PaymentGroupId"],
                CreatedAt      = (DateTime)result["CreatedAt"]
            });
        }

        return invoiceDetailsList;
    }
    
    public async Task<decimal> GetTotalRecyCoinSold()
    => await Context.InvoicesDetails.Where(x
            => !x.Invoice.CancellationDate.HasValue && x.Invoice.Status && x.PaymentGroupId == Constants.RecyCoinPaymentGroup)
        .SumAsync(x=>x.BaseAmount ?? 0);
}