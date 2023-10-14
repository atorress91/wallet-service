namespace WalletService.Data.Database.Models;

public class ApiClient
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Token { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
}