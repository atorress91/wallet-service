using System.Text.Json.Serialization;

namespace WalletService.Models.Responses;

public class GetAccountsEcoPoolResponse
{
    [JsonPropertyName("success")]
    public bool Success { get; set; }
    
    [JsonPropertyName("data")]
    public List<UserModelTwoThreeResponse> Data { get; set; }
    
    [JsonPropertyName("message")]
    public string Message { get; set; }
    
    [JsonPropertyName("code")]
    public int Code { get; set; }
}

public class UserModelTwoThreeResponse
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
    
    [JsonPropertyName("userName")]
    public string UserName { get; set; }
    
    [JsonPropertyName("email")]
    public string Email { get; set; }
    
    [JsonPropertyName("name")]
    public string? Name { get; set; }
    
    [JsonPropertyName("lastName")]
    public string? LastName { get; set; }
    
    [JsonPropertyName("level")]
    public int Level { get; set; }
    
    [JsonPropertyName("father")]
    public int Father { get; set; }
    
    [JsonPropertyName("side")]
    public int Side { get; set; }
    
    [JsonPropertyName("familyTree")]
    public List<UserLevel> FamilyTree { get; set; }
}

public class UserLevel
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
    
    [JsonPropertyName("userName")]
    public string UserName { get; set; }
    
    [JsonPropertyName("email")]
    public string Email { get; set; }
    
    [JsonPropertyName("name")]
    public string? Name { get; set; }
    
    [JsonPropertyName("lastName")]
    public string? LastName { get; set; }
    
    [JsonPropertyName("level")]
    public int Level { get; set; }
    
    [JsonPropertyName("father")]
    public int Father { get; set; }
    
    [JsonPropertyName("side")]
    public int Side { get; set; }
}