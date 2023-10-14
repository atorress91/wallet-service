namespace WalletService.Models.DTO.CoinPaymentTransactionDto;

public class PaymentTransactionDto
{
    public int Id { get; set; }
    public string IdTransaction { get; set; }
    public int AffiliateId { get; set; }
    public decimal Amount { get; set; }
    public string Products { get; set; }
    public int Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
    public string PaymentMethod { get; set; }
}