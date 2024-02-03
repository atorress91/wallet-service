namespace WalletService.Models.DTO.WalletModel1BDto;

public class BalanceInformationModel1BDto
{
    public decimal? ReverseBalance { get; set; }

    public decimal? TotalAcquisitions { get; set; }

    public decimal? AvailableBalance { get; set; }

    public double? TotalCommissionsPaid { get; set; }
    public decimal? ServiceBalance { get; set; }
}