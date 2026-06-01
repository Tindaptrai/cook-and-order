using DACS_Food.Services;
using DACS_Food.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace DACS_Food.Controllers.Api
{
    [ApiController]
    [Route("api/chatbot-legacy")]
    public class ChatbotApiController : ControllerBase
    {
        private readonly IGeminiChatService _geminiChatService;

        public ChatbotApiController(IGeminiChatService geminiChatService)
        {
            _geminiChatService = geminiChatService;
        }

        [HttpPost("ask")]
        public async Task<ActionResult<ChatbotResponse>> Ask([FromBody] ChatbotRequest request)
        {
            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
            var response = await _geminiChatService.AskAsync(request ?? new ChatbotRequest(), ipAddress);
            return Ok(response);
        }
    }
}
