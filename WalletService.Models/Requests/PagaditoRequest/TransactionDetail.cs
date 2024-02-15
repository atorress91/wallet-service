using System.Text.Json.Serialization;

namespace WalletService.Models.Requests.PagaditoRequest;

public class TransactionDetail
{
    public int quantity { get; set; }
    public string description { get; set; } = String.Empty;
    public decimal price { get; set; }
    public string url_product { get; set; } = String.Empty;
}