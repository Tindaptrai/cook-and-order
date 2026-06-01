namespace DACS_Food.Models
{
    public class OtpSendLog
    {
        public int Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string IpAddress { get; set; } = string.Empty;
        public OtpPurpose Purpose { get; set; }
        public DateTime SentAt { get; set; } = DateTime.UtcNow;
    }
}
