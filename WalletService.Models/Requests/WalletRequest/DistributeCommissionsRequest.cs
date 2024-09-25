namespace WalletService.Models.Requests.WalletRequest;

public class DistributeCommissionsRequest
{
    public int AffiliateId { get; set; }
    public decimal InvoiceAmount { get; set; }
    public int BrandId { get; set; }
}