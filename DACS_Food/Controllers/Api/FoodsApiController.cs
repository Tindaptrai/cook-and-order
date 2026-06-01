using DACS_Food.Services;
using Microsoft.AspNetCore.Mvc;

namespace DACS_Food.Controllers.Api
{
    [ApiController]
    [Route("api/foods")]
    public class FoodsApiController : ControllerBase
    {
        private readonly IFoodService _foodService;

        public FoodsApiController(IFoodService foodService)
        {
            _foodService = foodService;
        }

        [HttpGet]
        public async Task<IActionResult> GetFoods(string? category, string? keyword, string? mainCategory, string? subcategory, int page = 1, int pageSize = 12)
        {
            var model = await _foodService.GetMenuAsync(category, keyword, page, pageSize, mainCategory, subcategory);
            return Ok(new
            {
                model.Page,
                model.TotalPages,
                model.Category,
                model.Keyword,
                Categories = model.Categories.Select(x => new { x.Id, x.Name, x.Slug }),
                Items = model.FoodItems.Select(ToFoodDto)
            });
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetFoodById(int id)
        {
            var food = await _foodService.GetByIdAsync(id);
            if (food == null) return NotFound(new { message = "Không tìm thấy món ăn." });
            return Ok(ToFoodDto(food));
        }

        [HttpGet("slug/{slug}")]
        public async Task<IActionResult> GetFoodBySlug(string slug)
        {
            var food = await _foodService.GetBySlugAsync(slug);
            if (food == null) return NotFound(new { message = "Không tìm thấy món ăn." });
            return Ok(ToFoodDto(food));
        }

        [HttpGet("best-sellers")]
        public async Task<IActionResult> GetBestSellers(int count = 6)
        {
            var items = await _foodService.GetBestSellersAsync(count);
            return Ok(items.Select(ToFoodDto));
        }

        private static object ToFoodDto(DACS_Food.Models.FoodItem x)
        {
            return new
            {
                x.Id,
                x.Name,
                x.Slug,
                Category = x.FoodCategory?.Name ?? x.Category,
                x.MainCategory,
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
                x.IsFeatured,
                x.IsVegetarian,
                x.IsBestSeller
            };
        }
    }
}
