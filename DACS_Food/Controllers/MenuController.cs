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
            // Trang danh sach mon an: giao dien duoc render bang view va du lieu duoc nap qua API/menu.js.
            return View();
        }

        [HttpGet("/menu/{slug}")]
        public async Task<IActionResult> Detail(string slug)
        {
            // Trang chi tiet mon an dung slug de tim dung mon va goi y them cac mon lien quan.
            var food = await _foodService.GetBySlugAsync(slug);
            if (food == null) return NotFound();
            ViewBag.RelatedFoods = await _foodService.GetRelatedAsync(food, 4);
            return View(food);
        }
    }
}
