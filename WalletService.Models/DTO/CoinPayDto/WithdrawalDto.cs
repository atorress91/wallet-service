using WalletService.Models.Enums;
using WalletService.Models.Responses;
using WalletService.Models.Responses.BaseResponses;

namespace WalletService.Models.DTO.CoinPayDto;

public class WithdrawalDto
{
    public ServicesResponse? Response { get; set; }
    public WithdrawalStatus Status { get; set; }

}