namespace DACS_Food.Models
{
    public class Payment
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public PaymentMethod Method { get; set; }
        public decimal Amount { get; set; }
        public string QrContent { get; set; } = string.Empty;
        public string BankAccountName { get; set; } = string.Empty;
        public string BankAccountNumber { get; set; } = string.Empty;
        public string BankName { get; set; } = string.Empty;
        public PaymentStatus Status { get; set; } = PaymentStatus.Unpaid;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? PaidAt { get; set; }

        public Order? Order { get; set; }
    }
}
