using WalletService.Models.DTO.GradingDto;

namespace WalletService.Models.Requests.UserGradingRequest;

public class UserGradingRequest
{
    public int AffiliateId { get; set; }
    public int AffiliateOwnerId { get; set; }
    public int Side { get; set; }
    public string UserName { get; set; }
    public double Points { get; set; }
    public double Commissions { get; set; }
    public GradingDto? Grading { get; set; }
}