using DACS_Food.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DACS_Food.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class FoodItemsController : Controller
    {
        private readonly ApplicationDbContext _db;

        public FoodItemsController(ApplicationDbContext db)
        {
            _db = db;
        }

        [HttpGet("/admin/mon-an")]
        public async Task<IActionResult> Index()
        {
            var items = await _db.FoodItems.Include(x => x.FoodCategory).OrderBy(x => x.Name).ToListAsync();
            return View(items);
        }
    }
}
