using WalletService.Models.Responses;

namespace WalletService.Models.DTO.CoinPayDto;

public class SendFundsDto
{
    public List<SendFundsResponse> SuccessfulResponses { get; set; } = new List<SendFundsResponse>();
    public List<SendFundsResponse> FailedResponses { get; set; } = new List<SendFundsResponse>();
}