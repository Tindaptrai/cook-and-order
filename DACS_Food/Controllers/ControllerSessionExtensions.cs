using Microsoft.AspNetCore.Mvc;

namespace DACS_Food.Controllers
{
    public static class ControllerSessionExtensions
    {
        private const string SessionIdKey = "FoodieTTTMSessionId";
        private const string RecentOrderCodesKey = "FoodieTTTMRecentOrderCodes";

        public static string GetSessionId(this ControllerBase controller)
        {
            var sessionId = controller.HttpContext.Session.GetString(SessionIdKey);
            if (!string.IsNullOrWhiteSpace(sessionId))
            {
                return sessionId;
            }

            sessionId = Guid.NewGuid().ToString("N");
            controller.HttpContext.Session.SetString(SessionIdKey, sessionId);
            return sessionId;
        }

        public static void RememberOrderCode(this ControllerBase controller, string orderCode)
        {
            if (string.IsNullOrWhiteSpace(orderCode)) return;

            var codes = GetRecentOrderCodes(controller).ToList();
            if (!codes.Contains(orderCode, StringComparer.OrdinalIgnoreCase))
            {
                codes.Add(orderCode.Trim());
            }

            controller.HttpContext.Session.SetString(RecentOrderCodesKey, string.Join("|", codes.TakeLast(20)));
        }

        public static bool HasRecentOrderCode(this ControllerBase controller, string orderCode)
        {
            if (string.IsNullOrWhiteSpace(orderCode)) return false;
            return GetRecentOrderCodes(controller).Contains(orderCode.Trim(), StringComparer.OrdinalIgnoreCase);
        }

        private static IEnumerable<string> GetRecentOrderCodes(ControllerBase controller)
        {
            var raw = controller.HttpContext.Session.GetString(RecentOrderCodesKey) ?? string.Empty;
            return raw.Split('|', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        }
    }
}

