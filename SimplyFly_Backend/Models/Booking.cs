namespace SimplyFly_Backend.Models
{
    public class Booking
    {
        public int Id { get; set; }
        public string UserId { get; set; }  
        public int FlightId { get; set; }
        public int NumberOfSeats { get; set; }
        public decimal TotalAmount { get; set; }

        // Navigation properties
        public User? User { get; set; }
        public Flight? Flight { get; set; }
        public ICollection<Payment>? Payments { get; set; } = new List<Payment>();
    }
}
