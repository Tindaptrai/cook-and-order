using DACS_Food.Data;
using DACS_Food.Models;
using DACS_Food.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DACS_Food.Controllers.Api
{
    [ApiController]
    [Authorize(Roles = "Admin")]
    [Route("api/admin")]
    public class AdminApiController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        private readonly IOrderService _orderService;

        public AdminApiController(ApplicationDbContext db, IOrderService orderService)
        {
            _db = db;
            _orderService = orderService;
        }

        [HttpGet("dashboard")]
        public async Task<IActionResult> GetDashboard()
        {
            var today = DateTime.UtcNow.Date;

            var totalOrders = await _db.Orders.CountAsync();
            var newOrders = await _db.Orders.CountAsync(x => x.OrderStatus == OrderStatus.Pending);
            var paidOrders = await _db.Orders.CountAsync(x => x.PaymentStatus == PaymentStatus.Paid);
            var todayOrders = await _db.Orders.CountAsync(x => x.CreatedAt >= today);
            var revenue = await _db.Orders
                .Where(x => x.OrderStatus != OrderStatus.Cancelled)
                .SumAsync(x => x.TotalAmount);
            var todayRevenue = await _db.Orders
                .Where(x => x.CreatedAt >= today && x.OrderStatus != OrderStatus.Cancelled)
                .SumAsync(x => x.TotalAmount);

            var recentOrders = await _db.Orders
                .AsNoTracking()
                .OrderByDescending(x => x.CreatedAt)
                .Take(6)
                .Select(x => new
                {
                    x.Id,
                    x.OrderCode,
                    x.CustomerName,
                    x.PhoneNumber,
                    x.TotalAmount,
                    orderStatus = x.OrderStatus.ToString(),
                    orderStatusText = OrderService.GetOrderStatusLabel(x.OrderStatus),
                    deliveryStatus = x.DeliveryStatus.ToString(),
                    deliveryStatusText = OrderService.GetDeliveryStatusLabel(x.DeliveryStatus),
                    x.TrackingCode,
                    paymentStatus = x.PaymentStatus.ToString(),
                    paymentMethod = x.PaymentMethod.ToString(),
                    x.CreatedAt
                })
                .ToListAsync();

            return Ok(new
            {
                totalOrders,
                newOrders,
                paidOrders,
                todayOrders,
                revenue,
                todayRevenue,
                totalFoods = await _db.FoodItems.CountAsync(x => x.IsActive),
                totalUsers = await _db.Users.CountAsync(),
                totalReservations = await _db.TableReservations.CountAsync(),
                recentOrders
            });
        }

        [HttpGet("orders")]
        public async Task<IActionResult> GetOrders([FromQuery] string? status, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            page = Math.Max(1, page);
            pageSize = Math.Clamp(pageSize, 1, 100);

            var query = _db.Orders
                .AsNoTracking()
                .Include(x => x.Items)
                .Include(x => x.Payment)
                .Include(x => x.Shipment)
                .OrderByDescending(x => x.CreatedAt)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(status) &&
                Enum.TryParse<OrderStatus>(status, true, out var parsedStatus))
            {
                query = query.Where(x => x.OrderStatus == parsedStatus);
            }

            var totalItems = await query.CountAsync();
            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new
                {
                    x.Id,
                    x.OrderCode,
                    x.CustomerName,
                    x.PhoneNumber,
                    x.Address,
                    x.TrackingCode,
                    deliveryStatus = x.DeliveryStatus.ToString(),
                    deliveryStatusText = OrderService.GetDeliveryStatusLabel(x.DeliveryStatus),
                    x.ShipperName,
                    x.ShipperPhone,
                    x.DeliveryNote,
                    x.ShippedAt,
                    x.DeliveredAt,
                    x.Subtotal,
                    x.DiscountAmount,
                    x.TotalAmount,
                    orderStatus = x.OrderStatus.ToString(),
                    orderStatusText = OrderService.GetOrderStatusLabel(x.OrderStatus),
                    paymentStatus = x.PaymentStatus.ToString(),
                    paymentMethod = x.PaymentMethod.ToString(),
                    x.CreatedAt,
                    itemCount = x.Items.Sum(i => i.Quantity),
                    items = x.Items.Select(i => new
                    {
                        i.FoodName,
                        i.Quantity,
                        i.UnitPrice,
                        i.LineTotal
                    })
                })
                .ToListAsync();

            return Ok(new
            {
                page,
                pageSize,
                totalItems,
                totalPages = Math.Max(1, (int)Math.Ceiling(totalItems / (double)pageSize)),
                items
            });
        }

        [HttpPut("orders/{id:int}/status")]
        public async Task<IActionResult> UpdateOrderStatus(int id, [FromBody] UpdateOrderStatusRequest request)
        {
            if (!Enum.TryParse<OrderStatus>(request.Status, true, out var status))
            {
                return BadRequest(new { message = "Trạng thái đơn hàng không hợp lệ." });
            }

            var result = await _orderService.UpdateStatusAsync(id, status, User.Identity?.Name ?? "Admin API");
            if (!result.Success)
            {
                return BadRequest(new { message = result.Message });
            }

            return Ok(new
            {
                message = result.Message,
                orderId = result.Order!.Id,
                orderStatus = result.Order.OrderStatus.ToString(),
                orderStatusText = OrderService.GetOrderStatusLabel(result.Order.OrderStatus)
            });
        }

        [HttpPut("orders/{id:int}/delivery")]
        public async Task<IActionResult> UpdateOrderDelivery(int id, [FromBody] UpdateOrderDeliveryRequest request)
        {
            if (!Enum.TryParse<DeliveryStatus>(request.DeliveryStatus, true, out var deliveryStatus))
            {
                return BadRequest(new { message = "Trạng thái vận đơn không hợp lệ." });
            }

            var result = await _orderService.UpdateDeliveryAsync(id, deliveryStatus, request.ShipperName, request.ShipperPhone, request.DeliveryNote, User.Identity?.Name ?? "Admin API");
            if (!result.Success)
            {
                return BadRequest(new { message = result.Message });
            }

            return Ok(new
            {
                message = result.Message,
                orderId = result.Order!.Id,
                result.Order.TrackingCode,
                deliveryStatus = result.Order.DeliveryStatus.ToString(),
                deliveryStatusText = OrderService.GetDeliveryStatusLabel(result.Order.DeliveryStatus)
            });
        }

        [HttpPost("orders/confirm-delivered")]
        public async Task<IActionResult> ConfirmDelivered([FromBody] ConfirmDeliveredRequest request)
        {
            var result = await _orderService.ConfirmDeliveredAsync(request.OrderCode, User.Identity?.Name ?? "Nhân viên");
            if (!result.Success)
            {
                return BadRequest(new { message = result.Message });
            }

            return Ok(new
            {
                message = result.Message,
                orderId = result.Order!.Id,
                orderCode = result.Order.OrderCode,
                orderStatus = result.Order.OrderStatus.ToString(),
                deliveryStatus = result.Order.DeliveryStatus.ToString()
            });
        }

        public class UpdateOrderStatusRequest
        {
            public string Status { get; set; } = string.Empty;
        }

        public class UpdateOrderDeliveryRequest
        {
            public string DeliveryStatus { get; set; } = string.Empty;
            public string? ShipperName { get; set; }
            public string? ShipperPhone { get; set; }
            public string? DeliveryNote { get; set; }
        }

        public class ConfirmDeliveredRequest
        {
            public string OrderCode { get; set; } = string.Empty;
        }
    }
}
