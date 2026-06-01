namespace DACS_Food.Models
{
    public class Order
    {
        public int Id { get; set; }
        public string OrderCode { get; set; } = string.Empty;
        public string? UserId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public OrderType OrderType { get; set; } = OrderType.Delivery;
        public int? TableId { get; set; }
        public decimal Subtotal { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Unpaid;
        public OrderStatus OrderStatus { get; set; } = OrderStatus.New;
        public string TrackingCode { get; set; } = string.Empty;
        public DeliveryStatus DeliveryStatus { get; set; } = DeliveryStatus.Pending;
        public string ShipperName { get; set; } = string.Empty;
        public string ShipperPhone { get; set; } = string.Empty;
        public string DeliveryNote { get; set; } = string.Empty;
        public DateTime? ShippedAt { get; set; }
        public DateTime? DeliveredAt { get; set; }
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public int? DiscountCodeId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ApplicationUser? User { get; set; }
        public RestaurantTable? Table { get; set; }
        public DiscountCode? DiscountCode { get; set; }
        public Payment? Payment { get; set; }
        public Shipment? Shipment { get; set; }
        public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
        public ICollection<OrderStatusHistory> StatusHistories { get; set; } = new List<OrderStatusHistory>();
    }
}
