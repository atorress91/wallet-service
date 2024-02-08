using WalletService.Models.DTO.InvoiceDetailDto;
using WalletService.Models.DTO.ProcessGradingDto;
using WalletService.Models.DTO.ProductWalletDto;
using WalletService.Models.Responses;

namespace WalletService.Core.Kafka.Messages;

public class Model2Message
{
    public ICollection<InvoiceDetailDto>  ItemWithInMonth  { get; set; } 
    public ICollection<InvoiceDetailDto>  ItemWithOutMonth  { get; set; } 
    public ICollection<ProductWalletDto>  ListResultProducts { get; set; }
    public ICollection<UserModelResponse> ListResultAccounts   { get; set; }
    public ModelConfigurationDto          Configuration        { get; set; }
    public DateTime                       StarDate             { get; set; }
    public DateTime                       EndDate              { get; set; }
    public decimal                        Points               { get; set; }
}