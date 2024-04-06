namespace WalletService.Models.Requests.WalletRequest;

public class DebitTransactionRequest
{
    public int AffiliateId { get; set; }
    public int UserId { get; set; }
    public string Concept { get; set; } = null!;
    public decimal Points { get; set; }
    public decimal Commissionable { get; set; }
    public string? Bank { get; set; }
    public string PaymentMethod { get; set; } = null!;
    public byte Origin { get; set; }
    public int Level { get; set; }
    public decimal Debit { get; set; }
    public string AffiliateUserName { get; set; }
    public string AdminUserName { get; set; } = null!;
    public string? ReceiptNumber { get; set; }
    public byte Type { get; set; }
    public string? SecretKey { get; set; }
    public string? ConceptType { get; set; }
    public string? Reason { get; set; }

    public List<InvoiceDetailsTransactionRequest> invoices { get; set; }
}