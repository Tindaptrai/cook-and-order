using DACS_Food.Data;
using DACS_Food.Models;
using DACS_Food.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DACS_Food.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class OrdersController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IOrderService _orderService;

        public OrdersController(ApplicationDbContext db, IOrderService orderService)
        {
            _db = db;
            _orderService = orderService;
        }

        [HttpGet("/admin/don-hang")]
        public async Task<IActionResult> Index()
        {
            var orders = await _db.Orders
                .Include(x => x.Shipment)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();
            return View(orders);
        }

        [HttpGet("/admin/don-hang/{id:int}")]
        public async Task<IActionResult> Detail(int id)
        {
            var order = await _orderService.GetByIdAsync(id);
            if (order == null) return NotFound();
            return View(order);
        }

        [HttpPost("/admin/don-hang/status")]
        public async Task<IActionResult> UpdateStatus(int id, OrderStatus status)
        {
            var result = await _orderService.UpdateStatusAsync(id, status, User.Identity?.Name ?? "Admin");
            TempData[result.Success ? "AdminOrderMessage" : "AdminOrderError"] = result.Message;
            return Redirect(Request.Headers.Referer.ToString() ?? "/admin/don-hang");
        }

        [HttpPost("/admin/don-hang/delivery")]
        public async Task<IActionResult> UpdateDelivery(int id, DeliveryStatus deliveryStatus, string? shipperName, string? shipperPhone, string? deliveryNote)
        {
            var result = await _orderService.UpdateDeliveryAsync(id, deliveryStatus, shipperName, shipperPhone, deliveryNote, User.Identity?.Name ?? "Admin");
            TempData[result.Success ? "AdminOrderMessage" : "AdminOrderError"] = result.Message;
            return Redirect(Request.Headers.Referer.ToString() ?? "/admin/don-hang");
        }

        [HttpGet("/admin/don-hang/xac-nhan-giao-hang")]
        public IActionResult ConfirmDelivered()
        {
            return View();
        }

        [HttpPost("/admin/don-hang/xac-nhan-giao-hang")]
        public async Task<IActionResult> ConfirmDelivered(string orderCode)
        {
            var result = await _orderService.ConfirmDeliveredAsync(orderCode, User.Identity?.Name ?? "Nhân viên");
            TempData[result.Success ? "AdminOrderMessage" : "AdminOrderError"] = result.Message;
            if (result.Success && result.Order != null)
            {
                return RedirectToAction(nameof(Detail), new { id = result.Order.Id });
            }

            return View();
        }
    }
}
