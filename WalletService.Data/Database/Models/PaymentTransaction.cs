using System.ComponentModel.DataAnnotations.Schema;

namespace WalletService.Data.Database.Models;

[Table("CoinPaymentTransactions")]
public class PaymentTransaction
{
    public int Id { get; set; }
    public string? IdTransaction { get; set; }
    public int AffiliateId { get; set; }
    public decimal Amount { get; set; }
    public decimal AmountReceived { get; set; }
    public string? Products { get; set; }
    public int Status { get; set; }
    public bool Acredited { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
    public string? PaymentMethod { get; set; }
}