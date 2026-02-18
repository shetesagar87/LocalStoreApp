using CleanMvcApp.Models.Enums;

namespace CleanMvcApp.Models.Entities
{
    public class Notification
    {
        public int NotificationId { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
        public NotificationStatus Status { get; set; } = NotificationStatus.Pending;
        public int RetryCount { get; set; } = 0;
        public DateTime? SentAt { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public ApplicationUser User { get; set; } = null!;
    }
}
