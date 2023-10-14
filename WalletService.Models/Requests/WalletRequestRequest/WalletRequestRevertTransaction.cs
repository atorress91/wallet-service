namespace WalletService.Models.Requests.WalletRequestRequest;

public class WalletRequestRevertTransaction
{
    public int AffiliateId { get; set; }
    public int InvoiceId { get; set; }
    public string Concept { get; set; } = string.Empty;
}