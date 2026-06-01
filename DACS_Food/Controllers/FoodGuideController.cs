using Microsoft.AspNetCore.Mvc;

namespace DACS_Food.Controllers
{
    public class FoodGuideController : Controller
    {
        [HttpGet("/food-guide")]
        public IActionResult Index()
        {
            return View();
        }
    }
}
