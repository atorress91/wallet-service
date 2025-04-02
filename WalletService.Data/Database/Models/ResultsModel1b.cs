﻿
namespace WalletService.Data.Database.Models;

public partial class ResultsModel1b
{
    public long Id { get; set; }

    public int ProductExternalId { get; set; }

    public int AffiliateId { get; set; }

    public string AffiliateName { get; set; } = null!;

    public string ProductName { get; set; } = null!;

    public decimal BaseAmount { get; set; }

    public decimal ProfitDistributedLevels { get; set; }

    public decimal TotalPercentage { get; set; }

    public decimal PaymentAmount { get; set; }

    public string Points { get; set; } = null!;

    public DateOnly PeriodPool { get; set; }

    public DateTime? CompletedAt { get; set; }

    public DateTime? UserCreatedAt { get; set; }

    public virtual ICollection<ResultsModel1bLevel> ResultsModel1bLevels { get; } = new List<ResultsModel1bLevel>();
}
