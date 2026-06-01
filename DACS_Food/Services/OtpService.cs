using System.Security.Cryptography;
using System.Text;
using DACS_Food.Data;
using DACS_Food.Models;
using Microsoft.EntityFrameworkCore;

namespace DACS_Food.Services
{
    public class OtpService : IOtpService
    {
        private readonly ApplicationDbContext _db;
        private readonly IOtpRateLimitService _rateLimitService;
        private readonly IEmailSender _emailSender;

        public OtpService(ApplicationDbContext db, IOtpRateLimitService rateLimitService, IEmailSender emailSender)
        {
            _db = db;
            _rateLimitService = rateLimitService;
            _emailSender = emailSender;
        }

        public async Task<(bool Success, string? Message)> SendOtpAsync(string? userId, string email, OtpPurpose purpose, string ipAddress)
        {
            var normalizedEmail = email.Trim().ToLowerInvariant();
            var rateLimit = await _rateLimitService.CanSendAsync(normalizedEmail, ipAddress, purpose);
            if (!rateLimit.Allowed)
            {
                return (false, rateLimit.Message);
            }

            var code = RandomNumberGenerator.GetInt32(10000000, 100000000).ToString();
            _db.EmailOtps.Add(new EmailOtp
            {
                UserId = userId,
                Email = normalizedEmail,
                CodeHash = Hash(code),
                Purpose = purpose,
                ExpiresAt = DateTime.UtcNow.AddMinutes(5)
            });

            await _rateLimitService.RecordSendAsync(normalizedEmail, ipAddress, purpose);
            await _db.SaveChangesAsync();
            await _emailSender.SendAsync(normalizedEmail, "Mã OTP FoodieTTTM", $"Mã OTP của bạn là: {code}. Mã có hiệu lực trong 5 phút.");

            return (true, "OTP đã được gửi về Gmail.");
        }

        public async Task<(bool Success, string? Message)> VerifyAsync(string email, string code, OtpPurpose purpose)
        {
            var normalizedEmail = email.Trim().ToLowerInvariant();
            var normalizedCode = code.Trim();
            if (normalizedCode.Length != 8 || !normalizedCode.All(char.IsDigit))
            {
                return (false, "Mã OTP phải gồm đúng 8 chữ số.");
            }

            var otp = await _db.EmailOtps
                .Where(x => x.Email == normalizedEmail && x.Purpose == purpose && x.UsedAt == null)
                .OrderByDescending(x => x.CreatedAt)
                .FirstOrDefaultAsync();

            if (otp == null)
            {
                return (false, "Không tìm thấy OTP hợp lệ.");
            }

            if (otp.ExpiresAt < DateTime.UtcNow)
            {
                return (false, "OTP đã hết hạn.");
            }

            if (otp.FailedAttempts >= 5)
            {
                return (false, "OTP đã bị khóa do nhập sai quá nhiều lần.");
            }

            if (otp.CodeHash != Hash(normalizedCode))
            {
                otp.FailedAttempts += 1;
                await _db.SaveChangesAsync();
                return (false, "Mã OTP không đúng.");
            }

            otp.UsedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();
            return (true, "Xác thực OTP thành công.");
        }

        private static string Hash(string value)
        {
            var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(value));
            return Convert.ToHexString(bytes);
        }
    }
}

