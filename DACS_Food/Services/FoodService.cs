using DACS_Food.Data;
using DACS_Food.Models;
using DACS_Food.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace DACS_Food.Services
{
    public class FoodService : IFoodService
    {
        private readonly ApplicationDbContext _db;

        public FoodService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<MenuViewModel> GetMenuAsync(string? category, string? keyword, int page, int pageSize, string? mainCategory = null, string? subcategory = null)
        {
            page = Math.Max(1, page);
            pageSize = Math.Max(1, pageSize);

            var query = _db.FoodItems
                .Include(x => x.FoodCategory)
                .Where(x => x.IsActive && x.IsAvailable);

            if (!string.IsNullOrWhiteSpace(category) && category != "all")
            {
                query = query.Where(x => x.FoodCategory != null && x.FoodCategory.Slug == category);
            }

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                query = query.Where(x => x.Name.Contains(keyword) || x.Description.Contains(keyword));
            }

            if (!string.IsNullOrWhiteSpace(mainCategory) && mainCategory != "all")
            {
                query = query.Where(x => x.MainCategory == mainCategory);
            }

            if (!string.IsNullOrWhiteSpace(subcategory) && subcategory != "all")
            {
                query = query.Where(x => x.Subcategory == subcategory);
            }

            var total = await query.CountAsync();
            var totalPages = Math.Max(1, (int)Math.Ceiling(total / (double)pageSize));
            page = Math.Min(page, totalPages);

            var items = await query
                .OrderByDescending(x => x.IsBestSeller)
                .ThenBy(x => x.Name)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var categories = await _db.FoodCategories
                .Where(x => x.IsActive)
                .OrderBy(x => x.SortOrder)
                .ToListAsync();

            return new MenuViewModel
            {
                Categories = categories,
                FoodItems = items,
                Category = category,
                Keyword = keyword,
                Page = page,
                TotalPages = totalPages
            };
        }

        public async Task<IReadOnlyList<FoodItem>> GetBestSellersAsync(int count)
        {
            return await _db.FoodItems
                .Include(x => x.FoodCategory)
                .Where(x => x.IsActive && x.IsAvailable && (x.IsBestSeller || x.IsFeatured))
                .OrderBy(x => x.Name)
                .Take(count)
                .ToListAsync();
        }

        public async Task<IReadOnlyList<FoodItem>> GetRelatedAsync(FoodItem food, int count)
        {
            return await _db.FoodItems
                .Include(x => x.FoodCategory)
                .Where(x =>
                    x.IsActive &&
                    x.IsAvailable &&
                    x.Id != food.Id &&
                    (x.FoodCategoryId == food.FoodCategoryId || x.MainCategory == food.MainCategory))
                .OrderByDescending(x => x.IsFeatured || x.IsBestSeller)
                .ThenBy(x => x.Name)
                .Take(count)
                .ToListAsync();
        }

        public Task<FoodItem?> GetBySlugAsync(string slug)
        {
            return _db.FoodItems
                .Include(x => x.FoodCategory)
                .FirstOrDefaultAsync(x => x.Slug == slug && x.IsActive);
        }

        public Task<FoodItem?> GetByIdAsync(int id)
        {
            return _db.FoodItems
                .Include(x => x.FoodCategory)
                .FirstOrDefaultAsync(x => x.Id == id && x.IsActive);
        }
    }
}
