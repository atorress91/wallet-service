﻿namespace WalletService.Models.Requests.RequestValidationCode;

public class RequestValidationCode
{
        public int UserId { get; set; }
        public string Code{ get; set; }
        public string Password { get; set; }
        
}