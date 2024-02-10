using WalletService.Models.DTO.InvoiceDetailDto;
using WalletService.Models.DTO.ProcessGradingDto;
using WalletService.Models.DTO.ProductWalletDto;
using WalletService.Models.Responses;

namespace WalletService.Core.Kafka.Messages;

public class Model1AMessage
{
    public ICollection<InvoiceDetailDto>  ItemWithInMonth  { get; set; } = new List<InvoiceDetailDto>();
    public ICollection<InvoiceDetailDto> ItemWithOutMonth { get; set; } = new List<InvoiceDetailDto>();
    public ICollection<ProductWalletDto>  ListResultProducts { get; set; } = new List<ProductWalletDto>();
    public ICollection<UserModelResponse> ListResultAccounts   { get; set; } = new List<UserModelResponse>();
    public ModelConfigurationDto          Configuration        { get; set; }
    public DateTime                       StarDate             { get; set; }
    public DateTime                       EndDate              { get; set; }
    public decimal                        Points               { get; set; }
}