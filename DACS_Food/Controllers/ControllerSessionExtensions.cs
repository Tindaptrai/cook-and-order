using Microsoft.AspNetCore.Mvc;

namespace DACS_Food.Controllers
{
    public static class ControllerSessionExtensions
    {
        public static string GetSessionId(this Controller controller)
        {
            const string key = "FoodieTTTMSessionId";
            var sessionId = controller.HttpContext.Session.GetString(key);
            if (!string.IsNullOrWhiteSpace(sessionId))
            {
                return sessionId;
            }

            sessionId = Guid.NewGuid().ToString("N");
            controller.HttpContext.Session.SetString(key, sessionId);
            return sessionId;
        }
    }
}

