using DACS_Food.Models;

namespace DACS_Food.ViewModels
{
    public class MenuViewModel
    {
        public IReadOnlyList<FoodCategory> Categories { get; set; } = Array.Empty<FoodCategory>();
        public IReadOnlyList<FoodItem> FoodItems { get; set; } = Array.Empty<FoodItem>();
        public string? Category { get; set; }
        public string? Keyword { get; set; }
        public int Page { get; set; } = 1;
        public int TotalPages { get; set; } = 1;
    }
}
