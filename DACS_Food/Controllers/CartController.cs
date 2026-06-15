using System.Security.Claims;
using DACS_Food.Services;
using DACS_Food.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace DACS_Food.Controllers
{
    public class CartController : Controller
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        [HttpGet("/cart")]
        public async Task<IActionResult> Index()
        {
            var model = await _cartService.GetCartViewModelAsync(GetUserId(), this.GetSessionId());
            return View(model);
        }

        [HttpPost("/cart/add")]
        public async Task<IActionResult> Add(AddCartItemViewModel model)
        {
            var added = await _cartService.AddAsync(GetUserId(), this.GetSessionId(), model.FoodItemId, model.Quantity);
            if (!added)
            {
                TempData["CartError"] = "Món ăn không tồn tại hoặc đang tạm hết.";
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost("/cart/update")]
        public async Task<IActionResult> Update(int cartItemId, int quantity)
        {
            await _cartService.UpdateAsync(GetUserId(), this.GetSessionId(), cartItemId, quantity);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost("/cart/remove")]
        public async Task<IActionResult> Remove(int cartItemId)
        {
            await _cartService.RemoveAsync(GetUserId(), this.GetSessionId(), cartItemId);
            return RedirectToAction(nameof(Index));
        }

        private string? GetUserId()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier);
        }
    }
}
