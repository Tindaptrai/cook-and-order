using DACS_Food.Models;

namespace DACS_Food.ViewModels
{
    public class AdminOperationsViewModel
    {
        public IReadOnlyList<Order> Orders { get; set; } = Array.Empty<Order>();
        public IReadOnlyList<TableReservation> Reservations { get; set; } = Array.Empty<TableReservation>();
        public int PendingOrders { get; set; }
        public int DeliveringOrders { get; set; }
        public int PendingReservations { get; set; }
        public int TodayReservations { get; set; }
    }
}
