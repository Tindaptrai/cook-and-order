using DACS_Food.Services;
using Microsoft.AspNetCore.Mvc;

namespace DACS_Food.Controllers.Api
{
    [ApiController]
    [Route("api/payments")]
    public class PaymentsApiController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public PaymentsApiController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        [HttpPost("demo-confirm")]
        public async Task<IActionResult> DemoConfirm([FromBody] DemoConfirmPaymentRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.OrderCode))
            {
                return BadRequest(new { message = "Thiếu mã đơn hàng." });
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
    }
}
