namespace WalletService.Data.Database.Models;

public class WalletsRetentionsConfigs
{
    public int Id { get; set; }
    public decimal WithdrawalFrom { get; set; }
    public decimal WithdrawalTo { get; set; }
    public decimal Percentage { get; set; }
    public DateTime Date { get; set; }
    public DateTime? DisableDate { get; set; }
    public bool Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
}