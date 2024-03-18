using Newtonsoft.Json;

namespace WalletService.Models.Responses;

public class GetAccountsEcoPoolResponse
{
    [JsonProperty("success")]
    public bool Success { get; set; }
    
    [JsonProperty("data")]
    public List<UserModelResponse> Data { get; set; }
    
    [JsonProperty("message")]
    public string Message { get; set; }
    
    [JsonProperty("code")]
    public int Code { get; set; }
}

public class UserModelResponse
{
    [JsonProperty("id")]
    public int Id { get; set; }
    
    [JsonProperty("userName")]
    public string UserName { get; set; }
    
    [JsonProperty("email")]
    public string Email { get; set; }
    
    [JsonProperty("name")]
    public string? Name { get; set; }
    
    [JsonProperty("lastName")]
    public string? LastName { get; set; }
    
    [JsonProperty("level")]
    public int Level { get; set; }
    
    [JsonProperty("father")]
    public int Father { get; set; }
    
    [JsonProperty("side")]
    public int Side { get; set; }
    
    [JsonProperty("createdAt")]
    public DateTime UserCreatedAt { get; set; }
    
    [JsonProperty("familyTree")]
    public List<UserLevel> FamilyTree { get; set; }
}

public class UserLevel
{
    [JsonProperty("id")]
    public int Id { get; set; }
    
    [JsonProperty("userName")]
    public string UserName { get; set; }
    
    [JsonProperty("email")]
    public string Email { get; set; }
    
    [JsonProperty("name")]
    public string? Name { get; set; }
    
    [JsonProperty("lastName")]
    public string? LastName { get; set; }
    
    [JsonProperty("level")]
    public int Level { get; set; }
    
    [JsonProperty("father")]
    public int Father { get; set; }
    
    [JsonProperty("side")]
    public int Side { get; set; }
    [JsonProperty("createdAt")]
    public DateTime UserCreatedAt { get; set; }
}