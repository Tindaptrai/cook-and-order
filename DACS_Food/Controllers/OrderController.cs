using System.Security.Claims;
using DACS_Food.Services;
using DACS_Food.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace DACS_Food.Controllers
{
    public class OrderController : Controller
    {
        private readonly ICartService _cartService;
        private readonly ITableService _tableService;
        private readonly IOrderService _orderService;

        public OrderController(ICartService cartService, ITableService tableService, IOrderService orderService)
        {
            _cartService = cartService;
            _tableService = tableService;
            _orderService = orderService;
        }

        [HttpGet("/checkout")]
        public async Task<IActionResult> Checkout()
        {
            var model = new CheckoutViewModel
            {
                Cart = await _cartService.GetCartViewModelAsync(GetUserId(), this.GetSessionId()),
                Tables = await _tableService.GetActiveTablesAsync()
            };
            return View(model);
        }

        [HttpPost("/orders/create")]
        public async Task<IActionResult> Create(CreateOrderViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["OrderError"] = string.Join(" ", ModelState.Values
                    .SelectMany(x => x.Errors)
                    .Select(x => x.ErrorMessage));
                return RedirectToAction(nameof(Checkout));
            }

            try
            {
                var order = await _orderService.CreateOrderAsync(GetUserId(), this.GetSessionId(), model);
                return RedirectToAction(nameof(Success), new { code = order.OrderCode });
            }
            catch (InvalidOperationException ex)
            {
                TempData["OrderError"] = ex.Message;
                return RedirectToAction(nameof(Checkout));
            }
        }

        [HttpGet("/orders/success/{code}")]
        public async Task<IActionResult> Success(string code)
        {
            var order = await _orderService.GetByCodeAsync(code);
            if (order == null) return NotFound();
            return View(order);
        }

        [HttpGet("/tra-cuu-don-hang")]
        [HttpGet("/order-tracking")]
        public async Task<IActionResult> Tracking()
        {
            var userId = GetUserId();
            if (!string.IsNullOrWhiteSpace(userId))
            {
                return View(new OrderTrackingViewModel
                {
                    IsAccountLookup = true,
                    Orders = await _orderService.GetByUserIdAsync(userId)
                });
            }

            return View(new OrderTrackingViewModel());
        }

        [HttpPost("/tra-cuu-don-hang")]
        [HttpPost("/order-tracking")]
        public async Task<IActionResult> Tracking(OrderTrackingViewModel model)
        {
            if (string.IsNullOrWhiteSpace(model.OrderCode))
            {
                var userId = GetUserId();
                if (!string.IsNullOrWhiteSpace(userId))
                {
                    model.IsAccountLookup = true;
                    model.Orders = await _orderService.GetByUserIdAsync(userId);
                    if (!model.Orders.Any())
                    {
                        model.Message = "Tài khoản của bạn chưa có đơn hàng nào.";
                    }

                    return View(model);
                }

                model.Message = "Vui lòng nhập mã đơn hàng.";
                return View(model);
            }

            model.Orders = await _orderService.TrackAsync(model.OrderCode);
            if (!model.Orders.Any())
            {
                model.Message = "Không tìm thấy đơn hàng phù hợp.";
            }

            return View(model);
        }

        private string? GetUserId()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier);
        }
    }
}

