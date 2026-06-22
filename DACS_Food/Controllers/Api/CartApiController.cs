using System.Security.Claims;
using DACS_Food.Services;
using DACS_Food.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace DACS_Food.Controllers.Api
{
    [ApiController]
    [Route("api/cart")]
    public class CartApiController : ControllerBase
    {
        private readonly ICartService _cartService;

        public CartApiController(ICartService cartService)
        {
            _cartService = cartService;
        }

        [HttpGet]
        public async Task<IActionResult> GetCart()
        {
            var cart = await _cartService.GetCartViewModelAsync(GetUserId(), GetSessionId());
            return Ok(ToResponse(cart));
        }

        [HttpPost("items")]
        public async Task<IActionResult> AddItem([FromBody] AddCartItemViewModel model)
        {
            var added = await _cartService.AddAsync(GetUserId(), GetSessionId(), model.FoodItemId, model.Quantity);
            if (!added)
            {
                return NotFound(new { message = "Món ăn không tồn tại hoặc đang tạm hết." });
            }

            var cart = await _cartService.GetCartViewModelAsync(GetUserId(), GetSessionId());
            return Ok(ToResponse(cart));
        }

        [HttpPut("items/{cartItemId:int}")]
        public async Task<IActionResult> UpdateItem(int cartItemId, [FromBody] UpdateCartItemViewModel model)
        {
            await _cartService.UpdateAsync(GetUserId(), GetSessionId(), cartItemId, model.Quantity);
            var cart = await _cartService.GetCartViewModelAsync(GetUserId(), GetSessionId());
            return Ok(ToResponse(cart));
        }

        [HttpDelete("items/{cartItemId:int}")]
        public async Task<IActionResult> RemoveItem(int cartItemId)
        {
            await _cartService.RemoveAsync(GetUserId(), GetSessionId(), cartItemId);
            var cart = await _cartService.GetCartViewModelAsync(GetUserId(), GetSessionId());
            return Ok(ToResponse(cart));
        }

        [HttpDelete]
        public async Task<IActionResult> Clear()
        {
            var cart = await _cartService.GetOrCreateCartAsync(GetUserId(), GetSessionId());
            await _cartService.ClearAsync(cart);
            return Ok(new { items = Array.Empty<object>(), totalQuantity = 0, subtotal = 0 });
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

        private static object ToResponse(CartViewModel cart)
        {
            var items = cart.Items.Select(x => new
            {
                CartItemId = x.Id,
                FoodItemId = x.FoodItemId,
                Name = x.FoodItem?.Name ?? "Món ăn",
                Price = x.UnitPrice,
                Image = x.FoodItem?.ImageUrl ?? string.Empty,
                Quantity = x.Quantity,
                StockQuantity = cart.StockQuantities.TryGetValue(x.FoodItemId, out var stockQuantity) ? stockQuantity : null,
                LineTotal = x.UnitPrice * x.Quantity
            }).ToList();

            return new
            {
                Items = items,
                TotalQuantity = items.Sum(x => x.Quantity),
                Subtotal = cart.Subtotal
            };
        }
    }
}

