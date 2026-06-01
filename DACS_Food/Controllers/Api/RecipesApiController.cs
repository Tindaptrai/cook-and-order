using DACS_Food.Data;
using DACS_Food.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DACS_Food.Controllers.Api
{
    [ApiController]
    [Route("api/recipes")]
    public class RecipesApiController : ControllerBase
    {
        private readonly ApplicationDbContext _db;

        public RecipesApiController(ApplicationDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public async Task<IActionResult> GetRecipes(
            [FromQuery] string? category,
            [FromQuery] string? keyword,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 4)
        {
            page = Math.Max(1, page);
            pageSize = Math.Clamp(pageSize, 1, 100);

            var query = _db.Recipes
                .AsNoTracking()
                .Where(x => x.IsActive);

            if (!string.IsNullOrWhiteSpace(category) && category != "all")
            {
                query = query.Where(x => x.Category == category);
            }

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                var term = keyword.Trim();
                query = query.Where(x =>
                    x.Title.Contains(term) ||
                    x.Description.Contains(term) ||
                    x.Ingredients.Contains(term));
            }

            var totalItems = await query.CountAsync();
            var totalPages = Math.Max(1, (int)Math.Ceiling(totalItems / (double)pageSize));
            var items = await query
                .OrderBy(x => x.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(x => MapRecipe(x))
                .ToListAsync();

            return Ok(new
            {
                page,
                pageSize,
                totalItems,
                totalPages,
                items
            });
        }

        [HttpGet("{slug}")]
        public async Task<IActionResult> GetRecipe(string slug)
        {
            var recipe = await _db.Recipes
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Slug == slug && x.IsActive);

            return recipe == null ? NotFound() : Ok(MapRecipe(recipe));
        }

        private static object MapRecipe(Recipe recipe)
        {
            return new
            {
                id = recipe.Id,
                name = recipe.Title,
                slug = recipe.Slug,
                category = recipe.Category,
                categoryLabel = recipe.CategoryLabel,
                image = recipe.ImageUrl,
                difficulty = recipe.Difficulty,
                prepTime = recipe.PrepTime,
                cookTime = recipe.CookTime,
                servings = recipe.Servings,
                shortDesc = recipe.Description,
                ingredients = SplitList(recipe.Ingredients),
                steps = SplitList(recipe.Steps),
                tips = recipe.Tips,
                safetyNote = recipe.SafetyNote,
                allergyNote = recipe.AllergyNote
            };
        }

        private static string[] SplitList(string value)
        {
            return value
                .Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        }
    }
}
