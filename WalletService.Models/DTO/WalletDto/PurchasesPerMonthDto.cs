﻿namespace WalletService.Models.DTO.WalletDto;

public class PurchasesPerMonthDto
{
    public int Year { get; set; }
    public int Month { get; set; }
    public int TotalPurchases { get; set; }
}