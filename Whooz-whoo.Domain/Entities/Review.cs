namespace Whooz_whoo.Domain.Entities
{
    public class Review : BaseEntity
    {
        public Guid EventId { get; private set; }
        public Guid UserId { get; private set; }
        public int Rating { get; private set; }
        public string Comment { get; private set; }
        public List<string> Images { get; private set; }
        public bool IsVerified { get; private set; }

        private Review() { }

        public Review(Guid eventId, Guid userId, int rating, string comment, List<string>? images = null)
        {
            Id = Guid.NewGuid();
            EventId = eventId;
            UserId = userId;
            Rating = Math.Clamp(rating, 1, 5);
            Comment = comment;
            Images = images ?? new List<string>();
            IsVerified = false;
            CreatedAt = DateTime.UtcNow;
        }

        public void VerifyReview()
        {
            IsVerified = true;
            UpdateTimestamp();
        }
    }
}