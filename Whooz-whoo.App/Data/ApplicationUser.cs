using Microsoft.AspNetCore.Identity;

namespace Whooz_whoo.App.Data
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {
        public string? Surname { get; set; }

        private long phoneNumber;

        public long GetPhoneNumber()
        {
            return phoneNumber;
        }

        public void SetPhoneNumber(long value)
        {
            phoneNumber = value;
        }
        public string? Role { get; set; } // User, Vendor, Admin
    }

}
