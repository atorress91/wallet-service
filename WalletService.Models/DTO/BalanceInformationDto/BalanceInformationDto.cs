namespace WalletService.Models.DTO.BalanceInformationDto;

public class BalanceInformationDto
{
    public decimal? ReverseBalance { get; set; }

    public decimal? TotalAcquisitions { get; set; }

    public decimal? AvailableBalance { get; set; }

    public double? TotalCommissionsPaid { get; set; }
}