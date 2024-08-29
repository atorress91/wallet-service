using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WalletService.Data.Database.Models;

[Table("CoinPaymentTransactions")]
public class PaymentTransaction
{
    public int Id { get; set; }
    [MaxLength(255)] 
    public string IdTransaction { get; set; } = string.Empty;
    public int AffiliateId { get; set; }
    public decimal Amount { get; set; }
    public decimal AmountReceived { get; set; }
    [MaxLength(200)] 
    public string Products { get; set; } = string.Empty;
    public int Status { get; set; }
    public bool Acredited { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
    [MaxLength(50)] 
    public string? PaymentMethod { get; set; }
    
    [MaxLength(50)]
    public string? Reference { get; set; }
    public int BrandId { get; set; }
}