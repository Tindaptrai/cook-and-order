using DACS_Food.Data;
using DACS_Food.Models;
using DACS_Food.Services;
using DACS_Food.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DACS_Food.Controllers
{
    [ApiController]
    public class ChatbotController : Controller
    {
        private readonly IGeminiChatService _chatService;
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;

        public ChatbotController(IGeminiChatService chatService, ApplicationDbContext db, UserManager<ApplicationUser> userManager)
        {
            _chatService = chatService;
            _db = db;
            _userManager = userManager;
        }

        [HttpGet("/api/chatbot/history")]
        public async Task<ActionResult<ChatHistoryResponse>> History([FromQuery] string? sessionId)
        {
            var userId = await GetUserIdAsync();
            if (string.IsNullOrWhiteSpace(userId) && string.IsNullOrWhiteSpace(sessionId))
            {
                return Ok(new ChatHistoryResponse());
            }

            var query = _db.ChatMessages.AsNoTracking().AsQueryable();
            query = !string.IsNullOrWhiteSpace(userId)
                ? query.Where(x => x.UserId == userId || x.SessionId == sessionId)
                : query.Where(x => x.SessionId == sessionId);

            var messages = await query
                .OrderBy(x => x.CreatedAt)
                .Take(200)
                .Select(x => new ChatMessageDto
                {
                    Role = x.Role,
                    Message = x.Message,
                    Intent = x.Intent,
                    CreatedAt = x.CreatedAt,
                    PageUrl = x.PageUrl,
                    MetadataJson = x.MetadataJson
                })
                .ToListAsync();

            return Ok(new ChatHistoryResponse
            {
                Messages = messages,
                LoadedFromDatabase = !string.IsNullOrWhiteSpace(userId)
            });
        }

        [HttpPost("/api/chatbot/ask")]
        public async Task<ActionResult<ChatbotResponse>> Ask([FromBody] ChatbotRequest request)
        {
            var sessionId = NormalizeSessionId(request.SessionId);
            var userId = await GetUserIdAsync();

            if (!string.IsNullOrWhiteSpace(userId))
            {
                await MergeClientHistoryAsync(userId, sessionId, request.ClientHistory);
            }

            var userMessage = (request.Message ?? string.Empty).Trim();
            if (!string.IsNullOrWhiteSpace(userMessage))
            {
                await SaveMessageAsync(userId, sessionId, "user", userMessage, null, request.PageUrl, null);
            }

            var response = await _chatService.AskAsync(new ChatbotRequest
            {
                Message = userMessage,
                SessionId = sessionId,
                PageUrl = request.PageUrl
            }, HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown");

            if (!string.IsNullOrWhiteSpace(response.Reply))
            {
                await SaveMessageAsync(userId, sessionId, "assistant", response.Reply, response.Intent, request.PageUrl, null);
            }

            return Ok(response);
        }

        [HttpPost("/api/chatbot/clear")]
        public async Task<IActionResult> Clear([FromBody] ChatbotRequest request)
        {
            var sessionId = NormalizeSessionId(request.SessionId);
            var userId = await GetUserIdAsync();
            var query = _db.ChatMessages.AsQueryable();
            query = !string.IsNullOrWhiteSpace(userId)
                ? query.Where(x => x.UserId == userId || x.SessionId == sessionId)
                : query.Where(x => x.SessionId == sessionId);

            _db.ChatMessages.RemoveRange(await query.ToListAsync());
            await _db.SaveChangesAsync();
            return Ok(new { success = true });
        }

        [HttpPost("/Chatbot/Ask")]
        public async Task<ActionResult<ChatbotResponse>> LegacyAsk([FromBody] ChatbotRequest request)
        {
            return await Ask(request);
        }

        private async Task<string?> GetUserIdAsync()
        {
            var user = User.Identity?.IsAuthenticated == true ? await _userManager.GetUserAsync(User) : null;
            return user?.Id;
        }

        private static string NormalizeSessionId(string? sessionId)
        {
            return string.IsNullOrWhiteSpace(sessionId) ? $"server-{Guid.NewGuid():N}" : sessionId.Trim()[..Math.Min(sessionId.Trim().Length, 128)];
        }

        private async Task MergeClientHistoryAsync(string userId, string sessionId, IReadOnlyList<ChatMessageDto>? clientHistory)
        {
            if (clientHistory == null || clientHistory.Count == 0) return;

            var existing = await _db.ChatMessages
                .Where(x => x.UserId == userId || x.SessionId == sessionId)
                .Select(x => new { x.Role, x.Message })
                .ToListAsync();
            var existingKeys = existing.Select(x => $"{x.Role}|{x.Message}").ToHashSet(StringComparer.Ordinal);

            foreach (var item in clientHistory.TakeLast(100))
            {
                var role = item.Role == "assistant" ? "assistant" : "user";
                var message = (item.Message ?? string.Empty).Trim();
                if (string.IsNullOrWhiteSpace(message)) continue;

                var key = $"{role}|{message}";
                if (existingKeys.Contains(key)) continue;
                existingKeys.Add(key);

                _db.ChatMessages.Add(new ChatMessage
                {
                    UserId = userId,
                    SessionId = sessionId,
                    Role = role,
                    Message = message.Length > 2000 ? message[..2000] : message,
                    Intent = item.Intent,
                    CreatedAt = item.CreatedAt == default ? DateTime.UtcNow : item.CreatedAt.ToUniversalTime(),
                    PageUrl = item.PageUrl,
                    MetadataJson = item.MetadataJson
                });
            }

            await _db.SaveChangesAsync();
        }

        private async Task SaveMessageAsync(string? userId, string sessionId, string role, string message, string? intent, string? pageUrl, string? metadataJson)
        {
            var lastDuplicate = await _db.ChatMessages
                .Where(x => x.UserId == userId && x.SessionId == sessionId && x.Role == role && x.Message == message)
                .OrderByDescending(x => x.CreatedAt)
                .FirstOrDefaultAsync();

            if (lastDuplicate != null && lastDuplicate.CreatedAt > DateTime.UtcNow.AddSeconds(-10))
            {
                return;
            }

            _db.ChatMessages.Add(new ChatMessage
            {
                UserId = userId,
                SessionId = sessionId,
                Role = role,
                Message = message.Length > 2000 ? message[..2000] : message,
                Intent = intent,
                CreatedAt = DateTime.UtcNow,
                PageUrl = pageUrl,
                MetadataJson = metadataJson
            });
            await _db.SaveChangesAsync();
        }
    }
}
