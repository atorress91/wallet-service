using WalletService.Models.DTO.InvoiceDetailDto;
using WalletService.Models.DTO.ProcessGradingDto;
using WalletService.Models.DTO.ProductWalletDto;
using WalletService.Models.Responses;

namespace WalletService.Core.Kafka.Messages;

public class Model3Message
{
    public ICollection<InvoiceDetailDto> EducatedCourses { get; set; }
    public ICollection<ProductWalletDto> ListResultProducts { get; set; }
    public ICollection<UserModelResponse> ListResultAccounts { get; set; }
    public ModelConfigurationDto Configuration { get; set; }
}