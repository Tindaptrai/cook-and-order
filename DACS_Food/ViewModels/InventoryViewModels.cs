namespace DACS_Food.ViewModels
{
    public class InventoryPageViewModel
    {
        public IReadOnlyList<InventoryItemViewModel> Items { get; set; } = new List<InventoryItemViewModel>();
        public int CurrentMonth { get; set; }
    }

    public class InventoryItemViewModel
    {
        public int FoodItemId { get; set; }
        public string FoodName { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public string Ingredients { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public bool IsAvailable { get; set; }
        public int? StockQuantity { get; set; }
        public string Unit { get; set; } = "phần";
        public int[] AvailableMonths { get; set; } = Array.Empty<int>();
        public string InventoryNote { get; set; } = string.Empty;

        public bool HasSeason => AvailableMonths.Length > 0;
        public bool IsInCurrentSeason { get; set; }
    }

    public class UpdateInventoryViewModel
    {
        public int FoodItemId { get; set; }
        public string Ingredients { get; set; } = string.Empty;
        public bool IsAvailable { get; set; }
        public int? StockQuantity { get; set; }
        public string Unit { get; set; } = "phần";
        public int[] AvailableMonths { get; set; } = Array.Empty<int>();
        public string InventoryNote { get; set; } = string.Empty;
    }
}
