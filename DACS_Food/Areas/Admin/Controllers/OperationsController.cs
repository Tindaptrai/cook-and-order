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
    public class OperationsController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IOrderService _orderService;
        private readonly ITableService _tableService;

        public OperationsController(ApplicationDbContext db, IOrderService orderService, ITableService tableService)
        {
            _db = db;
            _orderService = orderService;
            _tableService = tableService;
        }

        [HttpGet("/admin/trung-tam-xu-ly")]
        public async Task<IActionResult> Index()
        {
            var today = DateTime.Today;
            var tomorrow = today.AddDays(1);

            var orders = await _db.Orders
                .AsNoTracking()
                .Include(x => x.Shipment)
                .Include(x => x.Items)
                .OrderByDescending(x => x.CreatedAt)
                .Take(80)
                .ToListAsync();

            var reservations = await _db.TableReservations
                .AsNoTracking()
                .Include(x => x.RestaurantTable)
                .Where(x => x.Status == ReservationStatus.Pending
                    || x.Status == ReservationStatus.Confirmed
                    || x.StartAt >= today.AddDays(-1))
                .OrderBy(x => x.StartAt)
                .Take(80)
                .ToListAsync();

            var model = new AdminOperationsViewModel
            {
                Orders = orders,
                Reservations = reservations,
                PendingOrders = orders.Count(x => x.OrderStatus == OrderStatus.Pending || x.OrderStatus == OrderStatus.New),
                DeliveringOrders = orders.Count(x => x.OrderStatus == OrderStatus.Delivering || x.DeliveryStatus == DeliveryStatus.Delivering),
                PendingReservations = reservations.Count(x => x.Status == ReservationStatus.Pending),
                TodayReservations = reservations.Count(x => x.StartAt >= today && x.StartAt < tomorrow)
            };

            return View(model);
        }

        [HttpPost("/admin/trung-tam-xu-ly/don-hang")]
        public async Task<IActionResult> UpdateOrder(int id, OrderStatus status)
        {
            var result = await _orderService.UpdateStatusAsync(id, status, User.Identity?.Name ?? "Admin");
            TempData[result.Success ? "OperationsMessage" : "OperationsError"] = result.Message;
            return RedirectToAction(nameof(Index));
        }

        [HttpPost("/admin/trung-tam-xu-ly/dat-ban")]
        public async Task<IActionResult> UpdateReservation(int id, ReservationStatus status)
        {
            var result = await _tableService.UpdateReservationStatusAsync(id, status);
            TempData[result.Success ? "OperationsMessage" : "OperationsError"] = result.Message;
            return RedirectToAction(nameof(Index));
        }
    }
}
