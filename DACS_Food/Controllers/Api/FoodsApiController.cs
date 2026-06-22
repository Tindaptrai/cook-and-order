using DACS_Food.Services;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace DACS_Food.Controllers.Api
{
    [ApiController]
    [Route("api/foods")]
    public class FoodsApiController : ControllerBase
    {
        private readonly IFoodService _foodService;
        private readonly string _inventoryMetadataPath;
        private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

        public FoodsApiController(IFoodService foodService, IWebHostEnvironment env)
        {
            _foodService = foodService;
            _inventoryMetadataPath = Path.Combine(env.ContentRootPath, "Data", "inventory-metadata.json");
        }

        [HttpGet]
        public async Task<IActionResult> GetFoods(string? category, string? keyword, string? mainCategory, string? subcategory, int page = 1, int pageSize = 12)
        {
            var model = await _foodService.GetMenuAsync(category, keyword, page, pageSize, mainCategory, subcategory);
            var inventoryMetadata = await LoadInventoryMetadataAsync();

            return Ok(new
            {
                model.Page,
                model.TotalPages,
                model.Category,
                model.Keyword,
                Categories = model.Categories.Select(x => new { x.Id, x.Name, x.Slug }),
                Items = model.FoodItems.Select(food => ToFoodDto(food, inventoryMetadata))
            });
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetFoodById(int id)
        {
            var food = await _foodService.GetByIdAsync(id);
            if (food == null) return NotFound(new { message = "Khong tim thay mon an." });

            var inventoryMetadata = await LoadInventoryMetadataAsync();
            return Ok(ToFoodDto(food, inventoryMetadata));
        }

        [HttpGet("slug/{slug}")]
        public async Task<IActionResult> GetFoodBySlug(string slug)
        {
            var food = await _foodService.GetBySlugAsync(slug);
            if (food == null) return NotFound(new { message = "Khong tim thay mon an." });

            var inventoryMetadata = await LoadInventoryMetadataAsync();
            return Ok(ToFoodDto(food, inventoryMetadata));
        }

        [HttpGet("best-sellers")]
        public async Task<IActionResult> GetBestSellers(int count = 6)
        {
            var items = await _foodService.GetBestSellersAsync(count);
            var inventoryMetadata = await LoadInventoryMetadataAsync();
            return Ok(items.Select(food => ToFoodDto(food, inventoryMetadata)));
        }

        private static object ToFoodDto(DACS_Food.Models.FoodItem x, IReadOnlyDictionary<string, InventoryFoodMetadata> inventoryMetadata)
        {
            inventoryMetadata.TryGetValue(x.Id.ToString(), out var inventory);

            return new
            {
                x.Id,
                x.Name,
                x.Slug,
                Category = x.FoodCategory?.Name ?? x.Category,
                MainCategory = string.IsNullOrWhiteSpace(x.MainCategory) ? InferMainCategory(x) : x.MainCategory,
                x.Subcategory,
                x.Price,
                x.DiscountPrice,
                x.Tag,
                x.ImageUrl,
                x.Description,
                x.DetailDescription,
                x.Ingredients,
                x.Calories,
                x.ServingSize,
                x.Story,
                x.AllergyNote,
                x.Allergens,
                x.SpiceLevel,
                x.IsAvailable,
                StockQuantity = inventory?.StockQuantity,
                x.IsFeatured,
                x.IsVegetarian,
                x.IsBestSeller
            };
        }

        private async Task<Dictionary<string, InventoryFoodMetadata>> LoadInventoryMetadataAsync()
        {
            if (!System.IO.File.Exists(_inventoryMetadataPath))
            {
                return new Dictionary<string, InventoryFoodMetadata>();
            }

            await using var stream = System.IO.File.OpenRead(_inventoryMetadataPath);
            var data = await JsonSerializer.DeserializeAsync<Dictionary<string, InventoryFoodMetadata>>(stream, JsonOptions);
            return data ?? new Dictionary<string, InventoryFoodMetadata>();
        }

        private static string InferMainCategory(DACS_Food.Models.FoodItem food)
        {
            var source = $"{food.MainCategory} {food.Category} {food.FoodCategory?.Name} {food.FoodCategory?.Slug}".ToLowerInvariant();
            if (source.Contains("chay")) return "MÃ³n chay";
            if (source.Contains("healthy")) return "Healthy";
            if (source.Contains("Ä‘á»“ uá»‘ng") || source.Contains("do-uong") || source.Contains("do uong")) return "Äá»“ uá»‘ng";
            return "MÃ³n máº·n";
        }

        private sealed class InventoryFoodMetadata
        {
            public int? StockQuantity { get; set; }
        }
    }
}
