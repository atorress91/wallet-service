﻿
namespace WalletService.Data.Database.Models;

public partial class ApiClient
{
    public long Id { get; set; }

    public string Name { get; set; } = null!;

    public string Token { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }
}
