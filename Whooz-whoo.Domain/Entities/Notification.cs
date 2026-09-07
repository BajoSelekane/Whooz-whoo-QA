using Whooz_whoo.Domain.Enums;

namespace Whooz_whoo.Domain.Entities
{
    public class Notification : BaseEntity
    {
        public Guid UserId { get; private set; }
        public string Title { get; private set; }
        public string Message { get; private set; }
        public NotificationType Type { get; private set; }
        public bool IsRead { get; private set; }
        public DateTime? ReadAt { get; private set; }
        public string ActionUrl { get; private set; }
        public Dictionary<string, string> Metadata { get; private set; }

        private Notification() { }

        public Notification(Guid userId, string title, string message, NotificationType type, string actionUrl = null)
        {
            Id = Guid.NewGuid();
            UserId = userId;
            Title = title;
            Message = message;
            Type = type;
            IsRead = false;
            ActionUrl = actionUrl;
            Metadata = new Dictionary<string, string>();
            CreatedAt = DateTime.UtcNow;
        }

        public void MarkAsRead()
        {
            IsRead = true;
            ReadAt = DateTime.UtcNow;
            UpdateTimestamp();
        }
    }
}