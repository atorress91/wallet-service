using WalletService.Models.DTO.GradingDto;
using WalletService.Models.Requests.UserGradingRequest;

namespace WalletService.Core.Kafka.Messages;

public class ModelFourMessage
{
    public ICollection<GradingDto> Gradings { get; set; }
    public ICollection<UserGradingRequest> UserGradings { get; set; }
}