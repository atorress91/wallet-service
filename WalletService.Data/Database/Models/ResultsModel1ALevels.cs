namespace WalletService.Data.Database.Models;

public class ResultsModel1ALevels
{
    public int Id { get; set; }
    public int ResultsModel1AId { get; set; }
    public int AffiliateId { get; set; }
    public string AffiliateName { get; set; }
    public int Level { get; set; }
    public decimal PercentageLevel { get; set; }
    public decimal PaymentAmount { get; set; }
    public decimal? Points { get; set; }
    public DateTime? CompletedAt { get; set; }
    public DateTime? UserCreatedAt { get; set; }
    public int BinarySide { get; set; }
    
    public virtual ResultsModel1A ResultsModel1A { get; set; }

}


