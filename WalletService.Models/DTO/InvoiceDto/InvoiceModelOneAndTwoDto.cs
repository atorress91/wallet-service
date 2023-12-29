namespace WalletService.Models.DTO.InvoiceDto;

public class InvoiceModelOneAndTwoDto
{
    public string   UserName       { get; set; } = string.Empty;
    public int      InvoiceId      { get; set; }
    public string   ProductName    { get; set; } = string.Empty;
    public decimal  ProductPrice   { get; set; }
    public int      PaymentGroupId { get; set; }
    public DateTime CreatedAt      { get; set; }
}