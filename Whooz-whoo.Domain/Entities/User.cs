using System;
using System.Collections.Generic;
using System.Text;
using Whooz_whoo.Domain.Common;

namespace Whooz_whoo.Domain.Entities
{
    public class User : AggregateRoot
    {
        public string Email { get; private set; }
        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public string PhoneNumber { get; private set; }
        public string ProfileImageUrl { get; private set; }
        public string Bio { get; private set; }
        public Location HomeLocation { get; private set; }
        public bool IsOrganizer { get; private set; }
        public bool IsVerified { get; private set; }
        public DateTime? LastLoginDate { get; private set; }
        public List<UserInterest> Interests { get; private set; }
        public List<Review> Reviews { get; private set; }
        public List<Event> EventsAttending { get; private set; }
        public List<Notification> Notifications { get; private set; }

        private User() { }

        public User(string email, string firstName, string lastName, string phoneNumber, Location homeLocation)
        {
            Id = Guid.NewGuid();
            Email = email;
            FirstName = firstName;
            LastName = lastName;
            PhoneNumber = phoneNumber;
            HomeLocation = homeLocation;
            IsVerified = false;
            IsOrganizer = false;
            Interests = new List<UserInterest>();
            Reviews = new List<Review>();
            EventsAttending = new List<Event>();
            Notifications = new List<Notification>();
            CreatedAt = DateTime.UtcNow;

            AddDomainEvent(new UserCreatedEvent(this));
        }

        public void UpdateProfile(string firstName, string lastName, string phoneNumber, string bio, string profileImageUrl)
        {
            FirstName = firstName;
            LastName = lastName;
            PhoneNumber = phoneNumber;
            Bio = bio;
            ProfileImageUrl = profileImageUrl;
            UpdateTimestamp();
        }

        public void UpdateLocation(Location location)
        {
            HomeLocation = location;
            UpdateTimestamp();
        }

        public void VerifyUser()
        {
            IsVerified = true;
            UpdateTimestamp();

            AddDomainEvent(new UserVerifiedEvent(this));
        }

        public void AddInterest(string category, int level)
        {
            var interest = new UserInterest(category, level);
            Interests.Add(interest);
            UpdateTimestamp();
        }

        public void RemoveInterest(string category)
        {
            var interest = Interests.FirstOrDefault(i => i.Category == category);
            if (interest != null)
            {
                Interests.Remove(interest);
                UpdateTimestamp();
            }
        }

        public void RecordLogin()
        {
            LastLoginDate = DateTime.UtcNow;
            UpdateTimestamp();
        }
    }
}
