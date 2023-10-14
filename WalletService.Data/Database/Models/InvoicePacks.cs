namespace WalletService.Data.Database.Models;

public class InvoicePacks
{
    public int Id { get; set; }
    public int InvoiceId { get; set; }
    public int ProductId { get; set; }
    public decimal BaseAmount { get; set; }
    public decimal Percentage { get; set; }
    public int CountDays { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
    public Invoices Invoice { get; set; }
}