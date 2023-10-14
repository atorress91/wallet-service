namespace WalletService.Models.Requests.CoinPayRequest;

public class CreateAddresRequest
{
    public int IdCurrency { get; set; }
    public int IdNetwork { get; set; }
    public int IdWallet { get; set; }
}