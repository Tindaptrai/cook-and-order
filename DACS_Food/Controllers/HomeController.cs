using Microsoft.AspNetCore.Mvc;
using DACS_Food.Services;

namespace DACS_Food.Controllers
{
    public class HomeController : Controller
    {
        private readonly ITableService _tableService;
        private readonly IEmailSender _emailSender;
        private readonly IConfiguration _configuration;

        public HomeController(ITableService tableService, IEmailSender emailSender, IConfiguration configuration)
        {
            _tableService = tableService;
            _emailSender = emailSender;
            _configuration = configuration;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.Tables = (await _tableService.GetTablePageAsync()).Tables;
            return View();
        }

        public IActionResult Menu(int page = 1)
        {
            if (page < 1) page = 1;
            if (page > 5) page = 5;
            ViewBag.Page = page;
            return View();
        }

        public IActionResult Recipes()
        {
            return View();
        }

        public IActionResult Tips()
        {
            return View();
        }

        public IActionResult Tables()
        {
            return RedirectToAction("Index", "Table");
        }

        public IActionResult Auth()
        {
            return RedirectToAction("Login", "Account");
        }

        public IActionResult Checkout()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Terms()
        {
            return View();
        }

        [HttpGet("/test-email")]
        public async Task<IActionResult> TestEmail()
        {
            var toEmail = _configuration["Smtp:UserName"];
            if (string.IsNullOrWhiteSpace(toEmail))
            {
                return Content("SMTP username is not configured.");
            }

            try
            {
                await _emailSender.SendAsync(toEmail, "FoodieLab SMTP Test", "Gửi email thành công từ hệ thống FoodieLab.");
                return Content("Email sent successfully");
            }
            catch (Exception ex)
            {
                return Content($"Email send failed: {SanitizeEmailError(ex.Message, _configuration["Smtp:Password"])}");
            }
        }

        private static string SanitizeEmailError(string message, string? password)
        {
            if (string.IsNullOrWhiteSpace(message)) return "SMTP error.";
            if (!string.IsNullOrWhiteSpace(password))
            {
                message = message.Replace(password, "[hidden]", StringComparison.Ordinal);
            }
            return message.Length > 180 ? message[..180] : message;
        }
    }
}
