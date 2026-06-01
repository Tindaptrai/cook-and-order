using DACS_Food.Models;
using DACS_Food.Services;
using DACS_Food.ViewModels;
using DACS_Food.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System.Text;

namespace DACS_Food.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IOtpService _otpService;
        private readonly ApplicationDbContext _db;
        private readonly ILoyaltyService _loyaltyService;
        private readonly IWebHostEnvironment _environment;
        private readonly IEmailSender _emailSender;
        private readonly IMemoryCache _cache;

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IOtpService otpService, ApplicationDbContext db, ILoyaltyService loyaltyService, IWebHostEnvironment environment, IEmailSender emailSender, IMemoryCache cache)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _otpService = otpService;
            _db = db;
            _loyaltyService = loyaltyService;
            _environment = environment;
            _emailSender = emailSender;
            _cache = cache;
        }

        [HttpGet("/dang-nhap")]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost("/dang-nhap")]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null || !user.IsEmailVerified)
            {
                ModelState.AddModelError(string.Empty, "Tài khoản chưa tồn tại hoặc chưa xác thực OTP.");
                return View(model);
            }

            var result = await _signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe, lockoutOnFailure: true);
            if (!result.Succeeded)
            {
                ModelState.AddModelError(string.Empty, "Email hoặc mật khẩu không đúng.");
                return View(model);
            }

            if (await _userManager.IsInRoleAsync(user, "Admin"))
            {
                return Redirect("/admin");
            }

            return RedirectToAction("Index", "Home");
        }

        [HttpGet("/Account/ForgotPassword")]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost("/Account/ForgotPassword")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var normalizedEmail = model.Email.Trim().ToLowerInvariant();
            var ipAddress = GetIpAddress();
            var rateLimitAllowed = CheckForgotPasswordRateLimit(normalizedEmail, ipAddress);
            if (rateLimitAllowed)
            {
                var user = await _userManager.FindByEmailAsync(normalizedEmail);
                if (user != null && user.IsEmailVerified)
                {
                    try
                    {
                        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                        var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
                        var resetUrl = Url.Action(nameof(ResetPassword), "Account", new
                        {
                            email = user.Email,
                            token = encodedToken
                        }, Request.Scheme);

                        if (!string.IsNullOrWhiteSpace(resetUrl))
                        {
                            await _emailSender.SendAsync(user.Email!, "FoodieLab - Đặt lại mật khẩu", BuildResetPasswordEmail(user.FullName, resetUrl));
                        }
                    }
                    catch
                    {
                        // Không lộ trạng thái SMTP hoặc email có tồn tại hay không.
                    }
                }
            }

            return RedirectToAction(nameof(ForgotPasswordConfirmation));
        }

        [HttpGet("/Account/ForgotPasswordConfirmation")]
        public IActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        [HttpGet("/Account/ResetPassword")]
        public IActionResult ResetPassword(string? email, string? token)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(token))
            {
                return View("ResetPasswordConfirmation", "Liên kết đặt lại mật khẩu không hợp lệ hoặc đã hết hạn.");
            }

            return View(new ResetPasswordViewModel
            {
                Email = email,
                Token = token
            });
        }

        [HttpPost("/Account/ResetPassword")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.FindByEmailAsync(model.Email.Trim());
            if (user == null)
            {
                ViewBag.ResetPasswordError = "Liên kết đặt lại mật khẩu không hợp lệ hoặc đã hết hạn.";
                return View(model);
            }

            string token;
            try
            {
                token = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(model.Token));
            }
            catch
            {
                ViewBag.ResetPasswordError = "Liên kết đặt lại mật khẩu không hợp lệ hoặc đã hết hạn.";
                return View(model);
            }

            var result = await _userManager.ResetPasswordAsync(user, token, model.NewPassword);
            if (!result.Succeeded)
            {
                ViewBag.ResetPasswordError = "Liên kết đặt lại mật khẩu không hợp lệ hoặc đã hết hạn.";
                return View(model);
            }

            TempData["LoginMessage"] = "Đặt lại mật khẩu thành công. Vui lòng đăng nhập bằng mật khẩu mới.";
            return RedirectToAction(nameof(Login));
        }

        [HttpGet("/Account/ResetPasswordConfirmation")]
        public IActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        [HttpGet("/dang-ky")]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost("/dang-ky")]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                FullName = model.FullName,
                IsEmailVerified = false
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View(model);
            }

            await _otpService.SendOtpAsync(user.Id, user.Email, OtpPurpose.Register, GetIpAddress());
            return RedirectToAction(nameof(VerifyOtp), new { email = user.Email, purpose = OtpPurpose.Register });
        }

        [HttpGet("/xac-thuc-otp")]
        public IActionResult VerifyOtp(string email, OtpPurpose purpose = OtpPurpose.Register)
        {
            return View(new VerifyOtpViewModel { Email = email, Purpose = purpose });
        }

        [HttpPost("/xac-thuc-otp")]
        public async Task<IActionResult> VerifyOtp(VerifyOtpViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await _otpService.VerifyAsync(model.Email, model.Code, model.Purpose);
            if (!result.Success)
            {
                ModelState.AddModelError(string.Empty, result.Message ?? "Xác thực OTP thất bại.");
                return View(model);
            }

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user != null)
            {
                user.IsEmailVerified = true;
                user.EmailConfirmed = true;
                await _userManager.UpdateAsync(user);
                await _signInManager.SignInAsync(user, isPersistent: false);
            }

            return RedirectToAction("Index", "Home");
        }

        [HttpPost("/gui-lai-otp")]
        public async Task<IActionResult> ResendOtp(string email, OtpPurpose purpose = OtpPurpose.Register)
        {
            var user = await _userManager.FindByEmailAsync(email);
            var result = await _otpService.SendOtpAsync(user?.Id, email, purpose, GetIpAddress());
            TempData[result.Success ? "OtpMessage" : "OtpError"] = result.Message;
            return RedirectToAction(nameof(VerifyOtp), new { email, purpose });
        }

        [HttpPost("/dang-xuat")]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [Authorize]
        [HttpGet("/tai-khoan")]
        public async Task<IActionResult> Profile()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction(nameof(Login));

            var orderCount = await _db.Orders.CountAsync(x => x.UserId == user.Id);
            var totalSpent = await _loyaltyService.GetEligibleTotalSpentAsync(user.Id);
            user.LoyaltyLevel = await _loyaltyService.RefreshUserLevelAsync(user.Id);

            ViewBag.OrderCount = orderCount;
            ViewBag.TotalSpent = totalSpent;
            ViewBag.NextLevel = _loyaltyService.GetNextLevel(user.LoyaltyLevel);
            ViewBag.NextLevelTarget = _loyaltyService.GetNextLevelTarget(user.LoyaltyLevel);
            return View(user);
        }

        [Authorize]
        [HttpPost("/tai-khoan/avatar")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateAvatar(IFormFile avatar)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction(nameof(Login));

            if (avatar == null || avatar.Length == 0)
            {
                TempData["AvatarError"] = "Vui lòng chọn ảnh avatar.";
                return RedirectToAction(nameof(Profile));
            }

            if (avatar.Length > 2 * 1024 * 1024)
            {
                TempData["AvatarError"] = "Ảnh avatar không được vượt quá 2MB.";
                return RedirectToAction(nameof(Profile));
            }

            var allowedExtensions = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                ".jpg", ".jpeg", ".png", ".webp", ".gif"
            };
            var extension = Path.GetExtension(avatar.FileName);
            if (string.IsNullOrWhiteSpace(extension) || !allowedExtensions.Contains(extension))
            {
                TempData["AvatarError"] = "Chỉ hỗ trợ ảnh JPG, PNG, WEBP hoặc GIF.";
                return RedirectToAction(nameof(Profile));
            }

            var uploadRoot = Path.Combine(_environment.WebRootPath, "uploads", "avatars");
            Directory.CreateDirectory(uploadRoot);

            var fileName = $"{user.Id}-{DateTime.UtcNow:yyyyMMddHHmmss}{extension.ToLowerInvariant()}";
            var filePath = Path.Combine(uploadRoot, fileName);

            await using (var stream = System.IO.File.Create(filePath))
            {
                await avatar.CopyToAsync(stream);
            }

            user.AvatarUrl = $"/uploads/avatars/{fileName}";
            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                TempData["AvatarError"] = "Không thể cập nhật avatar. Vui lòng thử lại.";
                return RedirectToAction(nameof(Profile));
            }

            TempData["AvatarMessage"] = "Đã cập nhật avatar cá nhân.";
            return RedirectToAction(nameof(Profile));
        }

        [Authorize]
        [HttpGet("/lich-su-don-hang")]
        public async Task<IActionResult> OrderHistory()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction(nameof(Login));

            var orders = await _db.Orders
                .Include(x => x.Items)
                .Include(x => x.Payment)
                .Where(x => x.UserId == user.Id)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();

            return View(orders);
        }

        private string GetIpAddress()
        {
            return HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        }

        private bool CheckForgotPasswordRateLimit(string email, string ipAddress)
        {
            var emailKey = $"forgot-password:email:{email}";
            var ipKey = $"forgot-password:ip:{ipAddress}";
            var emailCount = _cache.GetOrCreate(emailKey, entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(15);
                return 0;
            });
            var ipCount = _cache.GetOrCreate(ipKey, entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(15);
                return 0;
            });

            if (emailCount >= 3 || ipCount >= 8)
            {
                return false;
            }

            _cache.Set(emailKey, emailCount + 1, TimeSpan.FromMinutes(15));
            _cache.Set(ipKey, ipCount + 1, TimeSpan.FromMinutes(15));
            return true;
        }

        private static string BuildResetPasswordEmail(string fullName, string resetUrl)
        {
            var displayName = string.IsNullOrWhiteSpace(fullName) ? "bạn" : fullName;
            return $"""
Chào {displayName},

FoodieLab vừa nhận được yêu cầu đặt lại mật khẩu cho tài khoản của bạn.

Bấm vào liên kết bên dưới để đặt lại mật khẩu:
{resetUrl}

Liên kết này sẽ hết hạn sau 30 phút và chỉ có thể dùng một lần.

Nếu bạn không yêu cầu đặt lại mật khẩu, hãy bỏ qua email này.

FoodieLab
""";
        }

        private static string GetNextLoyaltyLevel(string currentLevel)
        {
            currentLevel = currentLevel switch
            {
                "Báº¡c" or "BÃ¡ÂºÂ¡c" => "Bạc",
                "VÃ ng" => "Vàng",
                "Báº¡ch kim" or "BÃ¡ÂºÂ¡ch kim" => "Bạch kim",
                _ => currentLevel
            };

            return currentLevel switch
            {
                "Bạc" => "Vàng",
                "Vàng" => "Bạch kim",
                "Bạch kim" => "Hạng cao nhất",
                _ => "Bạc"
            };
        }
    }
}
