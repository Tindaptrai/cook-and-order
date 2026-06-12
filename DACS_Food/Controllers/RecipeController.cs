using DACS_Food.Data;
using DACS_Food.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DACS_Food.Controllers
{
    public class RecipeController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IChatbotService _chatbotService;

        public RecipeController(ApplicationDbContext db, IChatbotService chatbotService)
        {
            _db = db;
            _chatbotService = chatbotService;
        }

        [HttpGet("/recipes")]
        public IActionResult Index()
        {
            // Trang danh sach cong thuc: view giu layout, recipes.js xu ly loc danh muc va phan trang.
            return View();
        }

        [HttpGet("/Recipe/Detail/{slug}")]
        public async Task<IActionResult> Detail(string slug)
        {
            // Uu tien lay cong thuc tu database de hien thi noi dung do nhom phu trach quan ly.
            var recipe = await _db.Recipes
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Slug == slug && x.IsActive);

            // Neu chua co trong database thi dung du lieu mau tu chatbot service de trang chi tiet khong bi rong.
            recipe ??= _chatbotService.GetRecipeBySlug(slug);
            if (recipe == null) return NotFound();

            return View(recipe);
        }
    }
}
