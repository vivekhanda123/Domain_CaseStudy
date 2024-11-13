namespace SimplyFly_Backend.Models
{
    public class Payment
    {
        public int PaymentId { get; set; }
        public int BookingId { get; set; }
        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; } // e.g., "Credit Card", "PayPal"
        public DateTime PaymentDate { get; set; }
        public string Status { get; set; } // e.g., "Completed", "Pending"

        // Navigation properties
        public Booking? Booking { get; set; }

        // Change the type of UserId to string to match IdentityUser's primary key type
        public string UserId { get; set; }  // Changed to string to match User.Id (IdentityUser)
        public User User { get; set; }
    }
}
