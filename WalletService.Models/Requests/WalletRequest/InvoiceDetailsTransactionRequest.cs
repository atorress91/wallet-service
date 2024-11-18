namespace WalletService.Models.Requests.WalletRequest;

public class InvoiceDetailsTransactionRequest
{
    public int ProductId { get; set; }
    public int PaymentGroupId { get; set; }
    public byte AccumMinPurchase { get; set; }
    public string ProductName { get; set; } = null!;
    public decimal ProductPrice { get; set; }
    public decimal? ProductPriceBtc { get; set; }
    public decimal ProductIva { get; set; }
    public int ProductQuantity { get; set; }
    public decimal? ProductCommissionable { get; set; }
    public decimal? BinaryPoints { get; set; }
    public int? ProductPoints { get; set; }
    public decimal ProductDiscount { get; set; }
    public int? CombinationId { get; set; }
    public byte? ProductPack { get; set; }
    public decimal? BaseAmount { get; set; }
    public decimal? DailyPercentage { get; set; }
    
    public int? WaitingDays { get; set; }
    
    public int? DaysToPayQuantity { get; set; }
    public byte ProductStart { get; set; }
    public long BrandId { get; set; }
    
    
}