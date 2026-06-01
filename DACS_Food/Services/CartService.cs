using DACS_Food.Data;
using DACS_Food.Models;
using DACS_Food.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace DACS_Food.Services
{
    public class CartService : ICartService
    {
        private readonly ApplicationDbContext _db;

        public CartService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<Cart> GetOrCreateCartAsync(string? userId, string sessionId)
        {
            var query = _db.Carts.Include(x => x.Items).ThenInclude(x => x.FoodItem).AsQueryable();
            var cart = !string.IsNullOrWhiteSpace(userId)
                ? await query.FirstOrDefaultAsync(x => x.UserId == userId)
                : await query.FirstOrDefaultAsync(x => x.SessionId == sessionId);

            if (cart != null)
            {
                return cart;
            }

            cart = new Cart { UserId = userId, SessionId = sessionId };
            _db.Carts.Add(cart);
            await _db.SaveChangesAsync();
            return cart;
        }

        public async Task<CartViewModel> GetCartViewModelAsync(string? userId, string sessionId)
        {
            var cart = await GetOrCreateCartAsync(userId, sessionId);
            return new CartViewModel { Items = cart.Items.ToList() };
        }

        public async Task AddAsync(string? userId, string sessionId, int foodItemId, int quantity)
        {
            quantity = Math.Clamp(quantity, 1, 20);
            var food = await _db.FoodItems.FirstOrDefaultAsync(x => x.Id == foodItemId && x.IsActive && x.IsAvailable);
            if (food == null) return;

            var cart = await GetOrCreateCartAsync(userId, sessionId);
            var item = cart.Items.FirstOrDefault(x => x.FoodItemId == foodItemId);
            if (item == null)
            {
                cart.Items.Add(new CartItem { FoodItemId = foodItemId, Quantity = quantity, UnitPrice = food.Price });
            }
            else
            {
                item.Quantity = Math.Min(20, item.Quantity + quantity);
            }

            cart.UpdatedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();
        }

        public async Task UpdateAsync(string? userId, string sessionId, int cartItemId, int quantity)
        {
            quantity = Math.Clamp(quantity, 1, 20);
            var cart = await GetOrCreateCartAsync(userId, sessionId);
            var item = cart.Items.FirstOrDefault(x => x.Id == cartItemId);
            if (item == null) return;

            item.Quantity = quantity;

            cart.UpdatedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();
        }

        public async Task RemoveAsync(string? userId, string sessionId, int cartItemId)
        {
            var cart = await GetOrCreateCartAsync(userId, sessionId);
            var item = cart.Items.FirstOrDefault(x => x.Id == cartItemId);
            if (item == null) return;

            _db.CartItems.Remove(item);
            cart.UpdatedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();
        }

        public async Task ClearAsync(Cart cart)
        {
            _db.CartItems.RemoveRange(cart.Items);
            cart.UpdatedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();
        }
    }
}
