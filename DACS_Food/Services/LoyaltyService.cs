using DACS_Food.Data;
using DACS_Food.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DACS_Food.Services
{
    public class LoyaltyService : ILoyaltyService
    {
        public const string Member = "Thành viên";
        public const string Silver = "Bạc";
        public const string Gold = "Vàng";
        public const string Platinum = "Bạch kim";

        private const decimal SilverTarget = 1_000_000m;
        private const decimal GoldTarget = 4_999_000m;
        private const decimal PlatinumTarget = 9_999_000m;

        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;

        public LoyaltyService(ApplicationDbContext db, UserManager<ApplicationUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        public string GetLevel(decimal totalSpent)
        {
            if (totalSpent < SilverTarget) return Member;
            if (totalSpent < GoldTarget) return Silver;
            if (totalSpent < PlatinumTarget) return Gold;
            return Platinum;
        }

        public string GetNextLevel(string currentLevel)
        {
            return NormalizeLevel(currentLevel) switch
            {
                Member => Silver,
                Silver => Gold,
                Gold => Platinum,
                Platinum => "Hạng cao nhất",
                _ => Silver
            };
        }

        public decimal? GetNextLevelTarget(string currentLevel)
        {
            return NormalizeLevel(currentLevel) switch
            {
                Member => SilverTarget,
                Silver => GoldTarget,
                Gold => PlatinumTarget,
                Platinum => null,
                _ => SilverTarget
            };
        }

        public int GetLevelRank(string? level)
        {
            return NormalizeLevel(level) switch
            {
                Member => 1,
                Silver => 2,
                Gold => 3,
                Platinum => 4,
                _ => 1
            };
        }

        public async Task<decimal> GetEligibleTotalSpentAsync(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId)) return 0;

            return await _db.Orders
                .Where(x => x.UserId == userId
                    && x.OrderStatus != OrderStatus.Cancelled
                    && (x.PaymentStatus == PaymentStatus.Paid
                        || x.OrderStatus == OrderStatus.Completed
                        || x.OrderStatus == OrderStatus.Delivered))
                .SumAsync(x => (decimal?)x.TotalAmount) ?? 0;
        }

        public async Task<string> RefreshUserLevelAsync(string? userId)
        {
            if (string.IsNullOrWhiteSpace(userId)) return Member;

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return Member;

            var level = GetLevel(await GetEligibleTotalSpentAsync(userId));
            if (NormalizeLevel(user.LoyaltyLevel) != level)
            {
                user.LoyaltyLevel = level;
                await _userManager.UpdateAsync(user);
            }

            return level;
        }

        private static string NormalizeLevel(string? level)
        {
            return level switch
            {
                "Báº¡c" or "BÃ¡ÂºÂ¡c" => Silver,
                "VÃ ng" => Gold,
                "Báº¡ch kim" or "BÃ¡ÂºÂ¡ch kim" => Platinum,
                "Member" or "Customer" => Member,
                null or "" => Member,
                _ => level
            };
        }
    }
}
