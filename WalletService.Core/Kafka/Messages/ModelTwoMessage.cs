using WalletService.Data.Database.Models;
using WalletService.Models.DTO.ProcessGradingDto;
using WalletService.Models.DTO.ProductWalletDto;
using WalletService.Models.Responses;

namespace WalletService.Core.Kafka.Messages;

public class ModelTwoMessage
{
    public ICollection<InvoicesDetails> EducatedCourses { get; set; }
    public ICollection<ProductWalletDto> ListResultProducts { get; set; }
    public ICollection<UserModelTwoThreeResponse> ListResultAccounts { get; set; }
    public EcoPoolConfigurationDto Configuration { get; set; }
}