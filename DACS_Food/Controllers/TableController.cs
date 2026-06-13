using DACS_Food.Services;
using DACS_Food.Models;
using DACS_Food.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DACS_Food.Controllers
{
    public class TableController : Controller
    {
        private readonly ITableService _tableService;
        private readonly IReservationRateLimitService _reservationRateLimitService;

        public TableController(ITableService tableService, IReservationRateLimitService reservationRateLimitService)
        {
            _tableService = tableService;
            _reservationRateLimitService = reservationRateLimitService;
        }

        [HttpGet("/ban")]
        public async Task<IActionResult> Index()
        {
            var model = await _tableService.GetTablePageAsync();
            return View(model);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("/ban/khung-gio")]
        public async Task<IActionResult> GetTimeSlots(DateOnly date, int tableId)
        {
            var slots = await _tableService.GetReservationSlotsAsync(date, tableId);
            return Json(slots);
        }

        [HttpPost("/ban/dat-ban")]
        public async Task<IActionResult> CreateReservation(CreateTableReservationViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["TableMessage"] = string.Join(" ", ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage));
                return RedirectToAction(nameof(Index));
            }

            if (!User.IsInRole("Admin"))
            {
                var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
                var limit = _reservationRateLimitService.CanCreateReservation(ipAddress, model.PhoneNumber);
                if (!limit.Allowed)
                {
                    TempData["TableMessage"] = limit.Message;
                    return RedirectToAction(nameof(Index));
                }
            }

            var result = await _tableService.CreateReservationAsync(model);
            if (result.Success && !User.IsInRole("Admin"))
            {
                var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
                _reservationRateLimitService.RecordCreateReservation(ipAddress, model.PhoneNumber);
            }

            TempData["TableMessage"] = User.IsInRole("Admin")
                ? result.Message
                : "FoodieTTTM đã nhận thông tin đặt bàn của bạn. Nhân viên nhà hàng sẽ liên hệ lại ngay khi kiểm tra được thông tin để xác nhận bàn, thời gian và các lưu ý cần thiết.";
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("/ban/cap-nhat-dat-ban")]
        public async Task<IActionResult> UpdateReservationStatus(int id, ReservationStatus status)
        {
            await _tableService.UpdateReservationStatusAsync(id, status);
            TempData["TableMessage"] = status == ReservationStatus.Completed || status == ReservationStatus.Cancelled
                ? "Đã cập nhật trạng thái. Khung giờ của lượt đặt này không còn khóa lịch."
                : "Đã cập nhật trạng thái. Khung giờ của lượt đặt này đang được giữ.";
            return RedirectToAction(nameof(Index));
        }
    }
}

