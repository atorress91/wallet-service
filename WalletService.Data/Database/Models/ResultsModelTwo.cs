namespace WalletService.Data.Database.Models;

public class ResultsModelTwo
{
    public int Id { get; set; }
    public int ProductExternalId { get; set; }
    public int AffiliateId { get; set; }
    public string AffiliateName { get; set; }
    public string ProductName { get; set; }
    public decimal BaseAmount { get; set; }
    public decimal ProfitDistributedLevels { get; set; }
    public decimal TotalPercentage { get; set; }
    public decimal PaymentAmount { get; set; }
    public string Points { get; set; }
    public DateTime PeriodPool { get; set; }
    public DateTime? CompletedAt { get; set; }
}
