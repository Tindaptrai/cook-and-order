using DACS_Food.Data;
using DACS_Food.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DACS_Food.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class DiscountCodesController : Controller
    {
        private readonly ApplicationDbContext _db;

        public DiscountCodesController(ApplicationDbContext db)
        {
            _db = db;
        }

        [HttpGet("/admin/ma-giam-gia")]
        public async Task<IActionResult> Index()
        {
            return View(await _db.DiscountCodes.OrderBy(x => x.Code).ToListAsync());
        }

        [HttpPost("/admin/ma-giam-gia")]
        public async Task<IActionResult> Create(
            string code,
            string name,
            DiscountType discountType,
            decimal discountValue,
            decimal minOrderAmount,
            decimal? maxDiscountAmount,
            int? usageLimit,
            DiscountScope discountScope,
            string? requiredLoyaltyLevel,
            DateTime? startAt,
            DateTime? endAt,
            bool isActive = true)
        {
            code = (code ?? string.Empty).Trim().ToUpperInvariant();
            name = (name ?? string.Empty).Trim();

            if (string.IsNullOrWhiteSpace(code) || string.IsNullOrWhiteSpace(name))
            {
                TempData["DiscountError"] = "Vui lòng nhập mã và tên chương trình giảm giá.";
                return RedirectToAction(nameof(Index));
            }

            if (await _db.DiscountCodes.AnyAsync(x => x.Code == code))
            {
                TempData["DiscountError"] = "Mã giảm giá đã tồn tại.";
                return RedirectToAction(nameof(Index));
            }

            if (discountType == DiscountType.Percent && (discountValue <= 0 || discountValue > 100))
            {
                TempData["DiscountError"] = "Giảm theo phần trăm phải nằm trong khoảng 1% đến 100%.";
                return RedirectToAction(nameof(Index));
            }

            if (endAt.HasValue && startAt.HasValue && endAt.Value <= startAt.Value)
            {
                TempData["DiscountError"] = "Thời gian kết thúc phải sau thời gian bắt đầu.";
                return RedirectToAction(nameof(Index));
            }

            _db.DiscountCodes.Add(new DiscountCode
            {
                Code = code,
                Name = name,
                DiscountType = discountType,
                DiscountValue = discountValue,
                MinOrderAmount = minOrderAmount,
                MaxDiscountAmount = maxDiscountAmount,
                UsageLimit = usageLimit,
                DiscountScope = discountScope,
                RequiredLoyaltyLevel = NormalizeLoyaltyLevel(requiredLoyaltyLevel),
                StartAt = startAt,
                EndAt = endAt,
                IsActive = isActive
            });

            await _db.SaveChangesAsync();
            TempData["DiscountMessage"] = "Đã tạo mã giảm giá.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost("/admin/ma-giam-gia/cap-nhat")]
        public async Task<IActionResult> Update(int id, bool isActive, int? usageLimit, string? requiredLoyaltyLevel, DateTime? startAt, DateTime? endAt)
        {
            var discount = await _db.DiscountCodes.FindAsync(id);
            if (discount == null)
            {
                TempData["DiscountError"] = "Không tìm thấy mã giảm giá.";
                return RedirectToAction(nameof(Index));
            }

            if (endAt.HasValue && startAt.HasValue && endAt.Value <= startAt.Value)
            {
                TempData["DiscountError"] = "Thời gian kết thúc phải sau thời gian bắt đầu.";
                return RedirectToAction(nameof(Index));
            }

            discount.IsActive = isActive;
            discount.UsageLimit = usageLimit;
            discount.RequiredLoyaltyLevel = NormalizeLoyaltyLevel(requiredLoyaltyLevel);
            discount.StartAt = startAt;
            discount.EndAt = endAt;
            await _db.SaveChangesAsync();

            TempData["DiscountMessage"] = "Đã cập nhật mã giảm giá.";
            return RedirectToAction(nameof(Index));
        }

        private static string? NormalizeLoyaltyLevel(string? level)
        {
            return string.IsNullOrWhiteSpace(level) ? null : level.Trim();
        }
    }
}
