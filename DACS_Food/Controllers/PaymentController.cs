using DACS_Food.Services;
using Microsoft.AspNetCore.Mvc;

namespace DACS_Food.Controllers
{
    public class PaymentController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly IPaymentService _paymentService;
        private readonly IQrPaymentService _qrPaymentService;

        public PaymentController(IOrderService orderService, IPaymentService paymentService, IQrPaymentService qrPaymentService)
        {
            _orderService = orderService;
            _paymentService = paymentService;
            _qrPaymentService = qrPaymentService;
        }

        [HttpGet("/payment/qr/{orderCode}")]
        public async Task<IActionResult> Qr(string orderCode)
        {
            var order = await _orderService.GetByCodeAsync(orderCode);
            if (order == null) return NotFound();
            if (order.PaymentMethod != Models.PaymentMethod.QR)
            {
                return RedirectToAction("Success", "Order", new { code = orderCode });
            }
            ViewBag.QrPayment = _qrPaymentService.CreateQrInfo(order);
            return View(order);
        }

        [HttpPost("/payment/demo-confirm")]
        public async Task<IActionResult> DemoConfirm(string orderCode)
        {
            await _paymentService.ConfirmDemoPaymentAsync(orderCode);
            return RedirectToAction("Success", "Order", new { code = orderCode });
        }
    }
}
