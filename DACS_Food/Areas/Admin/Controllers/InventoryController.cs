using DACS_Food.Data;
using DACS_Food.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace DACS_Food.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class InventoryController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly string _metadataPath;
        private static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = true };

        public InventoryController(ApplicationDbContext db, IWebHostEnvironment env)
        {
            _db = db;
            _metadataPath = Path.Combine(env.ContentRootPath, "Data", "inventory-metadata.json");
        }

        [HttpGet("/admin/kho-nguyen-lieu")]
        public async Task<IActionResult> Index()
        {
            var metadata = await LoadMetadataAsync();
            var currentMonth = DateTime.Now.Month;
            var foods = await _db.FoodItems
                .Include(x => x.FoodCategory)
                .OrderBy(x => x.Name)
                .ToListAsync();

            var model = new InventoryPageViewModel
            {
                CurrentMonth = currentMonth,
                Items = foods.Select(food =>
                {
                    metadata.TryGetValue(food.Id.ToString(), out var itemMeta);
                    var months = NormalizeMonths(itemMeta?.AvailableMonths);

                    return new InventoryItemViewModel
                    {
                        FoodItemId = food.Id,
                        FoodName = food.Name,
                        CategoryName = food.FoodCategory?.Name ?? food.Subcategory,
                        Ingredients = food.Ingredients,
                        IsActive = food.IsActive,
                        IsAvailable = food.IsAvailable,
                        StockQuantity = itemMeta?.StockQuantity,
                        Unit = string.IsNullOrWhiteSpace(itemMeta?.Unit) ? "phần" : itemMeta.Unit,
                        AvailableMonths = months,
                        InventoryNote = itemMeta?.InventoryNote ?? string.Empty,
                        IsInCurrentSeason = months.Length == 0 || months.Contains(currentMonth)
                    };
                }).ToList()
            };

            return View(model);
        }

        [HttpPost("/admin/kho-nguyen-lieu/cap-nhat/{id:int}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int id, UpdateInventoryViewModel model)
        {
            if (id != model.FoodItemId) return BadRequest();

            var food = await _db.FoodItems.FindAsync(id);
            if (food == null) return NotFound();

            var metadata = await LoadMetadataAsync();
            var months = NormalizeMonths(model.AvailableMonths);
            var hasStock = !model.StockQuantity.HasValue || model.StockQuantity.Value > 0;
            var isInCurrentSeason = months.Length == 0 || months.Contains(DateTime.Now.Month);
            var canSell = hasStock && isInCurrentSeason;

            food.Ingredients = (model.Ingredients ?? string.Empty).Trim();
            food.IsAvailable = canSell;
            food.UpdatedAt = DateTime.UtcNow;

            metadata[food.Id.ToString()] = new InventoryMetadata
            {
                StockQuantity = model.StockQuantity,
                Unit = string.IsNullOrWhiteSpace(model.Unit) ? "phần" : model.Unit.Trim(),
                AvailableMonths = months,
                InventoryNote = (model.InventoryNote ?? string.Empty).Trim()
            };

            await _db.SaveChangesAsync();
            await SaveMetadataAsync(metadata);

            TempData["InventoryMessage"] = $"Đã cập nhật kho cho món \"{food.Name}\".";
            return RedirectToAction(nameof(Index));
        }

        private async Task<Dictionary<string, InventoryMetadata>> LoadMetadataAsync()
        {
            if (!System.IO.File.Exists(_metadataPath))
            {
                return new Dictionary<string, InventoryMetadata>();
            }

            await using var stream = System.IO.File.OpenRead(_metadataPath);
            var data = await JsonSerializer.DeserializeAsync<Dictionary<string, InventoryMetadata>>(stream);
            return data ?? new Dictionary<string, InventoryMetadata>();
        }

        private async Task SaveMetadataAsync(Dictionary<string, InventoryMetadata> metadata)
        {
            var folder = Path.GetDirectoryName(_metadataPath);
            if (!string.IsNullOrWhiteSpace(folder))
            {
                Directory.CreateDirectory(folder);
            }

            await using var stream = System.IO.File.Create(_metadataPath);
            await JsonSerializer.SerializeAsync(stream, metadata, JsonOptions);
        }

        private static int[] NormalizeMonths(IEnumerable<int>? months)
        {
            return months?
                .Where(month => month >= 1 && month <= 12)
                .Distinct()
                .OrderBy(month => month)
                .ToArray() ?? Array.Empty<int>();
        }

        private sealed class InventoryMetadata
        {
            public int? StockQuantity { get; set; }
            public string Unit { get; set; } = "phần";
            public int[] AvailableMonths { get; set; } = Array.Empty<int>();
            public string InventoryNote { get; set; } = string.Empty;
        }
    }
}
