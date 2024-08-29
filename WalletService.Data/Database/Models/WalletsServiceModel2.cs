namespace WalletService.Data.Database.Models;

public class WalletsServiceModel2
{
    public int Id { get; set; }
    public int AffiliateId { get; set; }
    public int? UserId { get; set; }
    public decimal? Credit { get; set; }
    public decimal? Debit { get; set; }
    public bool Status { get; set; }
    public string Concept { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
    public int BrandId { get; set; }
}