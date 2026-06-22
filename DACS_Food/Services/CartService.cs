using DACS_Food.Data;
using DACS_Food.Models;
using DACS_Food.ViewModels;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace DACS_Food.Services
{
    public class CartService : ICartService
    {
        private readonly ApplicationDbContext _db;
        private readonly string _inventoryMetadataPath;
        private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

        public CartService(ApplicationDbContext db, IWebHostEnvironment env)
        {
            _db = db;
            _inventoryMetadataPath = Path.Combine(env.ContentRootPath, "Data", "inventory-metadata.json");
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
            var stockQuantities = await LoadInventoryStockAsync();
            var changed = NormalizeCartQuantities(cart, stockQuantities);
            if (changed)
            {
                cart.UpdatedAt = DateTime.UtcNow;
                await _db.SaveChangesAsync();
            }

            return new CartViewModel
            {
                Items = cart.Items.ToList(),
                StockQuantities = stockQuantities
            };
        }

        public async Task<bool> AddAsync(string? userId, string sessionId, int foodItemId, int quantity)
        {
            var food = await _db.FoodItems.FirstOrDefaultAsync(x => x.Id == foodItemId && x.IsActive && x.IsAvailable);
            if (food == null) return false;

            var stockQuantities = await LoadInventoryStockAsync();
            var maxQuantity = GetMaxQuantity(foodItemId, stockQuantities);
            if (maxQuantity <= 0) return false;

            quantity = Math.Clamp(quantity, 1, maxQuantity);

            var cart = await GetOrCreateCartAsync(userId, sessionId);
            var item = cart.Items.FirstOrDefault(x => x.FoodItemId == foodItemId);
            if (item == null)
            {
                cart.Items.Add(new CartItem { FoodItemId = foodItemId, Quantity = quantity, UnitPrice = food.Price });
            }
            else
            {
                item.Quantity = Math.Min(maxQuantity, item.Quantity + quantity);
            }

            cart.UpdatedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task UpdateAsync(string? userId, string sessionId, int cartItemId, int quantity)
        {
            var cart = await GetOrCreateCartAsync(userId, sessionId);
            var item = cart.Items.FirstOrDefault(x => x.Id == cartItemId);
            if (item == null) return;

            var stockQuantities = await LoadInventoryStockAsync();
            var maxQuantity = GetMaxQuantity(item.FoodItemId, stockQuantities);
            if (maxQuantity <= 0 || item.FoodItem == null || !item.FoodItem.IsActive || !item.FoodItem.IsAvailable)
            {
                _db.CartItems.Remove(item);
                cart.Items.Remove(item);
                cart.UpdatedAt = DateTime.UtcNow;
                await _db.SaveChangesAsync();
                return;
            }

            quantity = Math.Clamp(quantity, 1, maxQuantity);
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

        private bool NormalizeCartQuantities(Cart cart, IReadOnlyDictionary<int, int?> stockQuantities)
        {
            var changed = false;

            foreach (var item in cart.Items.ToList())
            {
                var maxQuantity = GetMaxQuantity(item.FoodItemId, stockQuantities);
                if (maxQuantity <= 0 || item.FoodItem == null || !item.FoodItem.IsActive || !item.FoodItem.IsAvailable)
                {
                    _db.CartItems.Remove(item);
                    cart.Items.Remove(item);
                    changed = true;
                    continue;
                }

                if (item.Quantity > maxQuantity)
                {
                    item.Quantity = maxQuantity;
                    changed = true;
                }

                if (item.Quantity < 1)
                {
                    item.Quantity = 1;
                    changed = true;
                }
            }

            return changed;
        }

        private static int GetMaxQuantity(int foodItemId, IReadOnlyDictionary<int, int?> stockQuantities)
        {
            if (stockQuantities.TryGetValue(foodItemId, out var stockQuantity) && stockQuantity.HasValue)
            {
                return Math.Clamp(stockQuantity.Value, 0, 20);
            }

            return 20;
        }

        private async Task<Dictionary<int, int?>> LoadInventoryStockAsync()
        {
            if (!File.Exists(_inventoryMetadataPath))
            {
                return new Dictionary<int, int?>();
            }

            await using var stream = File.OpenRead(_inventoryMetadataPath);
            var data = await JsonSerializer.DeserializeAsync<Dictionary<string, InventoryMetadata>>(stream, JsonOptions);
            return data?
                .Where(x => int.TryParse(x.Key, out _))
                .ToDictionary(x => int.Parse(x.Key), x => x.Value.StockQuantity)
                ?? new Dictionary<int, int?>();
        }

        private sealed class InventoryMetadata
        {
            public int? StockQuantity { get; set; }
        }
    }
}
