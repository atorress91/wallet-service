namespace WalletService.Models.Requests.MatrixRequest;

public class MatrixRequest
{
    public int UserId { get; set; }
    public int MatrixType { get; set; }
    public int? RecipientId { get; set; }
}