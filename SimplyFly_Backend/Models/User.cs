using Microsoft.AspNetCore.Identity;

namespace SimplyFly_Backend.Models
{
    public class User : IdentityUser
    {
        public string Name { get; set; }
        public string Gender { get; set; }
        public string ContactNumber { get; set; }
        public string Address { get; set; }
        public string Role { get; set; } // For roles like "Admin", "User", "Flight Owner", etc.

        // Navigation properties
        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
        public ICollection<Payment> Payments { get; set; } = new List<Payment>();

    }
}
