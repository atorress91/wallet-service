namespace WalletService.Data.Database.Models;

public class Bonuses
{
    public int BonusId { get; set; }
    public int AffiliateId { get; set; }
    public decimal CurrentAmount { get; set; }
    public bool Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
}