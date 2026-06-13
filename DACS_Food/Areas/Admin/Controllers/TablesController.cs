using DACS_Food.Data;
using DACS_Food.Models;
using DACS_Food.Services;
using DACS_Food.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DACS_Food.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class TablesController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly ITableService _tableService;

        public TablesController(ApplicationDbContext db, ITableService tableService)
        {
            _db = db;
            _tableService = tableService;
        }

        [HttpGet("/admin/ban")]
        public async Task<IActionResult> Index(DateOnly? date)
        {
            var selectedDate = date ?? DateOnly.FromDateTime(DateTime.Today);
            var dayStart = selectedDate.ToDateTime(TimeOnly.MinValue);
            var nextDay = dayStart.AddDays(1);
            var tables = await _tableService.GetActiveTablesAsync();
            var reservations = await _db.TableReservations
                .Include(x => x.RestaurantTable)
                .Where(x => x.Status == ReservationStatus.Pending
                    || x.Status == ReservationStatus.Confirmed
                    || (x.StartAt >= dayStart && x.StartAt < nextDay))
                .OrderBy(x => x.StartAt)
                .Take(80)
                .ToListAsync();

            var bookedTableIds = reservations
                .Where(x => x.StartAt >= dayStart
                    && x.StartAt < nextDay
                    && x.Status == ReservationStatus.Confirmed)
                .Select(x => x.RestaurantTableId)
                .Distinct()
                .ToList();

            return View(new AdminTablesViewModel
            {
                SelectedDate = selectedDate,
                Tables = tables,
                BookedTableIds = bookedTableIds,
                Reservations = reservations,
                SuggestedFoodItems = await _tableService.GetReservationFoodChoicesAsync(),
                PendingReservations = reservations.Count(x => x.Status == ReservationStatus.Pending),
                ConfirmedReservations = reservations.Count(x => x.Status == ReservationStatus.Confirmed)
            });
        }

        [HttpPost("/admin/ban/status")]
        public async Task<IActionResult> UpdateStatus(int id, TableStatus status)
        {
            await _tableService.UpdateStatusAsync(id, status);
            return RedirectToAction(nameof(Index));
        }

        [HttpGet("/admin/ban/khung-gio")]
        public async Task<IActionResult> GetTimeSlots(DateOnly date, int tableId)
        {
            var slots = await _tableService.GetReservationSlotsAsync(date, tableId);
            return Json(slots);
        }

        [HttpPost("/admin/ban/dat-ban")]
        public async Task<IActionResult> CreateReservation(CreateTableReservationViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["AdminTableError"] = string.Join(" ", ModelState.Values
                    .SelectMany(x => x.Errors)
                    .Select(x => x.ErrorMessage));
                return RedirectToAction(nameof(Index));
            }

            var result = await _tableService.CreateReservationAsync(model);
            TempData[result.Success ? "AdminTableMessage" : "AdminTableError"] = result.Message;
            return RedirectToAction(nameof(Index));
        }

        [HttpPost("/admin/ban/dat-ban/status")]
        public async Task<IActionResult> UpdateReservationStatus(int id, ReservationStatus status)
        {
            var result = await _tableService.UpdateReservationStatusAsync(id, status);
            TempData[result.Success ? "AdminTableMessage" : "AdminTableError"] = result.Message;
            return RedirectToAction(nameof(Index));
        }
    }
}
