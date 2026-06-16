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
        private readonly IWebHostEnvironment _env;

        public FoodItemsController(ApplicationDbContext db, IWebHostEnvironment env)
        {
            _db = db;
            _env = env;
        }

        // ── Lưu file ảnh lên wwwroot/images/foods/ ──────────────────────────
        private async Task<string?> SaveImageAsync(IFormFile? file)
        {
            if (file == null || file.Length == 0) return null;

            var allowed = new[] { ".jpg", ".jpeg", ".png", ".webp", ".gif" };
            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!allowed.Contains(ext)) return null;

            var folder = Path.Combine(_env.WebRootPath, "images", "foods");
            Directory.CreateDirectory(folder);

            var fileName = $"{Guid.NewGuid()}{ext}";
            var filePath = Path.Combine(folder, fileName);

            using var stream = new FileStream(filePath, FileMode.Create);
            await file.CopyToAsync(stream);

            return $"/images/foods/{fileName}";
        }

        // ── INDEX ─────────────────────────────────────────────────────────────
        [HttpGet("/admin/mon-an")]
        public async Task<IActionResult> Index()
        {
            var items = await _db.FoodItems.Include(x => x.FoodCategory).OrderBy(x => x.Name).ToListAsync();
            return View(items);
        }

        // ── CREATE GET ────────────────────────────────────────────────────────
        [HttpGet("/admin/mon-an/tao")]
        public async Task<IActionResult> Create()
        {
            ViewBag.Categories = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(await _db.FoodCategories.ToListAsync(), "Id", "Name");
            return View();
        }

        // ── CREATE POST ───────────────────────────────────────────────────────
        [HttpPost("/admin/mon-an/tao")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DACS_Food.Models.FoodItem model, IFormFile? imageFile)
        {
            // Bỏ qua validation cho ImageUrl và Slug vì hệ thống tự xử lý
            ModelState.Remove(nameof(model.ImageUrl));
            ModelState.Remove(nameof(model.Slug));

            if (ModelState.IsValid)
            {
                // Ưu tiên file upload, nếu không có thì dùng URL đã nhập
                var uploadedUrl = await SaveImageAsync(imageFile);
                if (uploadedUrl != null)
                    model.ImageUrl = uploadedUrl;

                if (string.IsNullOrWhiteSpace(model.Slug))
                    model.Slug = model.Name.ToLower().Replace(" ", "-");

                _db.FoodItems.Add(model);
                await _db.SaveChangesAsync();
                TempData["FoodItemMessage"] = $"Đã thêm món «{model.Name}» thành công.";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Categories = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(await _db.FoodCategories.ToListAsync(), "Id", "Name", model.FoodCategoryId);
            return View(model);
        }

        // ── EDIT GET ──────────────────────────────────────────────────────────
        [HttpGet("/admin/mon-an/sua/{id:int}")]
        public async Task<IActionResult> Edit(int id)
        {
            var item = await _db.FoodItems.FindAsync(id);
            if (item == null) return NotFound();
            ViewBag.Categories = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(await _db.FoodCategories.ToListAsync(), "Id", "Name", item.FoodCategoryId);
            return View(item);
        }

        // ── EDIT POST ─────────────────────────────────────────────────────────
        [HttpPost("/admin/mon-an/sua/{id:int}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, DACS_Food.Models.FoodItem model, IFormFile? imageFile)
        {
            if (id != model.Id) return BadRequest();

            // Bỏ qua validation cho ImageUrl và Slug vì hệ thống tự xử lý
            ModelState.Remove(nameof(model.ImageUrl));
            ModelState.Remove(nameof(model.Slug));

            if (ModelState.IsValid)
            {
                var uploadedUrl = await SaveImageAsync(imageFile);
                if (uploadedUrl != null)
                    model.ImageUrl = uploadedUrl;

                if (string.IsNullOrWhiteSpace(model.Slug))
                    model.Slug = model.Name.ToLower().Replace(" ", "-");

                model.UpdatedAt = DateTime.UtcNow;
                _db.FoodItems.Update(model);
                await _db.SaveChangesAsync();
                TempData["FoodItemMessage"] = $"Đã cập nhật món «{model.Name}» thành công.";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Categories = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(await _db.FoodCategories.ToListAsync(), "Id", "Name", model.FoodCategoryId);
            return View(model);
        }

        // ── DELETE ────────────────────────────────────────────────────────────
        [HttpPost("/admin/mon-an/xoa/{id:int}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _db.FoodItems.FindAsync(id);
            if (item == null) return NotFound();
            _db.FoodItems.Remove(item);
            await _db.SaveChangesAsync();
            TempData["FoodItemMessage"] = $"Đã xóa món «{item.Name}» thành công.";
            return RedirectToAction(nameof(Index));
        }
    }
}
