namespace WalletService.Models.DTO.WalletDto;

public class BalanceInformationAdminDto
{
    public decimal WalletProfit { get; set; }
    public decimal CommissionsPaid { get; set; }
    public decimal CalculatedCommissions { get; set; }
    public int EnabledAffiliates { get; set; }
}