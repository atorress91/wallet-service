namespace WalletService.Models.Requests.EcoPoolConfigurationRequest;

public class EcoPoolConfigurationRequest
{
    public int? Id { get; set; }
    public double CompanyPercentage { get; set; }
    
    public double CompanyPercentageLevels { get; set; }
    public double EcoPoolPercentage { get; set; }
    public double MaxGainLimit { get; set; }
    public DateTime DateInit { get; set; }
    public DateTime DateEnd { get; set; }
    public int Case { get; set; }
    
    public int? Processed { get; set; }
    
    public int? Totals { get; set; }
    public ICollection<LevelEcoPoolRequest> Levels { get; set; }
}