namespace WalletService.Models.Requests.CoinPayRequest;

public class WithDrawalRequest
{
    public int IdUser { get; set; }
    public int IdCurrency { get; set; }
    public int Amount { get; set; }
    public string? Description { get; set; }
    public string? IdReference { get; set; }
    public bool TakeOfAnyBalance { get; set; }
}