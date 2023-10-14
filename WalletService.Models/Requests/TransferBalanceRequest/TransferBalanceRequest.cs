namespace WalletService.Models.Requests.TransferBalanceRequest;

public class TransferBalanceRequest
{
    public int FromAffiliateId { get; set; }
    public string FromUserName { get; set; }
    public string ToUserName { get; set; } = string.Empty;
    public decimal? Amount { get; set; }

    public string SecurityCode { get; set; } = string.Empty;
}