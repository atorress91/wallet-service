﻿namespace WalletService.Models.Requests.WalletWithDrawalRequest;

public class WalletWithDrawalRequest
{
    public int Id { get; set; }
    public int AffiliateId { get; set; }
    public decimal Amount { get; set; }
    public bool State { get; set; }
    public string? Observation { get; set; }
    public string? AdminObservation { get; set; }
    public DateTime Date { get; set; }
    public DateTime? ResponseDate { get; set; }
    public decimal RetentionPercentage { get; set; }
    public bool Status { get; set; }

}