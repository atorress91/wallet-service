using Newtonsoft.Json;

namespace WalletService.Models.Responses;


public class GetConfigurationResponse
{
    [JsonProperty("success")]
    public bool Success { get; set; }
    
    [JsonProperty("data")]
    public ProductConfigurationDto Data { get; set; }
    
    [JsonProperty("message")]
    public string Message { get; set; }
    
    [JsonProperty("code")]
    public int Code { get; set; }
}

public class ProductConfigurationDto
{
    [JsonProperty("activate_shipping_system")]
    public bool ActivateShippingSystem { get; set; }

    [JsonProperty("activate_passive_payments_module")]
    public bool ActivatePassivePaymentsModule { get; set; }

    [JsonProperty("activate_public_shop")]
    public bool ActivatePublicShop { get; set; }

    [JsonProperty("currency_symbol")] public string? CurrencySymbol { get; set; }

    [JsonProperty("symbol_commissionable_value")]
    public string? SymbolCommissionableValue { get; set; }

    [JsonProperty("symbol_points_qualify")]
    public string? SymbolPointsQualify { get; set; }

    [JsonProperty("binary_points_symbol")]
    public string? BinaryPointsSymbol { get; set; }

    [JsonProperty("new_product_label")]
    public int NewProductLabel { get; set; }
}