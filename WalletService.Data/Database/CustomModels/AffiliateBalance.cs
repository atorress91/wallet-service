namespace WalletService.Data.Database.CustomModels;

public class AffiliateBalance
{
    public int AffiliateId { get; set; }
    public string? AffiliateUserName { get; set; }
    public double Balance { get; set; }
}