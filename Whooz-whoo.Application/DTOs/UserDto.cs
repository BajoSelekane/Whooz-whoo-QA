using System;
using System.Collections.Generic;
using System.Text;

namespace Whooz_whoo.Application.DTOs
{
    public class UserDto
    {
        public Guid Id { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName => $"{FirstName} {LastName}";
        public string PhoneNumber { get; set; }
        public string ProfileImageUrl { get; set; }
        public string Bio { get; set; }
        public LocationDto HomeLocation { get; set; }
        public bool IsOrganizer { get; set; }
        public bool IsVerified { get; set; }
        public MembershipDto CurrentMembership { get; set; }
        public List<string> Interests { get; set; }
        public int EventsAttended { get; set; }
        public int ReviewsWritten { get; set; }
        public DateTime? LastLoginDate { get; set; }
    }

}
