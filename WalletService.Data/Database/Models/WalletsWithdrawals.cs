namespace WalletService.Data.Database.Models;

public class WalletsWithdrawals
{
    public int Id { get; set; }
    public int AffiliateId { get; set; }
    public string AffiliateUserName { get; set; }
    public decimal Amount { get; set; }
    public string? Observation { get; set; }
    public string? AdminObservation { get; set; }
    public DateTime Date { get; set; }
    public DateTime? ResponseDate { get; set; }
    public decimal RetentionPercentage { get; set; }
    
    public bool IsProcessed { get; set; }
    public bool Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
}