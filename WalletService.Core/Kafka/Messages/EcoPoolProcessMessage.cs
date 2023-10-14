using WalletService.Data.Database.Models;
using WalletService.Models.DTO.InvoiceDto;
using WalletService.Models.DTO.ProcessGradingDto;
using WalletService.Models.DTO.ProductWalletDto;
using WalletService.Models.Responses;

namespace WalletService.Core.Kafka.Messages;

public class EcoPoolProcessMessage
{
    public ICollection<InvoicePackDto>      Pools              { get; set; } 
    public ICollection<ProductWalletDto>    ListResultProducts { get; set; }
    public ICollection<UserEcoPoolResponse> ListResultAccounts { get; set; }
    public EcoPoolConfigurationDto          Configuration      { get; set; }
    public DateTime                         StarDate           { get; set; }
    public DateTime                         EndDate            { get; set; }
    public decimal                          Points             { get; set; }
}