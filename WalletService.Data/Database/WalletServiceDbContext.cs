using Microsoft.EntityFrameworkCore;
using WalletService.Data.Database.CustomModels;
using WalletService.Data.Database.Models;

namespace WalletService.Data.Database;

public class WalletServiceDbContext : DbContext
{
    public WalletServiceDbContext()
    {
    }

    public WalletServiceDbContext(DbContextOptions<WalletServiceDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Wallets> Wallets { get; set; }
    public virtual DbSet<WalletsModel1A> WalletsModel1A { get; set; }
    public virtual DbSet<WalletsModel1B> WalletsModel1B { get; set; }
    public virtual DbSet<WalletsHistories> WalletsHistories { get; set; }
    public virtual DbSet<WalletsPeriods> WalletsPeriods { get; set; }
    public virtual DbSet<WalletsRequests> WalletsRequests { get; set; }
    public virtual DbSet<WalletsRetentionsConfigs> WalletsRetentionsConfigs { get; set; }
    public virtual DbSet<WalletsWaits> WalletsWaits { get; set; }
    public virtual DbSet<WalletsWithdrawals> WalletsWithdrawals { get; set; }
    public virtual DbSet<Invoices> Invoices { get; set; }
    public virtual DbSet<InvoicesDetails> InvoicesDetails { get; set; }
    public virtual DbSet<NetworkPurchases> NetworkPurchases { get; set; }
    public virtual DbSet<ModelConfiguration> ModelConfiguration { get; set; }
    public virtual DbSet<ModelConfigurationLevels> ModelConfigurationLevels { get; set; }
    public virtual DbSet<ResultsModel2> ResultsModel2 { get; set; }

    public virtual DbSet<ResultsModel2Levels> ResultsModel2Levels { get; set; }

    public virtual DbSet<ResultsModel1A> ResultsModel1A { get; set; }

    public virtual DbSet<ResultsModel1ALevels> ResultsModel1ALevels { get; set; }

    public virtual DbSet<ResultsModel1B> ResultsModel1B { get; set; }

    public virtual DbSet<ResultsModel1BLevels> ResultsModel1BLevels { get; set; }
    public virtual DbSet<ApiClient> ApiClient { get; set; }
    public virtual DbSet<PaymentTransaction> PaymentTransaction { get; set; }
    public virtual DbSet<ModelFourStatistics> ModelFourStatistics { get; set; }
    public virtual DbSet<Commissions> Commissions { get; set; }
    public virtual DbSet<ResultsModel3> ResultsModel3 { get; set; }
    public virtual DbSet<ResultsModel3Levels> ResultsModel3Levels { get; set; }
    public virtual DbSet<WalletsServiceModel1A> WalletsServiceModel1A { get; set; }
    public virtual DbSet<WalletsServiceModel1B> WalletsServiceModel1B { get; set; }
    public virtual DbSet<WalletsServiceModel2> WalletsServiceModel2 { get; set; }
    public virtual DbSet<Brand> Brand { get; set; }
    public virtual DbSet<Bonuses> Bonuses { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<InvoicesSpResponse>(entity => entity.HasNoKey());
        modelBuilder.Entity<PurchasesPerMonth>(entity => entity.HasNoKey());
        modelBuilder.Entity<EcoPoolesSpResponse>(entity => entity.HasNoKey());

        modelBuilder.Entity<Wallets>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.AffiliateId)
                .HasColumnType("int")
                .IsRequired();

            entity.HasIndex(e => e.AffiliateId, "index_affiliate_id");
            entity.HasIndex(e => e.ConceptType, "index_concept_type");
            entity.HasIndex(e => e.UserId, "index_user_id");

            entity.Property(e => e.UserId)
                .HasColumnType("int");

            entity.Property(e => e.Credit)
                .HasColumnType("decimal(10,5)")
                .HasDefaultValue(0.0m);

            entity.Property(e => e.Debit)
                .HasColumnType("decimal(10,5)")
                .HasDefaultValue(0.0m);

            entity.Property(e => e.Deferred)
                .HasColumnType("decimal");

            entity.Property(e => e.Status).HasColumnType("bit");

            entity.Property(e => e.Concept)
                .HasColumnType("varchar(255)")
                .HasMaxLength(255)
                .IsRequired();

            entity.Property(e => e.ConceptType)
                .HasColumnType("varchar(50)")
                .HasMaxLength(50);

            entity.Property(e => e.AffiliateUserName)
                .HasColumnType("varchar(50)")
                .HasMaxLength(50);

            entity.Property(e => e.AdminUserName)
                .HasColumnType("varchar(50)")
                .HasMaxLength(50);

            entity.Property(e => e.Support)
                .HasColumnType("int");

            entity.Property(e => e.Date)
                .HasColumnType("datetime")
                .IsRequired();

            entity.Property(e => e.Compression).HasColumnType("bit");

            entity.Property(e => e.Detail)
                .HasColumnType("text");

            entity.Property(e => e.CreatedAt).IsRequired().HasColumnType("datetime");
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
            entity.Property(e => e.DeletedAt).HasColumnType("datetime");
            entity.Property(e => e.BrandId).IsRequired();

            entity.HasQueryFilter(e => !e.DeletedAt.HasValue);
        });

        modelBuilder.Entity<WalletsModel1A>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.AffiliateId)
                .HasColumnType("int")
                .IsRequired();

            entity.HasIndex(e => e.AffiliateId, "index_affiliate_id");
            entity.HasIndex(e => e.ConceptType, "index_concept_type");
            entity.HasIndex(e => e.UserId, "index_user_id");

            entity.Property(e => e.UserId)
                .HasColumnType("int");

            entity.Property(e => e.Credit)
                .HasColumnType("decimal(10,5)")
                .HasDefaultValue(0.0m);

            entity.Property(e => e.Debit)
                .HasColumnType("decimal(10,5)")
                .HasDefaultValue(0.0m);

            entity.Property(e => e.Deferred)
                .HasColumnType("decimal");

            entity.Property(e => e.Status).HasColumnType("bit");


            entity.Property(e => e.Concept)
                .HasColumnType("varchar(255)")
                .HasMaxLength(255)
                .IsRequired();

            entity.Property(e => e.ConceptType)
                .HasColumnType("varchar(50)")
                .HasMaxLength(50);

            entity.Property(e => e.AffiliateUserName)
                .HasColumnType("varchar(50)")
                .HasMaxLength(50);


            entity.Property(e => e.AdminUserName)
                .HasColumnType("varchar(50)")
                .HasMaxLength(50);


            entity.Property(e => e.Support)
                .HasColumnType("int");

            entity.Property(e => e.Date)
                .HasColumnType("datetime")
                .IsRequired();

            entity.Property(e => e.Compression).HasColumnType("bit");

            entity.Property(e => e.Detail)
                .HasColumnType("text");

            entity.Property(e => e.CreatedAt).IsRequired().HasColumnType("datetime");
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
            entity.Property(e => e.DeletedAt).HasColumnType("datetime");

            entity.HasQueryFilter(e => !e.DeletedAt.HasValue);
        });
        modelBuilder.Entity<WalletsModel1B>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.AffiliateId)
                .HasColumnType("int")
                .IsRequired();

            entity.HasIndex(e => e.AffiliateId, "index_affiliate_id");
            entity.HasIndex(e => e.ConceptType, "index_concept_type");
            entity.HasIndex(e => e.UserId, "index_user_id");

            entity.Property(e => e.UserId)
                .HasColumnType("int");

            entity.Property(e => e.Credit)
                .HasColumnType("decimal(10,5)")
                .HasDefaultValue(0.0m);

            entity.Property(e => e.Debit)
                .HasColumnType("decimal(10,5)")
                .HasDefaultValue(0.0m);

            entity.Property(e => e.Deferred)
                .HasColumnType("decimal");

            entity.Property(e => e.Status).HasColumnType("bit");

            entity.Property(e => e.Concept)
                .HasColumnType("varchar(255)")
                .HasMaxLength(255)
                .IsRequired();

            entity.Property(e => e.ConceptType)
                .HasColumnType("varchar(50)")
                .HasMaxLength(50);

            entity.Property(e => e.AffiliateUserName)
                .HasColumnType("varchar(50)")
                .HasMaxLength(50);


            entity.Property(e => e.AdminUserName)
                .HasColumnType("varchar(50)")
                .HasMaxLength(50);


            entity.Property(e => e.Support)
                .HasColumnType("int");

            entity.Property(e => e.Date)
                .HasColumnType("datetime")
                .IsRequired();

            entity.Property(e => e.Compression).HasColumnType("bit");
            entity.Property(e => e.Detail)
                .HasColumnType("text");

            entity.Property(e => e.CreatedAt).IsRequired().HasColumnType("datetime");
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
            entity.Property(e => e.DeletedAt).HasColumnType("datetime");

            entity.HasQueryFilter(e => !e.DeletedAt.HasValue);
        });

        modelBuilder.Entity<WalletsHistories>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id)
                .HasColumnType("int")
                .IsRequired();

            entity.Property(e => e.AffiliateId)
                .HasColumnType("int")
                .IsRequired();

            entity.HasIndex(e => e.AffiliateId, "index_affiliate_id");

            entity.Property(e => e.UserId)
                .HasColumnType("int");

            entity.HasIndex(e => e.UserId, "index_user_id");

            entity.Property(e => e.Credit)
                .HasColumnType("decimal")
                .HasDefaultValue(0.00m);

            entity.Property(e => e.Debit)
                .HasColumnType("decimal")
                .HasDefaultValue(0.00m);

            entity.Property(e => e.Deferred)
                .HasColumnType("decimal");

            entity.Property(e => e.Status)
                .HasColumnType("bool")
                .HasDefaultValueSql("'1'");

            entity.Property(e => e.Concept)
                .HasColumnType("varchar(255)")
                .HasMaxLength(255)
                .IsRequired();

            entity.Property(e => e.Support)
                .HasColumnType("int");

            entity.Property(e => e.Date)
                .HasColumnType("datetime")
                .IsRequired();

            entity.Property(e => e.Compression)
                .HasColumnType("bool")
                .HasDefaultValueSql("'0'");

            entity.Property(e => e.CreatedAt).IsRequired().HasColumnType("datetime");
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
            entity.Property(e => e.DeletedAt).HasColumnType("datetime");

            entity.HasQueryFilter(e => !e.DeletedAt.HasValue);
        });

        modelBuilder.Entity<WalletsPeriods>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id)
                .HasColumnType("int")
                .IsRequired();

            entity.Property(e => e.Date)
                .IsRequired();

            entity.Property(e => e.Status)
                .HasColumnType("bool")
                .IsRequired();

            entity.Property(e => e.CreatedAt).IsRequired().HasColumnType("datetime");
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
            entity.Property(e => e.DeletedAt).HasColumnType("datetime");

            entity.HasQueryFilter(e => !e.DeletedAt.HasValue);
        });

        modelBuilder.Entity<WalletsRequests>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnType("int").IsRequired();
            entity.Property(e => e.AffiliateId)
                .HasColumnType("int")
                .IsRequired();

            entity.HasIndex(e => e.AffiliateId, "index_affiliate_id");

            entity.Property(e => e.PaymentAffiliateId)
                .HasColumnType("int");

            entity.HasIndex(e => e.PaymentAffiliateId, "index_payment_affiliate_id");
            entity.Property(e => e.OrderNumber).HasColumnType("varchar(25)").IsRequired();
            entity.Property(e => e.AdminUserName).HasColumnType("varchar(1h50)");
            entity.Property(e => e.Type).IsRequired().HasColumnType("varchar(50)");
            entity.Property(e => e.InvoiceNumber).IsRequired().HasColumnType("int");
            entity.Property(e => e.Amount).HasColumnType("decimal").IsRequired();
            entity.Property(e => e.Concept).HasColumnType("varchar(255)");
            entity.Property(e => e.CreationDate).IsRequired();
            entity.Property(e => e.BrandId).IsRequired();
            entity.Property(e => e.Status).IsRequired();
            entity.Property(e => e.AttentionDate).HasColumnType("datetime");
            entity.Property(e => e.CreatedAt).IsRequired().HasColumnType("datetime");
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
            entity.Property(e => e.DeletedAt).HasColumnType("datetime");

            entity.HasQueryFilter(e => !e.DeletedAt.HasValue);
        });

        modelBuilder.Entity<WalletsRetentionsConfigs>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id)
                .IsRequired();

            entity.Property(e => e.WithdrawalFrom)
                .HasColumnType("decimal")
                .IsRequired();

            entity.Property(e => e.WithdrawalTo)
                .HasColumnType("decimal")
                .IsRequired();

            entity.Property(e => e.Percentage)
                .HasColumnType("decimal")
                .IsRequired();

            entity.Property(e => e.Date)
                .HasColumnType("datetime")
                .IsRequired();

            entity.Property(e => e.DisableDate)
                .HasColumnType("datetime");

            entity.Property(e => e.Status).HasColumnType("bool").IsRequired().HasDefaultValueSql("'0'");

            entity.Property(e => e.CreatedAt).IsRequired().HasColumnType("datetime");
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
            entity.Property(e => e.DeletedAt).HasColumnType("datetime");

            entity.HasQueryFilter(e => !e.DeletedAt.HasValue);
        });

        modelBuilder.Entity<WalletsWaits>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id);

            entity.Property(e => e.AffiliateId).IsRequired();
            entity.HasIndex(e => e.AffiliateId, "index_affiliate_id");
            entity.Property(e => e.Credit)
                .HasColumnType("decimal");

            entity.Property(e => e.PaymentMethod)
                .HasColumnType("varchar(255)")
                .HasMaxLength(255)
                .IsRequired();

            entity.Property(e => e.Bank)
                .HasMaxLength(255)
                .HasColumnType("varchar(255)");

            entity.Property(e => e.Support)
                .HasColumnType("varchar(255)")
                .HasMaxLength(255);

            entity.Property(e => e.DepositDate)
                .HasColumnType("datetime");

            entity.Property(e => e.Status)
                .HasColumnType("bool")
                .HasDefaultValueSql("false");

            entity.Property(e => e.Attended)
                .HasColumnType("bool")
                .HasDefaultValueSql("false");

            entity.Property(e => e.Date)
                .HasColumnType("datetime")
                .IsRequired();

            entity.Property(e => e.Order)
                .HasColumnType("varchar(100)");

            entity.Property(e => e.CreatedAt).IsRequired().HasColumnType("datetime");
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
            entity.Property(e => e.DeletedAt).HasColumnType("datetime");

            entity.HasQueryFilter(e => !e.DeletedAt.HasValue);
        });

        modelBuilder.Entity<WalletsWithdrawals>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id);

            entity.Property(e => e.AffiliateId)
                .IsRequired();

            entity.Property(e => e.Amount)
                .HasColumnType("decimal")
                .IsRequired();

            entity.Property(e => e.Status)
                .HasColumnType("bool")
                .IsRequired();

            entity.Property(e => e.IsProcessed)
                .HasColumnType("bool")
                .IsRequired();

            entity.Property(e => e.Observation)
                .HasColumnType("varchar(255)").HasMaxLength(255);

            entity.Property(e => e.AdminObservation)
                .HasColumnType("varchar(255)").HasMaxLength(255);

            entity.Property(e => e.Date)
                .HasColumnType("datetime")
                .IsRequired();

            entity.Property(e => e.ResponseDate)
                .HasColumnType("datetime");

            entity.Property(e => e.RetentionPercentage)
                .HasColumnType("decimal")
                .IsRequired();

            entity.Property(e => e.CreatedAt).IsRequired().HasColumnType("datetime");
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
            entity.Property(e => e.DeletedAt).HasColumnType("datetime");

            entity.HasQueryFilter(e => !e.DeletedAt.HasValue);
        });

        modelBuilder.Entity<Invoices>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.InvoiceNumber).IsRequired();
            entity.Property(e => e.PurchaseOrderId);
            entity.HasIndex(e => e.PurchaseOrderId, "index_purchase_order_id");
            entity.Property(e => e.Recurring);
            entity.Property(e => e.AffiliateId).IsRequired();
            entity.HasIndex(e => e.AffiliateId, "index_affiliate_id");
            entity.Property(e => e.TotalInvoice).HasColumnType("decimal(10,2)");
            entity.Property(e => e.TotalInvoiceBtc).HasColumnType("decimal(16,8)");
            entity.Property(e => e.TotalCommissionable).HasColumnType("decimal(10,2)");

            entity.Property(e => e.TotalPoints);
            entity.Property(e => e.State).HasColumnType("bit");
            entity.Property(e => e.Type).HasColumnType("bit");
            entity.Property(e => e.Status).HasColumnType("bit");
            entity.Property(e => e.Date).HasColumnType("datetime");
            entity.Property(e => e.CancellationDate).HasColumnType("datetime");
            entity.Property(e => e.Bank).HasMaxLength(250);
            entity.Property(e => e.ReceiptNumber).HasMaxLength(100);
            entity.Property(e => e.PaymentMethod).HasMaxLength(50);
            entity.Property(e => e.Reason).HasMaxLength(250);
            entity.Property(e => e.InvoiceData).HasMaxLength(250);
            entity.Property(e => e.InvoiceAddress).HasMaxLength(250);
            entity.Property(e => e.ShippingAddress).HasMaxLength(250);
            entity.Property(e => e.SecretKey).HasMaxLength(40);
            entity.Property(e => e.BtcAddress).HasMaxLength(100);
            entity.Property(e => e.BrandId).IsRequired();

            entity.Property(e => e.DepositDate).HasColumnType("datetime");
            entity.Property(e => e.CreatedAt).IsRequired().HasColumnType("datetime");
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
            entity.Property(e => e.DeletedAt).HasColumnType("datetime");

            entity.HasQueryFilter(e => !e.DeletedAt.HasValue);
        });

        modelBuilder.Entity<InvoicesDetails>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.InvoiceId).IsRequired();
            entity.HasIndex(e => e.InvoiceId, "index_invoice_id");
            entity.Property(e => e.ProductId).IsRequired();
            entity.HasIndex(e => e.ProductId, "index_product_id");

            entity.Property(e => e.PaymentGroupId).IsRequired();
            entity.HasIndex(e => e.PaymentGroupId, "index_payment_group_id");
            entity.Property(e => e.AccumMinPurchase).IsRequired();
            entity.Property(e => e.ProductName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.ProductPrice).IsRequired().HasColumnType("decimal(10,2)");
            entity.Property(e => e.ProductPriceBtc).HasColumnType("decimal(10,8)");
            entity.Property(e => e.ProductIva).HasColumnType("decimal(10,2)");
            entity.Property(e => e.ProductQuantity).IsRequired();
            entity.Property(e => e.ProductCommissionable).HasColumnType("decimal(10,2)");
            entity.Property(e => e.BinaryPoints).HasColumnType("decimal(10,2)");
            entity.Property(e => e.ProductDiscount).HasColumnType("decimal(10,2)");
            entity.Property(e => e.ProductPoints);
            entity.Property(e => e.CombinationId);
            entity.HasIndex(e => e.CombinationId, "index_combination_id");
            entity.Property(e => e.WaitingDays);
            entity.Property(e => e.DaysToPayQuantity);
            entity.Property(e => e.ProductPack).HasColumnType("bit");
            entity.Property(e => e.BrandId).IsRequired();
            entity.Property(e => e.ProductStart).HasColumnType("bit");

            entity.Property(e => e.Date).HasColumnType("datetime").IsRequired();
            entity.Property(e => e.BaseAmount).HasColumnType("decimal(10,2)");
            entity.Property(e => e.DailyPercentage).HasColumnType("decimal(10,2)");
            entity.Property(e => e.CreatedAt).IsRequired().HasColumnType("datetime");
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
            entity.Property(e => e.DeletedAt).HasColumnType("datetime");

            entity.HasOne(d => d.Invoice)
                .WithMany(p => p.InvoiceDetail)
                .HasForeignKey(d => d.InvoiceId);

            entity.HasQueryFilter(e => !e.DeletedAt.HasValue);
        });

        modelBuilder.Entity<NetworkPurchases>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Level).IsRequired();
            entity.Property(e => e.CommisionableAmount).IsRequired().HasColumnType("decimal(10,2)");
            entity.Property(e => e.Points).IsRequired().HasColumnType("decimal(10,2)");
            entity.Property(e => e.Origin).HasColumnType("tinyint");
            entity.Property(e => e.CreatedAt).IsRequired().HasColumnType("datetime");
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
            entity.Property(e => e.DeletedAt).HasColumnType("datetime");

            entity.HasQueryFilter(e => !e.DeletedAt.HasValue);
        });

        modelBuilder.Entity<ModelConfiguration>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.CompanyPercentage).IsRequired().HasColumnType("decimal(10,2)");
            entity.Property(e => e.ModelPercentage).IsRequired().HasColumnType("decimal(10,2)");
            entity.Property(e => e.CompanyPercentageLevels).IsRequired().HasColumnType("decimal(10,2)");
            entity.Property(e => e.MaxGainLimit).IsRequired().HasColumnType("decimal(10,2)");
            entity.Property(e => e.DateInit).IsRequired().HasColumnType("datetime");
            entity.Property(e => e.DateEnd).IsRequired().HasColumnType("datetime");
            entity.Property(e => e.Case).IsRequired();
            entity.Property(e => e.ModelType).IsRequired();
            entity.Property(e => e.CompletedAt).HasColumnType("datetime");
            entity.Property(e => e.CreatedAt).IsRequired().HasColumnType("datetime");
            entity.Property(e => e.UpdatedAt).IsRequired().HasColumnType("datetime");
            entity.Property(e => e.DeletedAt).HasColumnType("datetime");
            entity.Property(e => e.Processed);
            entity.Property(e => e.Totals);

            entity.HasQueryFilter(e => !e.DeletedAt.HasValue);
        });

        modelBuilder.Entity<ModelConfigurationLevels>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.EcoPoolConfigurationId).IsRequired();
            entity.Property(e => e.Level).IsRequired();
            entity.Property(e => e.Percentage).IsRequired().HasColumnType("decimal(10,2)");
            entity.Property(e => e.CreatedAt).IsRequired().HasColumnType("datetime");
            entity.Property(e => e.UpdatedAt).IsRequired().HasColumnType("datetime");

            entity.HasOne(d => d.ModelConfiguration)
                .WithMany(p => p.ModelConfigurationLevels)
                .HasForeignKey(d => d.EcoPoolConfigurationId);
        });

        modelBuilder.Entity<ResultsModel1A>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ProductExternalId).IsRequired();
            entity.Property(e => e.AffiliateId).IsRequired();
            entity.Property(e => e.AffiliateName).IsRequired().HasMaxLength(50).HasColumnType("varchar(50)");
            entity.Property(e => e.ProductName).IsRequired().HasMaxLength(50).HasColumnType("varchar(50)");
            entity.Property(e => e.BaseAmount).IsRequired().HasColumnType("decimal(10,5)");
            entity.Property(e => e.ProfitDistributedLevels).IsRequired().HasColumnType("decimal(10,5)");
            entity.Property(e => e.TotalPercentage).IsRequired().HasColumnType("decimal(10,2)");
            entity.Property(e => e.PaymentAmount).IsRequired().HasColumnType("decimal(10,5)");
            entity.Property(e => e.Points).IsRequired().HasMaxLength(50).HasColumnType("varchar(50)");
            entity.Property(e => e.PeriodPool).IsRequired().HasColumnType("datetime");
            entity.Property(e => e.CompletedAt).HasColumnType("datetime");
            entity.Property(e => e.UserCreatedAt).HasColumnType("datetime");
        });


        modelBuilder.Entity<ResultsModel1ALevels>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ResultsModel1AId).IsRequired();
            entity.Property(e => e.AffiliateId).IsRequired();
            entity.Property(e => e.AffiliateName).IsRequired().HasMaxLength(50).HasColumnType("varchar(50)");
            entity.Property(e => e.Level).IsRequired();
            entity.Property(e => e.PercentageLevel).IsRequired().HasColumnType("decimal(10,2)");
            entity.Property(e => e.PaymentAmount).IsRequired().HasColumnType("decimal(10,5)");
            entity.Property(e => e.Points).HasColumnType("decimal(10,5)");
            entity.Property(e => e.CompletedAt).HasColumnType("datetime");
            entity.Property(e => e.BinarySide).IsRequired().HasColumnType("int");
            entity.Property(e => e.UserCreatedAt).HasColumnType("datetime");

            entity.HasOne(d => d.ResultsModel1A)
                .WithMany(p => p.ResultsModel1ALevels)
                .HasForeignKey(d => d.ResultsModel1AId);
        });

        modelBuilder.Entity<ResultsModel1B>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ProductExternalId).IsRequired();
            entity.Property(e => e.AffiliateId).IsRequired();
            entity.Property(e => e.AffiliateName).IsRequired().HasMaxLength(50).HasColumnType("varchar(50)");
            entity.Property(e => e.ProductName).IsRequired().HasMaxLength(50).HasColumnType("varchar(50)");
            entity.Property(e => e.BaseAmount).IsRequired().HasColumnType("decimal(10,5)");
            entity.Property(e => e.ProfitDistributedLevels).IsRequired().HasColumnType("decimal(10,5)");
            entity.Property(e => e.TotalPercentage).IsRequired().HasColumnType("decimal(10,2)");
            entity.Property(e => e.PaymentAmount).IsRequired().HasColumnType("decimal(10,5)");
            entity.Property(e => e.Points).IsRequired().HasMaxLength(50).HasColumnType("varchar(50)");
            entity.Property(e => e.PeriodPool).IsRequired().HasColumnType("datetime");
            entity.Property(e => e.CompletedAt).HasColumnType("datetime");
            entity.Property(e => e.UserCreatedAt).HasColumnType("datetime");
        });


        modelBuilder.Entity<ResultsModel1BLevels>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ResultsModel1BId).IsRequired();
            entity.Property(e => e.AffiliateId).IsRequired();
            entity.Property(e => e.AffiliateName).IsRequired().HasMaxLength(50).HasColumnType("varchar(50)");
            entity.Property(e => e.Level).IsRequired();
            entity.Property(e => e.PercentageLevel).IsRequired().HasColumnType("decimal(10,2)");
            entity.Property(e => e.PaymentAmount).IsRequired().HasColumnType("decimal(10,5)");
            entity.Property(e => e.Points).HasColumnType("decimal(10,5)");
            entity.Property(e => e.CompletedAt).HasColumnType("datetime");
            entity.Property(e => e.BinarySide).IsRequired().HasColumnType("int");
            entity.Property(e => e.UserCreatedAt).HasColumnType("datetime");

            entity.HasOne(d => d.ResultsModel1B)
                .WithMany(p => p.ResultsModel1BLevels)
                .HasForeignKey(d => d.ResultsModel1BId);
        });


        modelBuilder.Entity<ResultsModel2>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.EcoPoolConfigurationId).IsRequired();
            entity.Property(e => e.ProductExternalId).IsRequired();
            entity.Property(e => e.AffiliateId).IsRequired();
            entity.Property(e => e.AffiliateName).IsRequired().HasMaxLength(50).HasColumnType("varchar(50)");
            entity.Property(e => e.ProductName).IsRequired().HasMaxLength(50).HasColumnType("varchar(50)");
            entity.Property(e => e.PaymentDate).IsRequired().HasColumnType("datetime");
            entity.Property(e => e.LastDaydate).IsRequired().HasColumnType("datetime");
            entity.Property(e => e.DailyPercentage).IsRequired().HasColumnType("decimal(10,2)");
            entity.Property(e => e.BasePack).IsRequired().HasColumnType("decimal(10,5)");
            entity.Property(e => e.DaysAmount).IsRequired();
            entity.Property(e => e.BaseAmount).IsRequired().HasColumnType("decimal(10,5)");
            entity.Property(e => e.CompanyAmount).IsRequired().HasColumnType("decimal(10,5)");
            entity.Property(e => e.CompanyPercentage).IsRequired().HasColumnType("decimal(10,2)");
            entity.Property(e => e.ProfitDistributedLevels).IsRequired().HasColumnType("decimal(10,5)");
            entity.Property(e => e.TotalPercentage).IsRequired().HasColumnType("decimal(10,2)");
            entity.Property(e => e.DeductionAmount).IsRequired().HasColumnType("decimal(10,5)");
            entity.Property(e => e.PaymentAmount).IsRequired().HasColumnType("decimal(10,5)");
            entity.Property(e => e.Points).IsRequired().HasMaxLength(50).HasColumnType("varchar(50)");
            entity.Property(e => e.CasePool).IsRequired().HasMaxLength(50).HasColumnType("varchar(50)");
            entity.Property(e => e.PeriodPool).IsRequired().HasColumnType("datetime");
            entity.Property(e => e.CompletedAt).HasColumnType("datetime");
            entity.Property(e => e.UserCreatedAt).HasColumnType("datetime");

            entity.HasOne(d => d.ModelConfiguration)
                .WithMany(p => p.ResultsModel2)
                .HasForeignKey(d => d.EcoPoolConfigurationId);
        });

        modelBuilder.Entity<ResultsModel2Levels>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ResultEcoPoolId).IsRequired();
            entity.Property(e => e.AffiliateId).IsRequired();
            entity.Property(e => e.AffiliateName).IsRequired().HasMaxLength(50).HasColumnType("varchar(50)");
            entity.Property(e => e.Level).IsRequired();
            entity.Property(e => e.PercentageLevel).IsRequired().HasColumnType("decimal(10,2)");
            entity.Property(e => e.CompanyPercentageLevel).IsRequired().HasColumnType("decimal(10,2)");
            entity.Property(e => e.CompanyAmountLevel).IsRequired().HasColumnType("decimal(10,5)");
            entity.Property(e => e.CommisionAmount).IsRequired().HasColumnType("decimal(10,5)");
            entity.Property(e => e.PaymentAmount).IsRequired().HasColumnType("decimal(10,5)");
            entity.Property(e => e.Points).HasColumnType("decimal(10,5)");
            entity.Property(e => e.CompletedAt).HasColumnType("datetime");
            entity.Property(e => e.BinarySide).IsRequired().HasColumnType("int");
            entity.Property(e => e.UserCreatedAt).HasColumnType("datetime");

            entity.HasOne(d => d.ResultsModel2)
                .WithMany(p => p.ResultsModel2Levels)
                .HasForeignKey(d => d.ResultEcoPoolId);
        });

        modelBuilder.Entity<ModelFourStatistics>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.AffiliateId).IsRequired();
            entity.Property(e => e.AffiliateNetworkId).IsRequired();
            entity.Property(e => e.InvoiceId).IsRequired();
            entity.Property(e => e.CreditLeft).HasColumnType("decimal(10,2)");
            entity.Property(e => e.CreditRight).HasColumnType("decimal(10,2)");
            entity.Property(e => e.DebitLeft).HasColumnType("decimal(10,2)");
            entity.Property(e => e.DebitRight).HasColumnType("decimal(10,2)");
            entity.Property(e => e.Concept).HasColumnType("varchar(100)");
            entity.Property(e => e.Date)
                .IsRequired();
            entity.Property(e => e.Compression).IsRequired().HasColumnType("bit");
        });


        modelBuilder.Entity<ApiClient>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired();
            entity.Property(e => e.Token).IsRequired();
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.CreatedAt).IsRequired().HasColumnType("datetime");
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
            entity.Property(e => e.DeletedAt).HasColumnType("datetime");
        });

        modelBuilder.Entity<PaymentTransaction>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.IdTransaction).IsRequired().HasMaxLength(255).HasColumnType("varchar(255)");
            entity.Property(e => e.AffiliateId).IsRequired();
            entity.Property(e => e.Amount).IsRequired().HasColumnType("decimal(10,2)");
            entity.Property(e => e.AmountReceived).IsRequired().HasColumnType("decimal(10,2)");
            entity.Property(e => e.Products).IsRequired().HasMaxLength(200).HasColumnType("varchar(200)");
            entity.Property(e => e.Acredited).IsRequired().HasColumnType("bit");
            entity.Property(e => e.Status).IsRequired();
            entity.Property(e => e.CreatedAt).IsRequired().HasColumnType("datetime");
            entity.Property(e => e.UpdatedAt).IsRequired().HasColumnType("datetime");
            entity.Property(e => e.DeletedAt).HasColumnType("datetime");
            entity.Property(e => e.BrandId).IsRequired();

            entity.HasQueryFilter(e => !e.DeletedAt.HasValue);
        });

        modelBuilder.Entity<Commissions>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ConceptId).IsRequired();
            entity.Property(e => e.AffiliateId).IsRequired();
            entity.Property(e => e.DepositAffiliateId).IsRequired();
            entity.Property(e => e.InvoiceId).IsRequired();
            entity.Property(e => e.Level).HasColumnType("int");
            entity.Property(e => e.Gif).HasColumnType("decimal(10,2)");
            entity.Property(e => e.Compression).HasColumnType("decimal(10,2)");
            entity.Property(e => e.Compressionable).HasColumnType("decimal(10,2)");
            entity.Property(e => e.SalesPrice).HasColumnType("decimal(10,2)");
            entity.Property(e => e.Tax).HasColumnType("decimal(10,2)");
            entity.Property(e => e.Quantity).HasColumnType("int");
            entity.Property(e => e.Percentage).HasColumnType("decimal(10,2)");
            entity.Property(e => e.SubtractBinary).HasColumnType("decimal(10,2)");
            entity.Property(e => e.CompressionPosition).HasColumnType("tinyint");
            entity.Property(e => e.ClosingDate).HasColumnType("datetime");
            entity.Property(e => e.CreatedAt).IsRequired().HasColumnType("datetime");
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
            entity.Property(e => e.DeletedAt).HasColumnType("datetime");

            entity.HasQueryFilter(e => !e.DeletedAt.HasValue);
        });

        modelBuilder.Entity<ResultsModel3>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ProductExternalId).IsRequired();
            entity.Property(e => e.AffiliateId).IsRequired();
            entity.Property(e => e.AffiliateName).IsRequired();
            entity.Property(e => e.ProductName).IsRequired();
            entity.Property(e => e.BaseAmount).HasColumnType("decimal(10, 5)");
            entity.Property(e => e.ProfitDistributedLevels).HasColumnType("decimal(10,5)");
            entity.Property(e => e.TotalPercentage).HasColumnType("decimal(10,2)");
            entity.Property(e => e.PaymentAmount).HasColumnType("decimal(10,5)");
            entity.Property(e => e.Points).HasColumnType("varchar(50)");
            entity.Property(e => e.UserCreatedAt).HasColumnType("datetime");
        });

        modelBuilder.Entity<ResultsModel3Levels>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ResultsModel3Id).IsRequired();
            entity.Property(e => e.AffiliateId).IsRequired();
            entity.Property(e => e.AffiliateName).IsRequired();
            entity.Property(e => e.Level).IsRequired();
            entity.Property(e => e.PercentageLevel).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.PaymentAmount).HasColumnType("decimal(10,5)");
            entity.Property(e => e.Points).HasColumnType("decimal(10,5)");
            entity.Property(e => e.CompletedAt).HasColumnType("datetime");
            entity.Property(e => e.BinarySide).IsRequired();
            entity.Property(e => e.UserCreatedAt).HasColumnType("datetime");

            entity.HasOne(d => d.ResultsModel3)
                .WithMany(p => p.ResultsModel3Levels)
                .HasForeignKey(d => d.ResultsModel3Id);
        });

        modelBuilder.Entity<WalletsServiceModel1A>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.AffiliateId)
                .HasColumnType("int")
                .IsRequired();

            entity.HasIndex(e => e.AffiliateId, "index_affiliate_id");
            entity.HasIndex(e => e.UserId, "index_user_id");

            entity.Property(e => e.UserId)
                .HasColumnType("int");

            entity.Property(e => e.Credit)
                .HasColumnType("decimal(10,5)")
                .HasDefaultValue(0.0m);

            entity.Property(e => e.Debit)
                .HasColumnType("decimal(10,5)")
                .HasDefaultValue(0.0m);

            entity.Property(e => e.Status).HasColumnType("bit");

            entity.Property(e => e.Concept)
                .HasColumnType("varchar(255)")
                .HasMaxLength(255)
                .IsRequired();
            entity.Property(e => e.CreatedAt).IsRequired().HasColumnType("datetime");
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
            entity.Property(e => e.DeletedAt).HasColumnType("datetime");

            entity.HasQueryFilter(e => !e.DeletedAt.HasValue);
        });

        modelBuilder.Entity<WalletsServiceModel1B>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.AffiliateId)
                .HasColumnType("int")
                .IsRequired();

            entity.HasIndex(e => e.AffiliateId, "index_affiliate_id");
            entity.HasIndex(e => e.UserId, "index_user_id");

            entity.Property(e => e.UserId)
                .HasColumnType("int");

            entity.Property(e => e.Credit)
                .HasColumnType("decimal(10,5)")
                .HasDefaultValue(0.0m);

            entity.Property(e => e.Debit)
                .HasColumnType("decimal(10,5)")
                .HasDefaultValue(0.0m);

            entity.Property(e => e.Status).HasColumnType("bit");

            entity.Property(e => e.Concept)
                .HasColumnType("varchar(255)")
                .HasMaxLength(255)
                .IsRequired();
            entity.Property(e => e.CreatedAt).IsRequired().HasColumnType("datetime");
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
            entity.Property(e => e.DeletedAt).HasColumnType("datetime");

            entity.HasQueryFilter(e => !e.DeletedAt.HasValue);
        });

        modelBuilder.Entity<WalletsServiceModel2>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.AffiliateId).HasColumnType("int").IsRequired();
            entity.HasIndex(e => e.AffiliateId, "index_affiliate_id");
            entity.HasIndex(e => e.UserId, "index_user_id");
            entity.Property(e => e.UserId).HasColumnType("int");
            entity.Property(e => e.Credit).HasColumnType("decimal(10,5)").HasDefaultValue(0.0m);
            entity.Property(e => e.Debit).HasColumnType("decimal(10,5)").HasDefaultValue(0.0m);
            entity.Property(e => e.Status).HasColumnType("bit");
            entity.Property(e => e.Concept).HasColumnType("varchar(255)").HasMaxLength(255).IsRequired();
            entity.Property(e => e.CreatedAt).IsRequired().HasColumnType("datetime");
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
            entity.Property(e => e.DeletedAt).HasColumnType("datetime");
            entity.Property(e => e.BrandId).IsRequired();
            
            entity.HasQueryFilter(e => !e.DeletedAt.HasValue);
        });

        modelBuilder.Entity<Brand>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).HasColumnType("nvarchar(100)");
            entity.Property(e => e.SecretKey).HasColumnType("nvarchar(64)");
            entity.Property(e => e.IsActive).HasColumnType("bit");
            entity.Property(e => e.CreatedAt).IsRequired().HasColumnType("datetime");
            entity.Property(e => e.UpdatedAt).IsRequired().HasColumnType("datetime");
            entity.Property(e => e.DeletedAt).HasColumnType("datetime");

            entity.HasQueryFilter(e => !e.DeletedAt.HasValue);
        });

        modelBuilder.Entity<Bonuses>(entity =>
        {
            entity.HasKey(e   => e.BonusId);
            entity.Property(e => e.AffiliateId).IsRequired().HasColumnType("int");
            entity.Property(e => e.CurrentAmount).IsRequired().HasColumnType("decimal(18,8)");
            entity.Property(e => e.Status).HasColumnType("bit");
            entity.Property(e => e.CreatedAt).IsRequired().HasColumnType("datetime");
            entity.Property(e => e.UpdatedAt).IsRequired().HasColumnType("datetime");
            entity.Property(e => e.DeletedAt).HasColumnType("datetime");
            
            entity.HasQueryFilter(e => !e.DeletedAt.HasValue);
        });
    }
}