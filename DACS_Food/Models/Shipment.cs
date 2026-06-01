namespace DACS_Food.Models
{
    public class Shipment
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public string ShipmentCode { get; set; } = string.Empty;
        public string OrderCode { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerPhone { get; set; } = string.Empty;
        public string DeliveryAddress { get; set; } = string.Empty;
        public string ShipperName { get; set; } = string.Empty;
        public string ShipperPhone { get; set; } = string.Empty;
        public string DeliveryNote { get; set; } = string.Empty;
        public DeliveryStatus DeliveryStatus { get; set; } = DeliveryStatus.Pending;
        public DateTime? DeliveryStartedAt { get; set; }
        public DateTime? DeliveredAt { get; set; }
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public Order? Order { get; set; }
    }
}
