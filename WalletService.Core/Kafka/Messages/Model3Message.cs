using WalletService.Models.DTO.InvoiceDetailDto;
using WalletService.Models.DTO.ProcessGradingDto;
using WalletService.Models.DTO.ProductWalletDto;
using WalletService.Models.Responses;

namespace WalletService.Core.Kafka.Messages;

public class Model3Message
{
    public ICollection<InvoiceDetailDto> EducatedCourses { get; set; } = new List<InvoiceDetailDto>();
    public ICollection<ProductWalletDto> ListResultProducts { get; set; } = new List<ProductWalletDto>();
    public ICollection<UserModelResponse> ListResultAccounts { get; set; } = new List<UserModelResponse>();
    public ModelConfigurationDto Configuration { get; set; }
}