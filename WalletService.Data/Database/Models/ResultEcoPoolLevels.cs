namespace WalletService.Data.Database.Models;

public class ResultEcoPoolLevels
{
    public int Id { get; set; }
    public int ResultEcoPoolId { get; set; }
    public int AffiliateId { get; set; }
    public string AffiliateName { get; set; } = string.Empty;
    public int Level { get; set; }
    public decimal PercentageLevel { get; set; }
    public decimal CompanyPercentageLevel { get; set; }
    public decimal CompanyAmountLevel { get; set; }
    public decimal CommisionAmount { get; set; }
    public decimal PaymentAmount { get; set; }
    public decimal? Points { get; set; }
    public DateTime? CompletedAt { get; set; }
    public int BinarySide { get; set; }
    public DateTime? UserCreatedAt { get; set; }
    
    public virtual ResultsEcoPool ResultsEcoPool { get; set; }
}