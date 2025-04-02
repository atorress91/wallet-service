namespace WalletService.Models.Requests.WalletPeriodRequest;

public class WalletPeriodRequest
{
    public int Id { get; set; }
    public DateOnly Date { get; set; }
    public bool Status { get; set; }
   
}