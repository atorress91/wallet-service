
namespace WalletService.Models.Responses;

public class CreateConPaymentsTransactionResponse
{

    public string? Error { get; set; }
    public PaymentResult? Result { get; set; }
    
}

public class PaymentResult
{
    public string? Amount { get; set; }
    public string? Txn_Id { get; set; }
    public string? Address { get; set; }
    public string? Confirms_Needed { get; set; }
    public int Timeout { get; set; }
    public string? Checkout_Url { get; set; }
    public string? Status_Url { get; set; }
    public string? Qrcode_Url { get; set; }
}