using DACS_Food.Services;
using Microsoft.AspNetCore.Mvc;

namespace DACS_Food.Controllers
{
    public class MenuController : Controller
    {
        private readonly IFoodService _foodService;

        public MenuController(IFoodService foodService)
        {
            _foodService = foodService;
        }

        [HttpGet("/menu")]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet("/menu/{slug}")]
        public async Task<IActionResult> Detail(string slug)
        {
            var food = await _foodService.GetBySlugAsync(slug);
            if (food == null) return NotFound();
            ViewBag.RelatedFoods = await _foodService.GetRelatedAsync(food, 4);
            return View(food);
        }
    }
}
