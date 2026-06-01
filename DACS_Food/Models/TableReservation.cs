namespace DACS_Food.Models
{
    public class TableReservation
    {
        public int Id { get; set; }
        public int RestaurantTableId { get; set; }
        public DateTime StartAt { get; set; }
        public int DurationMinutes { get; set; } = 90;
        public string CustomerName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string? Note { get; set; }
        public ReservationStatus Status { get; set; } = ReservationStatus.Pending;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public RestaurantTable? RestaurantTable { get; set; }
    }
}
