namespace WalletService.Data.Database.Models;

public class LeaderBoardModel5
{
    public int Id { get; set; }
    public int AffiliateId { get; set; }
    public string MatrixPosition { get; set; }
    public int GradingPosition { get; set; }
    public DateTime CreatedAt { get; set; }
    public decimal Amount { get; set; }
}