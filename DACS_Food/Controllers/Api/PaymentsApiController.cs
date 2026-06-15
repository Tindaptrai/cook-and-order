using System.Security.Claims;
using DACS_Food.Controllers;
using DACS_Food.Models;
using DACS_Food.Services;
using Microsoft.AspNetCore.Mvc;

namespace DACS_Food.Controllers.Api
{
    [ApiController]
    [Route("api/payments")]
    public class PaymentsApiController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly IPaymentService _paymentService;

        public PaymentsApiController(IOrderService orderService, IPaymentService paymentService)
        {
            _orderService = orderService;
            _paymentService = paymentService;
        }

        [HttpPost("demo-confirm")]
        public async Task<IActionResult> DemoConfirm([FromBody] DemoConfirmPaymentRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.OrderCode))
            {
                return BadRequest(new { message = "Thiếu mã đơn hàng." });
            }

            var order = await _orderService.GetByCodeAsync(request.OrderCode);
            if (order == null || !CanViewOrder(order))
            {
                return NotFound(new { message = "Không tìm thấy đơn hàng hoặc thanh toán." });
            }

            var success = await _paymentService.ConfirmDemoPaymentAsync(request.OrderCode);
            if (!success)
            {
                return NotFound(new { message = "Không tìm thấy đơn hàng hoặc thanh toán." });
            }

            return Ok(new
            {
                message = "Đã xác nhận thanh toán demo.",
                orderCode = request.OrderCode,
                redirectUrl = $"/orders/success/{request.OrderCode}"
            });
        }

        public class DemoConfirmPaymentRequest
        {
            public string OrderCode { get; set; } = string.Empty;
        }

        private string? GetUserId()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier);
        }

        private bool CanViewOrder(Order order)
        {
            var userId = GetUserId();
            return User.IsInRole("Admin")
                || (!string.IsNullOrWhiteSpace(userId) && order.UserId == userId)
                || this.HasRecentOrderCode(order.OrderCode);
        }
    }
}
