using System.Security.Cryptography.X509Certificates;

namespace WalletService.Data.Database.Models;

public class Commissions
{
    public int Id { get; set; }
    public int ConceptId { get; set; }
    public int AffiliateId { get; set; }
    public int DepositAffiliateId { get; set; }
    public int InvoiceId { get; set; }
    public int? Level { get; set; }
    public decimal? Gif { get; set; }
    public decimal? Compression { get; set; }
    public decimal? Compressionable { get; set; }
    public decimal? SalesPrice { get; set; }
    public decimal? Tax { get; set; }
    public int? Quantity { get; set; }
    public decimal? Percentage { get; set; }
    public decimal? SubtractBinary { get; set; }
    public byte? CompressionPosition { get; set; }
    public DateTime ClosingDate { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
}