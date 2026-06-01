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
            return View();
        }

        [HttpGet("/Recipe/Detail/{slug}")]
        public async Task<IActionResult> Detail(string slug)
        {
            var recipe = await _db.Recipes
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Slug == slug && x.IsActive);

            recipe ??= _chatbotService.GetRecipeBySlug(slug);
            if (recipe == null) return NotFound();

            return View(recipe);
        }
    }
}
