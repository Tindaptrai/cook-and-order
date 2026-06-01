namespace DACS_Food.Models
{
    public class RestaurantTable
    {
        public int Id { get; set; }
        public int TableNumber { get; set; }
        public string Name { get; set; } = string.Empty;
        public TableType TableType { get; set; }
        public int Capacity { get; set; }
        public TableStatus Status { get; set; } = TableStatus.Available;
        public bool IsActive { get; set; } = true;

        public ICollection<Order> Orders { get; set; } = new List<Order>();
        public ICollection<TableReservation> Reservations { get; set; } = new List<TableReservation>();
    }
}
