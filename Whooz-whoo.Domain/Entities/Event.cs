using System;
using System.Collections.Generic;
using System.Text;
using Whooz_whoo.Domain.Enums;

namespace Whooz_whoo.Domain.Entities
{
    public class Event : BaseEntity
    {
        public string Title { get; private set; }
        public string Description { get; private set; }
        public EventType Type { get; private set; }
        public decimal Price { get; private set; }
        public DateTime StartDateTime { get; private set; }
        public DateTime EndDateTime { get; private set; }
        public Location Location { get; private set; }
        public Guid OrganizerId { get; private set; }
        public int MaxCapacity { get; private set; }
        public bool IsLiveStreaming { get; private set; }
        public string StreamUrl { get; private set; }
        public List<EventCategory> Categories { get; private set; }
        public List<EventTag> Tags { get; private set; }
        public EventStatus Status { get; private set; }
        public double AverageRating { get; private set; }
        public int TotalReviews { get; private set; }

        private Event() { }

        public Event(string title, string description, EventType type, decimal price,
                     DateTime startDateTime, DateTime endDateTime, Location location,
                     Guid organizerId, int maxCapacity, bool isLiveStreaming = false)
        {
            Title = title;
            Description = description;
            Type = type;
            Price = price;
            StartDateTime = startDateTime;
            EndDateTime = endDateTime;
            Location = location;
            OrganizerId = organizerId;
            MaxCapacity = maxCapacity;
            IsLiveStreaming = isLiveStreaming;
            Status = EventStatus.Active;
            Categories = [];
            Tags = [];
        }
    }
}
