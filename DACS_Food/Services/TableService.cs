using DACS_Food.Data;
using DACS_Food.Models;
using DACS_Food.ViewModels;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace DACS_Food.Services
{
    public class TableService : ITableService
    {
        private readonly ApplicationDbContext _db;

        public TableService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IReadOnlyList<RestaurantTable>> GetActiveTablesAsync()
        {
            return await _db.RestaurantTables
                .Where(x => x.IsActive)
                .OrderBy(x => x.TableNumber)
                .ToListAsync();
        }

        public async Task<bool> UpdateStatusAsync(int tableId, TableStatus status)
        {
            var table = await _db.RestaurantTables.FindAsync(tableId);
            if (table == null) return false;

            table.Status = status == TableStatus.Reserved ? TableStatus.Available : status;
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<TablePageViewModel> GetTablePageAsync(DateOnly? date = null)
        {
            var selectedDate = date ?? DateOnly.FromDateTime(DateTime.Now);
            var tables = await GetActiveTablesAsync();
            var dayStart = selectedDate.ToDateTime(TimeOnly.MinValue);
            var nextDay = dayStart.AddDays(1);
            var today = DateOnly.FromDateTime(DateTime.Now);
            var now = DateTime.Now;
            var reservationQuery = _db.TableReservations
                .Where(x => x.Status == ReservationStatus.Confirmed);

            reservationQuery = selectedDate == today
                ? reservationQuery.Where(x => x.StartAt <= now && x.StartAt.AddMinutes(x.DurationMinutes) > now)
                : reservationQuery.Where(x => x.StartAt >= dayStart && x.StartAt < nextDay);

            var busyTableIds = await reservationQuery
                    .Select(x => x.RestaurantTableId)
                    .Distinct()
                    .ToListAsync();

            var reservations = await _db.TableReservations
                .Include(x => x.RestaurantTable)
                .Where(x => x.Status == ReservationStatus.Pending || x.Status == ReservationStatus.Confirmed)
                .OrderBy(x => x.StartAt)
                .Take(30)
                .ToListAsync();

            return new TablePageViewModel
            {
                SelectedDate = selectedDate,
                Tables = tables.Select(x => new PublicTableViewModel
                {
                    Id = x.Id,
                    TableNumber = x.TableNumber,
                    Name = x.Name,
                    TableType = x.TableType,
                    Capacity = x.Capacity,
                    IsBusy = x.Status == TableStatus.Occupied || busyTableIds.Contains(x.Id)
                }).ToList(),
                Reservations = reservations
            };
        }

        public async Task<IReadOnlyList<ReservationSlotViewModel>> GetReservationSlotsAsync(DateOnly date, int tableId)
        {
            var dayStart = date.ToDateTime(TimeOnly.MinValue);
            var nextDay = dayStart.AddDays(1);
            var activeReservations = await _db.TableReservations
                .Where(x => x.RestaurantTableId == tableId
                    && x.Status == ReservationStatus.Confirmed
                    && x.StartAt >= dayStart
                    && x.StartAt < nextDay)
                .ToListAsync();

            var slots = new List<ReservationSlotViewModel>();
            var firstSlot = dayStart.AddHours(8).AddMinutes(30);
            var lastSlot = dayStart.AddHours(21);

            for (var slot = firstSlot; slot <= lastSlot; slot = slot.AddMinutes(30))
            {
                var slotEnd = slot.AddMinutes(90);
                var isBlocked = activeReservations.Any(x =>
                {
                    var reservationEnd = x.StartAt.AddMinutes(x.DurationMinutes);
                    return slot < reservationEnd && slotEnd > x.StartAt;
                });

                slots.Add(new ReservationSlotViewModel
                {
                    Time = slot.ToString("HH:mm"),
                    IsAvailable = !isBlocked
                });
            }

            return slots;
        }

        public async Task<(bool Success, string Message)> CreateReservationAsync(CreateTableReservationViewModel model)
        {
            if (!TimeOnly.TryParseExact(model.TimeSlot, "HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out var time))
            {
                return (false, "Khung giờ không hợp lệ.");
            }

            var table = await _db.RestaurantTables.FirstOrDefaultAsync(x => x.Id == model.TableId && x.IsActive);
            if (table == null)
            {
                return (false, "Không tìm thấy bàn.");
            }

            var startAt = model.ReservationDate.ToDateTime(time);
            if (startAt < DateTime.Now.AddMinutes(30))
            {
                return (false, "Vui lòng đặt bàn trước ít nhất 30 phút để quán kịp chuẩn bị.");
            }

            if (model.ReservationDate > DateOnly.FromDateTime(DateTime.Now.AddDays(30)))
            {
                return (false, "FoodieTTTM chỉ nhận lịch đặt trong vòng 30 ngày tới.");
            }

            var slots = await GetReservationSlotsAsync(model.ReservationDate, model.TableId);
            if (!slots.Any(x => x.Time == model.TimeSlot && x.IsAvailable))
            {
                return (false, "Khung giờ này đã có lịch được xác nhận cho bàn đã chọn. Vui lòng chọn giờ khác.");
            }

            if (time < new TimeOnly(8, 30) || time > new TimeOnly(21, 0))
            {
                return (false, "FoodieTTTM chỉ nhận đặt bàn từ 08:30 đến 21:00 để quán có đủ thời gian chuẩn bị và phục vụ trước giờ đóng cửa.");
            }

            _db.TableReservations.Add(new TableReservation
            {
                RestaurantTableId = model.TableId,
                StartAt = startAt,
                DurationMinutes = 90,
                CustomerName = model.CustomerName.Trim(),
                PhoneNumber = model.PhoneNumber.Trim(),
                Note = string.IsNullOrWhiteSpace(model.Note) ? null : model.Note.Trim(),
                Status = model.Status
            });

            await _db.SaveChangesAsync();
            return (true, $"Đã nhận yêu cầu đặt {table.Name} lúc {startAt:dd/MM/yyyy HH:mm}. Khung giờ chỉ được giữ sau khi admin xác nhận.");
        }

        public async Task<(bool Success, string Message)> UpdateReservationStatusAsync(int reservationId, ReservationStatus status)
        {
            var reservation = await _db.TableReservations
                .Include(x => x.RestaurantTable)
                .FirstOrDefaultAsync(x => x.Id == reservationId);
            if (reservation == null) return (false, "Không tìm thấy lịch đặt bàn.");

            if (status == ReservationStatus.Confirmed)
            {
                var startAt = reservation.StartAt;
                var endAt = reservation.StartAt.AddMinutes(reservation.DurationMinutes);
                var hasConflict = await _db.TableReservations.AnyAsync(x =>
                    x.Id != reservation.Id
                    && x.RestaurantTableId == reservation.RestaurantTableId
                    && x.Status == ReservationStatus.Confirmed
                    && startAt < x.StartAt.AddMinutes(x.DurationMinutes)
                    && endAt > x.StartAt);

                if (hasConflict)
                {
                    return (false, "Khung giờ này đã có lịch được xác nhận. Vui lòng chọn lịch khác hoặc hủy lịch cũ trước.");
                }
            }

            reservation.Status = status;

            await _db.SaveChangesAsync();
            return (true, "Đã cập nhật trạng thái đặt bàn.");
        }
    }
}

