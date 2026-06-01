using DACS_Food.Data;
using DACS_Food.Models;
using Microsoft.EntityFrameworkCore;

namespace DACS_Food.Services
{
    public class DiscountService : IDiscountService
    {
        private readonly ApplicationDbContext _db;
        private readonly ILoyaltyService _loyaltyService;

        public DiscountService(ApplicationDbContext db, ILoyaltyService loyaltyService)
        {
            _db = db;
            _loyaltyService = loyaltyService;
        }

        public bool IsGoldenHour(DateTime localTime)
        {
            var time = TimeOnly.FromDateTime(localTime);
            return IsInRange(time, new TimeOnly(10, 30), new TimeOnly(12, 30))
                || IsInRange(time, new TimeOnly(16, 0), new TimeOnly(18, 0));
        }

        public async Task<(DiscountCode? Code, decimal Amount, string? Message)> CalculateDiscountAsync(string? code, decimal subtotal, string? userId)
        {
            if (string.IsNullOrWhiteSpace(code))
            {
                return (null, 0, null);
            }

            var normalized = code.Trim().ToUpperInvariant();
            var discount = await _db.DiscountCodes.FirstOrDefaultAsync(x => x.Code == normalized && x.IsActive);
            if (discount == null)
            {
                return (null, 0, "Mã giảm giá không tồn tại.");
            }

            var now = DateTime.UtcNow;
            if (discount.StartAt.HasValue && discount.StartAt.Value > now)
            {
                return (null, 0, "Mã giảm giá chưa có hiệu lực.");
            }

            if (discount.EndAt.HasValue && discount.EndAt.Value < now)
            {
                return (null, 0, "Mã giảm giá đã hết hạn.");
            }

            if (discount.UsageLimit.HasValue && discount.UsedCount >= discount.UsageLimit.Value)
            {
                return (null, 0, "Mã giảm giá đã hết lượt sử dụng.");
            }

            if (subtotal < discount.MinOrderAmount)
            {
                return (null, 0, $"Đơn hàng cần tối thiểu {discount.MinOrderAmount:N0}đ để dùng mã này.");
            }

            if (discount.DiscountScope == DiscountScope.GoldenHour && !IsGoldenHour(DateTime.Now))
            {
                return (null, 0, "Mã giờ vàng chỉ áp dụng lúc 10:30-12:30 và 16:00-18:00.");
            }

            if (discount.DiscountScope == DiscountScope.Loyalty && string.IsNullOrWhiteSpace(userId))
            {
                return (null, 0, "Vui lòng đăng nhập để dùng mã giảm giá thành viên.");
            }

            if (discount.DiscountScope == DiscountScope.Loyalty && !string.IsNullOrWhiteSpace(userId))
            {
                var lastUsage = await _db.DiscountUsages
                    .Where(x => x.UserId == userId && x.DiscountCodeId == discount.Id)
                    .OrderByDescending(x => x.UsedAt)
                    .FirstOrDefaultAsync();

                if (lastUsage != null && lastUsage.UsedAt > DateTime.UtcNow.AddDays(-7))
                {
                    return (null, 0, "Mã thành viên thân thiết chỉ dùng 1 tuần/lần.");
                }
            }

            if (!string.IsNullOrWhiteSpace(discount.RequiredLoyaltyLevel))
            {
                if (string.IsNullOrWhiteSpace(userId))
                {
                    return (null, 0, "Vui lòng đăng nhập để dùng mã giảm giá theo hạng thành viên.");
                }

                var currentLevel = await _loyaltyService.RefreshUserLevelAsync(userId);
                if (_loyaltyService.GetLevelRank(currentLevel) < _loyaltyService.GetLevelRank(discount.RequiredLoyaltyLevel))
                {
                    return (null, 0, $"Mã này chỉ áp dụng từ hạng {discount.RequiredLoyaltyLevel} trở lên. Hạng hiện tại của bạn là {currentLevel}.");
                }
            }

            var amount = discount.DiscountType == DiscountType.Percent
                ? subtotal * discount.DiscountValue / 100
                : discount.DiscountValue;

            if (discount.MaxDiscountAmount.HasValue)
            {
                amount = Math.Min(amount, discount.MaxDiscountAmount.Value);
            }

            amount = Math.Min(amount, subtotal);
            return (discount, amount, null);
        }

        public async Task MarkUsedAsync(DiscountCode? code, string? userId, int orderId)
        {
            if (code == null) return;

            code.UsedCount += 1;
            _db.DiscountUsages.Add(new DiscountUsage
            {
                DiscountCodeId = code.Id,
                UserId = userId,
                OrderId = orderId
            });
            await _db.SaveChangesAsync();
        }

        private static bool IsInRange(TimeOnly value, TimeOnly start, TimeOnly end)
        {
            return value >= start && value <= end;
        }
    }
}
