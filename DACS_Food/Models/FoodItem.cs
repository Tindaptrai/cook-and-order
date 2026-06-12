namespace DACS_Food.Models
{
    // Model dai dien cho mot mon an trong menu, gom thong tin hien thi, gia, nguyen lieu va trang thai ban.
    public class FoodItem
    {
        public int Id { get; set; }
        public int FoodCategoryId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string MainCategory { get; set; } = string.Empty;
        public string Subcategory { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public decimal? DiscountPrice { get; set; }
        public string Tag { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string DetailDescription { get; set; } = string.Empty;
        public string Ingredients { get; set; } = string.Empty;
        public int Calories { get; set; }
        public string ServingSize { get; set; } = string.Empty;
        public string Story { get; set; } = string.Empty;
        // Thong tin an toan/di ung duoc hien thi o chi tiet mon an de nguoi dung can nhac truoc khi dat.
        public string AllergyNote { get; set; } = string.Empty;
        public string Allergens { get; set; } = string.Empty;
        public string SpiceLevel { get; set; } = string.Empty;
        public bool IsAvailable { get; set; } = true;
        public bool IsFeatured { get; set; }
        public bool IsVegetarian { get; set; }
        public bool IsBestSeller { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        public FoodCategory? FoodCategory { get; set; }
        public ICollection<Recipe> Recipes { get; set; } = new List<Recipe>();
    }
}
