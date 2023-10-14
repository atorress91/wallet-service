namespace WalletService.Data.Database.Models;

public class NetworkPurchases
{
    public int Id { get; set; }
    public int InvoiceId { get; set; }
    public int AffiliateId { get; set; }
    public int Level { get; set; }
    public double CommisionableAmount { get; set; }
    public double Points { get; set; }
    public byte Origin { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }

}