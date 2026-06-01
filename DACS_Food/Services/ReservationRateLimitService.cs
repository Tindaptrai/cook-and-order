using Microsoft.Extensions.Caching.Memory;

namespace DACS_Food.Services
{
    public class ReservationRateLimitService : IReservationRateLimitService
    {
        private readonly IMemoryCache _cache;

        public ReservationRateLimitService(IMemoryCache cache)
        {
            _cache = cache;
        }

        public (bool Allowed, string Message) CanCreateReservation(string ipAddress, string phoneNumber)
        {
            var ipMinuteKey = $"reservation:ip:minute:{ipAddress}";
            var ipHourKey = $"reservation:ip:hour:{ipAddress}";
            var phoneKey = $"reservation:phone:{NormalizePhone(phoneNumber)}";

            if (GetCount(ipMinuteKey) >= 5)
            {
                return (false, "Bạn gửi yêu cầu đặt bàn quá nhanh. Vui lòng chờ khoảng 1 phút rồi thử lại.");
            }

            if (GetCount(ipHourKey) >= 30)
            {
                return (false, "Thiết bị này gửi quá nhiều yêu cầu đặt bàn trong 1 giờ. Vui lòng thử lại sau.");
            }

            if (GetCount(phoneKey) >= 3)
            {
                return (false, "Số điện thoại này đã gửi nhiều yêu cầu đặt bàn gần đây. Vui lòng chờ thêm trước khi gửi lại.");
            }

            return (true, string.Empty);
        }

        public void RecordCreateReservation(string ipAddress, string phoneNumber)
        {
            Increment($"reservation:ip:minute:{ipAddress}", TimeSpan.FromMinutes(1));
            Increment($"reservation:ip:hour:{ipAddress}", TimeSpan.FromHours(1));
            Increment($"reservation:phone:{NormalizePhone(phoneNumber)}", TimeSpan.FromMinutes(10));
        }

        private int GetCount(string key)
        {
            return _cache.TryGetValue(key, out int count) ? count : 0;
        }

        private void Increment(string key, TimeSpan ttl)
        {
            var count = GetCount(key) + 1;
            _cache.Set(key, count, ttl);
        }

        private static string NormalizePhone(string phoneNumber)
        {
            return new string((phoneNumber ?? string.Empty).Where(char.IsDigit).ToArray());
        }
    }
}
