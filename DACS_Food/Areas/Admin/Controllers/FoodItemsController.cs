using DACS_Food.Data;
using DACS_Food.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

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
            ModelState.Remove(nameof(model.Category));
            ModelState.Remove(nameof(model.MainCategory));
            ModelState.Remove(nameof(model.Subcategory));
            ModelState.Remove(nameof(model.Tag));
            ModelState.Remove(nameof(model.DetailDescription));
            ModelState.Remove(nameof(model.Ingredients));
            ModelState.Remove(nameof(model.ServingSize));
            ModelState.Remove(nameof(model.Story));
            ModelState.Remove(nameof(model.AllergyNote));
            ModelState.Remove(nameof(model.Allergens));
            ModelState.Remove(nameof(model.SpiceLevel));

            if (ModelState.IsValid)
            {
                // Ưu tiên file upload, nếu không có thì dùng URL đã nhập
                var category = await _db.FoodCategories.FindAsync(model.FoodCategoryId);
                if (category == null)
                {
                    ModelState.AddModelError(nameof(model.FoodCategoryId), "Danh mục không hợp lệ.");
                    ViewBag.Categories = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(await _db.FoodCategories.ToListAsync(), "Id", "Name", model.FoodCategoryId);
                    return View(model);
                }

                var uploadedUrl = await SaveImageAsync(imageFile);
                if (uploadedUrl != null)
                    model.ImageUrl = uploadedUrl;

                if (string.IsNullOrWhiteSpace(model.Slug))
                    model.Slug = GenerateSlug(model.Name);

                ApplyMenuDefaults(model, category);

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
            ModelState.Remove(nameof(model.Category));
            ModelState.Remove(nameof(model.MainCategory));
            ModelState.Remove(nameof(model.Subcategory));
            ModelState.Remove(nameof(model.Tag));
            ModelState.Remove(nameof(model.DetailDescription));
            ModelState.Remove(nameof(model.Ingredients));
            ModelState.Remove(nameof(model.ServingSize));
            ModelState.Remove(nameof(model.Story));
            ModelState.Remove(nameof(model.AllergyNote));
            ModelState.Remove(nameof(model.Allergens));
            ModelState.Remove(nameof(model.SpiceLevel));

            if (ModelState.IsValid)
            {
                var category = await _db.FoodCategories.FindAsync(model.FoodCategoryId);
                if (category == null)
                {
                    ModelState.AddModelError(nameof(model.FoodCategoryId), "Danh mục không hợp lệ.");
                    ViewBag.Categories = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(await _db.FoodCategories.ToListAsync(), "Id", "Name", model.FoodCategoryId);
                    return View(model);
                }

                var uploadedUrl = await SaveImageAsync(imageFile);
                if (uploadedUrl != null)
                    model.ImageUrl = uploadedUrl;

                if (string.IsNullOrWhiteSpace(model.Slug))
                    model.Slug = GenerateSlug(model.Name);

                ApplyMenuDefaults(model, category);

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

        private static void ApplyMenuDefaults(FoodItem item, FoodCategory category)
        {
            item.Category = string.IsNullOrWhiteSpace(item.Category) ? category.Slug : item.Category;
            item.MainCategory = InferMainCategory(category);
            item.Subcategory = string.IsNullOrWhiteSpace(item.Subcategory) ? category.Name : item.Subcategory;
            item.Tag = string.IsNullOrWhiteSpace(item.Tag) ? item.Subcategory : item.Tag;
            item.DetailDescription = string.IsNullOrWhiteSpace(item.DetailDescription) ? item.Description : item.DetailDescription;
            item.Story = string.IsNullOrWhiteSpace(item.Story) ? item.Description : item.Story;
            item.Ingredients = string.IsNullOrWhiteSpace(item.Ingredients) ? "Đang cập nhật" : item.Ingredients;
            item.AllergyNote = string.IsNullOrWhiteSpace(item.AllergyNote) ? "Vui lòng liên hệ nhân viên nếu quý khách có dị ứng thực phẩm." : item.AllergyNote;
            item.Allergens = string.IsNullOrWhiteSpace(item.Allergens) ? item.AllergyNote : item.Allergens;
            item.ServingSize = string.IsNullOrWhiteSpace(item.ServingSize) ? "1 người" : item.ServingSize;
            item.SpiceLevel = string.IsNullOrWhiteSpace(item.SpiceLevel) ? "Không cay" : item.SpiceLevel;
            item.IsVegetarian = item.IsVegetarian || item.MainCategory == "Món chay";
        }

        private static string InferMainCategory(FoodCategory category)
        {
            var key = NormalizeKey($"{category.Slug} {category.Name}");
            if (key.Contains("mon chay")) return "Món chay";
            if (key.Contains("healthy")) return "Healthy";
            if (key.Contains("do uong")) return "Đồ uống";
            return "Món mặn";
        }

        private static string GenerateSlug(string value)
        {
            var normalized = NormalizeKey(value);
            normalized = Regex.Replace(normalized, @"[^a-z0-9\s-]", "");
            normalized = Regex.Replace(normalized, @"\s+", "-").Trim('-');
            return string.IsNullOrWhiteSpace(normalized) ? Guid.NewGuid().ToString("N") : normalized;
        }

        private static string NormalizeKey(string value)
        {
            var normalized = value.Trim().ToLowerInvariant().Normalize(NormalizationForm.FormD);
            var builder = new StringBuilder();
            foreach (var character in normalized)
            {
                if (CharUnicodeInfo.GetUnicodeCategory(character) != UnicodeCategory.NonSpacingMark)
                {
                    builder.Append(character == 'đ' ? 'd' : character);
                }
            }

            return builder.ToString().Normalize(NormalizationForm.FormC);
        }
    }
}
