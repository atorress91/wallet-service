using WalletService.Models.DTO.CoinPayDto;

namespace WalletService.Models.Responses;

public class GetBalanceByCurrencyResponse
{
    public int StatusCode { get; set; }
    public int IdTypeStatusCode { get; set; }
    public string? Message { get; set; }
    public List<object>? Messages { get; set; }
    public UserBalanceDto? Data { get; set; }
}