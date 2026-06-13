using DACS_Food.Models;
using System.ComponentModel.DataAnnotations;

namespace DACS_Food.ViewModels
{
    public class TablePageViewModel
    {
        public DateOnly SelectedDate { get; set; } = DateOnly.FromDateTime(DateTime.Now);
        public IReadOnlyList<PublicTableViewModel> Tables { get; set; } = Array.Empty<PublicTableViewModel>();
        public IReadOnlyList<TableReservation> Reservations { get; set; } = Array.Empty<TableReservation>();
        public IReadOnlyList<ReservationFoodChoiceViewModel> SuggestedFoodItems { get; set; } = Array.Empty<ReservationFoodChoiceViewModel>();
    }

    public class AdminTablesViewModel
    {
        public DateOnly SelectedDate { get; set; } = DateOnly.FromDateTime(DateTime.Now);
        public IReadOnlyList<RestaurantTable> Tables { get; set; } = Array.Empty<RestaurantTable>();
        public IReadOnlyList<int> BookedTableIds { get; set; } = Array.Empty<int>();
        public IReadOnlyList<TableReservation> Reservations { get; set; } = Array.Empty<TableReservation>();
        public IReadOnlyList<ReservationFoodChoiceViewModel> SuggestedFoodItems { get; set; } = Array.Empty<ReservationFoodChoiceViewModel>();
        public int PendingReservations { get; set; }
        public int ConfirmedReservations { get; set; }
    }

    public class PublicTableViewModel
    {
        public int Id { get; set; }
        public int TableNumber { get; set; }
        public string Name { get; set; } = string.Empty;
        public TableType TableType { get; set; }
        public int Capacity { get; set; }
        public bool IsBusy { get; set; }
    }

    public class ReservationSlotViewModel
    {
        public string Time { get; set; } = string.Empty;
        public bool IsAvailable { get; set; }
        public string Label => IsAvailable ? Time : $"{Time} - bận";
    }

    public class ReservationFoodChoiceViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Tag { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public bool IsBestSeller { get; set; }
    }

    public class CreateTableReservationViewModel
    {
        [Required(ErrorMessage = "Vui lòng chọn ngày đặt bàn.")]
        public DateOnly ReservationDate { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn bàn.")]
        public int TableId { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn khung giờ.")]
        public string TimeSlot { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập tên khách.")]
        [StringLength(80, ErrorMessage = "Tên khách tối đa 80 ký tự.")]
        public string CustomerName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập số điện thoại.")]
        [RegularExpression(@"^[0-9+\-\s]{8,16}$", ErrorMessage = "Số điện thoại không hợp lệ.")]
        public string PhoneNumber { get; set; } = string.Empty;

        [StringLength(300, ErrorMessage = "Ghi chú tối đa 300 ký tự.")]
        public string? Note { get; set; }
        public List<int> SelectedFoodItemIds { get; set; } = new();
        public ReservationStatus Status { get; set; } = ReservationStatus.Pending;
    }
}
