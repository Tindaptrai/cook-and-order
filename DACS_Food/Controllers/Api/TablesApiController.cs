using DACS_Food.Models;
using DACS_Food.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DACS_Food.Controllers.Api
{
    [ApiController]
    [Route("api/tables")]
    public class TablesApiController : ControllerBase
    {
        private readonly ITableService _tableService;
        private readonly IReservationRateLimitService _reservationRateLimitService;

        public TablesApiController(ITableService tableService, IReservationRateLimitService reservationRateLimitService)
        {
            _tableService = tableService;
            _reservationRateLimitService = reservationRateLimitService;
        }

        [HttpGet]
        public async Task<IActionResult> GetTables()
        {
            return Ok(await _tableService.GetActiveTablesAsync());
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id:int}/status")]
        public async Task<IActionResult> UpdateStatus(int id, TableStatus status)
        {
            var updated = await _tableService.UpdateStatusAsync(id, status);
            if (!updated) return NotFound(new { message = "Không tìm thấy bàn." });
            return Ok(new { message = "Đã cập nhật trạng thái bàn." });
        }

        [HttpGet("{id:int}/slots")]
        public async Task<IActionResult> GetSlots(int id, DateOnly date)
        {
            return Ok(await _tableService.GetReservationSlotsAsync(date, id));
        }

        [HttpPost("reservations")]
        public async Task<IActionResult> CreateReservation([FromBody] ViewModels.CreateTableReservationViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    success = false,
                    message = string.Join(" ", ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage))
                });
            }

            model.Status = ReservationStatus.Pending;
            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
            var limit = _reservationRateLimitService.CanCreateReservation(ipAddress, model.PhoneNumber);
            if (!limit.Allowed)
            {
                return StatusCode(StatusCodes.Status429TooManyRequests, new { success = false, message = limit.Message });
            }

            var result = await _tableService.CreateReservationAsync(model);
            if (!result.Success)
            {
                return BadRequest(new { success = false, result.Message });
            }

            _reservationRateLimitService.RecordCreateReservation(ipAddress, model.PhoneNumber);
            return Ok(new
            {
                success = true,
                message = "FoodieTTTM đã nhận thông tin đặt bàn của bạn. Nhân viên nhà hàng sẽ liên hệ lại để xác nhận bàn và thời gian.",
                detail = result.Message
            });
        }
    }
}

