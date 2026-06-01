namespace DACS_Food.Models
{
    public class ChatMessage
    {
        public int Id { get; set; }
        public string? UserId { get; set; }
        public string SessionId { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string? Intent { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string? PageUrl { get; set; }
        public string? MetadataJson { get; set; }

        public ApplicationUser? User { get; set; }
    }
}
