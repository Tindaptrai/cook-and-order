using System.Security.Claims;
using DACS_Food.Models;
using DACS_Food.Services;
using DACS_Food.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace DACS_Food.Controllers.Api
{
    [ApiController]
    [Route("api/orders")]
    public class OrdersApiController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrdersApiController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpGet("{orderCode}")]
        public async Task<IActionResult> GetOrder(string orderCode)
        {
            var order = await _orderService.GetByCodeAsync(orderCode);
            if (order == null) return NotFound(new { message = "Không tìm thấy đơn hàng." });
            return Ok(MapOrder(order));
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetOrderById(int id)
        {
            var order = await _orderService.GetByIdAsync(id);
            if (order == null) return NotFound(new { message = "Không tìm thấy đơn hàng." });
            return Ok(MapOrder(order));
        }

        [HttpGet("track")]
        public async Task<IActionResult> Track([FromQuery] string? orderCode, [FromQuery] string? phone)
        {
            if (string.IsNullOrWhiteSpace(orderCode) && string.IsNullOrWhiteSpace(phone))
            {
                var userId = GetUserId();
                if (string.IsNullOrWhiteSpace(userId))
                {
                    return BadRequest(new { message = "Vui lòng nhập mã đơn hàng hoặc số điện thoại." });
                }

                var accountOrders = await _orderService.GetByUserIdAsync(userId);
                return Ok(new
                {
                    items = accountOrders.Select(MapTrackingOrder),
                    lookupMode = "account"
                });
            }

            var orders = await _orderService.TrackAsync(orderCode, phone);
            if (!orders.Any())
            {
                return NotFound(new { message = "Không tìm thấy đơn hàng phù hợp." });
            }

            return Ok(new
            {
                items = orders.Select(MapTrackingOrder)
            });
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var message = string.Join(" ", ModelState.Values
                    .SelectMany(x => x.Errors)
                    .Select(x => x.ErrorMessage));
                return BadRequest(new { message });
            }

            try
            {
                var order = await _orderService.CreateOrderAsync(GetUserId(), GetSessionId(), model);
                var paymentUrl = order.PaymentMethod == PaymentMethod.QR ? $"/payment/qr/{order.OrderCode}" : null;

                return Ok(new
                {
                    message = "Đã tạo đơn hàng thành công.",
                    orderCode = order.OrderCode,
                    redirectUrl = paymentUrl ?? $"/orders/success/{order.OrderCode}",
                    paymentUrl,
                    order = MapOrder(order)
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        private string? GetUserId()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier);
        }

        private string GetSessionId()
        {
            const string key = "FoodieTTTMSessionId";
            var sessionId = HttpContext.Session.GetString(key);
            if (!string.IsNullOrWhiteSpace(sessionId))
            {
                return sessionId;
            }

            sessionId = Guid.NewGuid().ToString("N");
            HttpContext.Session.SetString(key, sessionId);
            return sessionId;
        }

        private static object MapOrder(Order order)
        {
            return new
            {
                id = order.Id,
                orderCode = order.OrderCode,
                customerName = order.CustomerName,
                phoneNumber = order.PhoneNumber,
                address = order.Address,
                orderType = order.OrderType.ToString(),
                trackingCode = order.TrackingCode,
                deliveryStatus = order.DeliveryStatus.ToString(),
                deliveryStatusText = OrderService.GetDeliveryStatusLabel(order.DeliveryStatus),
                shipperName = order.ShipperName,
                shipperPhone = order.ShipperPhone,
                deliveryNote = order.DeliveryNote,
                order.ShippedAt,
                order.DeliveredAt,
                subtotal = order.Subtotal,
                discountAmount = order.DiscountAmount,
                totalAmount = order.TotalAmount,
                paymentMethod = order.PaymentMethod.ToString(),
                paymentStatus = order.PaymentStatus.ToString(),
                orderStatus = order.OrderStatus.ToString(),
                orderStatusText = OrderService.GetOrderStatusLabel(order.OrderStatus),
                createdAt = order.CreatedAt,
                items = order.Items.Select(x => new
                {
                    x.FoodItemId,
                    x.FoodName,
                    x.UnitPrice,
                    x.Quantity,
                    x.LineTotal
                }),
                payment = order.Payment == null ? null : new
                {
                    method = order.Payment.Method.ToString(),
                    order.Payment.Amount,
                    order.Payment.QrContent,
                    order.Payment.BankName,
                    order.Payment.BankAccountName,
                    order.Payment.BankAccountNumber,
                    status = order.Payment.Status.ToString(),
                    order.Payment.PaidAt
                }
            };
        }

        private static object MapTrackingOrder(Order order)
        {
            return new
            {
                order.Id,
                order.OrderCode,
                orderStatus = order.OrderStatus.ToString(),
                orderStatusText = OrderService.GetOrderStatusLabel(order.OrderStatus),
                deliveryStatus = order.DeliveryStatus.ToString(),
                deliveryStatusText = OrderService.GetDeliveryStatusLabel(order.DeliveryStatus),
                order.TotalAmount,
                paymentMethod = order.PaymentMethod.ToString(),
                order.CreatedAt,
                address = MaskAddress(order.Address),
                items = order.Items.Select(x => new { x.FoodName, x.Quantity })
            };
        }

        private static string MaskAddress(string address)
        {
            if (string.IsNullOrWhiteSpace(address)) return string.Empty;
            var parts = address.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length >= 2) return string.Join(", ", parts.TakeLast(2));
            return address.Length <= 8 ? "Đã ẩn" : $"{address[..Math.Min(8, address.Length)]}...";
        }
    }
}


