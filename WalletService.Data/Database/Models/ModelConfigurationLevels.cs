namespace WalletService.Data.Database.Models;

public class ModelConfigurationLevels
{
    public int Id { get; set; }
    public int EcoPoolConfigurationId { get; set; }
    public int Level { get; set; }
    public double Percentage { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public virtual ModelConfiguration ModelConfiguration { get; set; }
}