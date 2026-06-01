namespace DACS_Food.Models
{
    public class DiscountUsage
    {
        public int Id { get; set; }
        public int DiscountCodeId { get; set; }
        public string? UserId { get; set; }
        public int? OrderId { get; set; }
        public DateTime UsedAt { get; set; } = DateTime.UtcNow;

        public DiscountCode? DiscountCode { get; set; }
        public ApplicationUser? User { get; set; }
        public Order? Order { get; set; }
    }
}
