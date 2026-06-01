using DACS_Food.Data;
using DACS_Food.Models;
using Microsoft.EntityFrameworkCore;

namespace DACS_Food.Services
{
    public class OtpRateLimitService : IOtpRateLimitService
    {
        private readonly ApplicationDbContext _db;

        public OtpRateLimitService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<(bool Allowed, string? Message)> CanSendAsync(string email, string ipAddress, OtpPurpose purpose)
        {
            var now = DateTime.UtcNow;
            var normalizedEmail = email.Trim().ToLowerInvariant();

            var recent = await _db.OtpSendLogs.AnyAsync(x =>
                x.Email == normalizedEmail && x.Purpose == purpose && x.SentAt > now.AddSeconds(-60));
            if (recent)
            {
                return (false, "Vui lòng chờ 60 giây trước khi gửi lại OTP.");
            }

            var hourlyCount = await _db.OtpSendLogs.CountAsync(x =>
                x.Email == normalizedEmail && x.Purpose == purpose && x.SentAt > now.AddHours(-1));
            if (hourlyCount >= 5)
            {
                return (false, "Email này đã gửi OTP quá nhiều lần trong 1 giờ.");
            }

            var dailyCount = await _db.OtpSendLogs.CountAsync(x =>
                x.Email == normalizedEmail && x.Purpose == purpose && x.SentAt > now.AddDays(-1));
            if (dailyCount >= 10)
            {
                return (false, "Email này đã đạt giới hạn OTP trong ngày.");
            }

            var ipHourlyCount = await _db.OtpSendLogs.CountAsync(x =>
                x.IpAddress == ipAddress && x.Purpose == purpose && x.SentAt > now.AddHours(-1));
            if (ipHourlyCount >= 20)
            {
                return (false, "Thiết bị này gửi OTP quá nhiều lần. Vui lòng thử lại sau.");
            }

            return (true, null);
        }

        public async Task RecordSendAsync(string email, string ipAddress, OtpPurpose purpose)
        {
            _db.OtpSendLogs.Add(new OtpSendLog
            {
                Email = email.Trim().ToLowerInvariant(),
                IpAddress = ipAddress,
                Purpose = purpose
            });
            await _db.SaveChangesAsync();
        }
    }
}
