namespace WalletService.Data.Database.Models;

public class MatrixQualification
{
    public int QualificationId { get; set; }
    public long UserId { get; set; }
    public int MatrixType { get; set; }
    public decimal TotalEarnings { get; set; }
    public decimal WithdrawnAmount { get; set; }
    public decimal AvailableBalance { get; set; }
    public bool IsQualified { get; set; }
    public DateTime QualificationDate { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
}