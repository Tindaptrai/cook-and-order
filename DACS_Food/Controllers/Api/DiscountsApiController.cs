using DACS_Food.Services;
using Microsoft.AspNetCore.Mvc;

namespace DACS_Food.Controllers.Api
{
    [ApiController]
    [Route("api/discounts")]
    public class DiscountsApiController : ControllerBase
    {
        private readonly IDiscountService _discountService;

        public DiscountsApiController(IDiscountService discountService)
        {
            _discountService = discountService;
        }

        [HttpGet("golden-hour")]
        public IActionResult CheckGoldenHour()
        {
            var now = DateTime.Now;
            return Ok(new
            {
                Now = now,
                IsGoldenHour = _discountService.IsGoldenHour(now),
                Windows = new[] { "10:30-12:30", "16:00-18:00" }
            });
        }

        [HttpGet("calculate")]
        public async Task<IActionResult> Calculate(string code, decimal subtotal, string? userId)
        {
            var result = await _discountService.CalculateDiscountAsync(code, subtotal, userId);
            return Ok(new
            {
                Valid = result.Code != null && string.IsNullOrWhiteSpace(result.Message),
                DiscountCode = result.Code?.Code,
                DiscountAmount = result.Amount,
                result.Message
            });
        }
    }
}
