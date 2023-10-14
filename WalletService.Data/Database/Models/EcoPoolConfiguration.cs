namespace WalletService.Data.Database.Models;

public class EcoPoolConfiguration
{
    public int Id { get; set; }
    public double CompanyPercentage { get; set; }
    public double CompanyPercentageLevels { get; set; }
    public double EcoPoolPercentage { get; set; }
    public double MaxGainLimit { get; set; }
    public DateTime DateInit { get; set; }
    public DateTime DateEnd { get; set; }
    public int Case { get; set; }

    public int? Processed { get; set; }
    public int? Totals { get; set; }
    public DateTime? CompletedAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }

    public virtual ICollection<EcoPoolLevels>  Levels { get; set; }
    public virtual ICollection<ResultsEcoPool> ResultsEcoPools { get; set; }
}