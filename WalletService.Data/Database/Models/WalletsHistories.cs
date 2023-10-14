﻿namespace WalletService.Data.Database.Models;

public class WalletsHistories
{
    public int Id { get; set; }
    public int? AffiliateId { get; set; }
    public int? UserId { get; set; }
    public double Credit { get; set; }
    public double Debit { get; set; }
    public double Deferred { get; set; }
    public bool Status { get; set; }
    public string? Concept { get; set; }
    public int? Support { get; set; }
    public DateTime Date { get; set; }
    public bool Compression { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
}