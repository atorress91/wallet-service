using WalletService.Data.Database.Models;

namespace WalletService.Core.Kafka.Messages;

public class PaymentModelTwoThreeMessage
{
    public IEnumerable<ResultsModel2> EcoPools { get; set; }
    public ModelConfiguration ModelConfiguration { get; set; }
}