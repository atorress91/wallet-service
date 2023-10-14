namespace WalletService.Models.Requests.EcoPoolConfigurationRequest;

public class LevelEcoPoolRequest
{
    public int Id { get; set; }
    public int Level { get; set; }
    public double Percentage { get; set; }
}