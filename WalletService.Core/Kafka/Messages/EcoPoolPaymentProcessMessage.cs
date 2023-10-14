using WalletService.Data.Database.Models;

namespace WalletService.Core.Kafka.Messages;

public class EcoPoolPaymentProcessMessage
{
    public IEnumerable<ResultsEcoPool> EcoPools { get; set; }
    public EcoPoolConfiguration EcoPoolConfiguration { get; set; }
}