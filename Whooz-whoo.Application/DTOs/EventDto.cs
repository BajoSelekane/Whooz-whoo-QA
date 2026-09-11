using System;
using System.Collections.Generic;
using System.Text;
using Whooz_whoo.Domain.Enums;

namespace Whooz_whoo.Application.DTOs
{
    public class EventDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public EventType Type { get; set; }
        public decimal Price { get; set; }
        public DateTime StartDateTime { get; set; }
        public DateTime EndDateTime { get; set; }
        public LocationDto Location { get; set; }
        public Guid OrganizerId { get; set; }
        public string OrganizerName { get; set; }
        public int MaxCapacity { get; set; }
        public int CurrentAttendees { get; set; }
        public bool IsLiveStreaming { get; set; }
        public string StreamUrl { get; set; }
        public List<string> Categories { get; set; }
        public List<string> Tags { get; set; }
        public EventStatus Status { get; set; }
        public double AverageRating { get; set; }
        public int TotalReviews { get; set; }
        public string ImageUrl { get; set; }
        public string GalleryImages { get; set; }
        public double DistanceFromUser { get; set; }
        public bool IsUserAttending { get; set; }
    }
}
