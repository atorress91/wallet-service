namespace WalletService.Models.DTO.InvoiceDto;

public class InvoicePackDto
{
        public int Id { get; set; }
        public int InvoiceId { get; set; }
        public int ProductId { get; set; }
        public decimal BaseAmount { get; set; }
        public decimal Percentage { get; set; }
        public int CountDays { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Status { get; set; }
        public InvoiceDto Invoice { get; set; }
}