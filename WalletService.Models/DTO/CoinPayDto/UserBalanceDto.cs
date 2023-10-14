namespace WalletService.Models.DTO.CoinPayDto;

public class UserBalanceDto
{
    public double Balance { get; set; }
    public int BlockedBalance { get; set; }
    public double CurrentBalance { get; set; }
    public CurrencyDto? Currency { get; set; }
    public bool IsActive { get; set; }
    public bool Locked { get; set; }
}