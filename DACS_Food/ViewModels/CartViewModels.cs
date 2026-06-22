using DACS_Food.Models;

namespace DACS_Food.ViewModels
{
    public class CartViewModel
    {
        public IReadOnlyList<CartItem> Items { get; set; } = Array.Empty<CartItem>();
        public IReadOnlyDictionary<int, int?> StockQuantities { get; set; } = new Dictionary<int, int?>();
        public decimal Subtotal => Items.Sum(x => x.UnitPrice * x.Quantity);
    }

    public class AddCartItemViewModel
    {
        public int FoodItemId { get; set; }
        public int Quantity { get; set; } = 1;
    }

    public class UpdateCartItemViewModel
    {
        public int Quantity { get; set; }
    }
}
