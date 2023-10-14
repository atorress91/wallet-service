namespace WalletService.Data.Database.Models;

public class WalletsPeriods
{
    public int Id { get; set; }
    public DateTime Date { get; set; }
    public bool Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
}