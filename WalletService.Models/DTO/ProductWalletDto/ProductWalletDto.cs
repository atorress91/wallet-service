using Newtonsoft.Json;

namespace WalletService.Models.DTO.ProductWalletDto;

public class ProductWalletDto
{
    [JsonProperty("id")] public int Id { get; set; }
    [JsonProperty("categoryId")] public int CategoryId { get; set; }
    [JsonProperty("salePrice")] public decimal SalePrice { get; set; }
    [JsonProperty("commissionableValue")] public decimal CommissionableValue { get; set; }
    [JsonProperty("binaryPoints")] public decimal BinaryPoints { get; set; }
    [JsonProperty("valuePoints")] public int ValuePoints { get; set; }
    [JsonProperty("tax")] public decimal Tax { get; set; }
    [JsonProperty("modelTwoPercentage")] public decimal? ModelTwoPercentage { get; set; }
    [JsonProperty("paymentGroup")] public int PaymentGroup { get; set; }
    [JsonProperty("acumCompMin")] public bool AcumCompMin { get; set; }
    [JsonProperty("productType")] public bool ProductType { get; set; }
    [JsonProperty("productPacks")] public bool ProductPacks { get; set; }
    [JsonProperty("baseAmount")] public decimal BaseAmount { get; set; }
    [JsonProperty("dailyPercentage")] public decimal DailyPercentage { get; set; }
    [JsonProperty("daysWait")] public int DaysWait { get; set; }

    [JsonProperty("name")] public string? Name { get; set; }
    [JsonProperty("productDiscount")] public decimal ProductDiscount { get; set; }

}