namespace DACS_Food.Models
{
    // Model cong thuc nau an: luu nguyen lieu, cac buoc lam, thoi gian va ghi chu an toan.
    public class Recipe
    {
        public int Id { get; set; }
        public int? FoodItemId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string CategoryLabel { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Ingredients { get; set; } = string.Empty;
        public string Steps { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public string Difficulty { get; set; } = "Cơ bản";
        public string PrepTime { get; set; } = string.Empty;
        public string CookTime { get; set; } = string.Empty;
        public string Servings { get; set; } = string.Empty;
        public string Tips { get; set; } = string.Empty;
        // Hai ghi chu nay giup trang cong thuc nhac nguoi dung ve an toan thuc pham va di ung.
        public string SafetyNote { get; set; } = string.Empty;
        public string AllergyNote { get; set; } = string.Empty;
        public int CookingTimeMinutes { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public FoodItem? FoodItem { get; set; }
    }
}
