namespace WalletService.Models.DTO.ProcessGradingDto;

public class EcoPoolConfigurationDto
{
    public int Id { get; set; }
    public double CompanyPercentage { get; set; }
    public double CompanyPercentageLevels { get; set; }
    public double EcoPoolPercentage { get; set; }
    public double MaxGainLimit { get; set; }
    public DateTime DateInit { get; set; }
    public DateTime DateEnd { get; set; }
    public int Case { get; set; }
    public DateTime? CompletedAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }

    public ICollection<EcoPoolLevelDto> Levels { get; set; } = new List<EcoPoolLevelDto>();
}