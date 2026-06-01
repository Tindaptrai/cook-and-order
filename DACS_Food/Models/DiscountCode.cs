namespace DACS_Food.Models
{
    public class DiscountCode
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public DiscountType DiscountType { get; set; }
        public decimal DiscountValue { get; set; }
        public decimal MinOrderAmount { get; set; }
        public decimal? MaxDiscountAmount { get; set; }
        public int? UsageLimit { get; set; }
        public int UsedCount { get; set; }
        public DiscountScope DiscountScope { get; set; } = DiscountScope.Manual;
        public string? RequiredLoyaltyLevel { get; set; }
        public DateTime? StartAt { get; set; }
        public DateTime? EndAt { get; set; }
        public bool IsActive { get; set; } = true;

        public ICollection<DiscountUsage> Usages { get; set; } = new List<DiscountUsage>();
        public ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}
