namespace DACS_Food.Models
{
    public class FoodCategory
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        public int SortOrder { get; set; }

        public ICollection<FoodItem> FoodItems { get; set; } = new List<FoodItem>();
    }
}
