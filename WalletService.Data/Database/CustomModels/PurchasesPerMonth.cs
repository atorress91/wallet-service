namespace WalletService.Data.Database.CustomModels;

public class PurchasesPerMonth
{
    public int Year { get; set; }
    public int Month { get; set; }
    public int TotalPurchases { get; set; }
}