namespace SimplyFly_Backend.Models
{
    public class Booking
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int FlightId { get; set; }
        public int SeatCount { get; set; }
        public decimal TotalPrice { get; set; }
        public DateTime BookingDate { get; set; }
        public string Status { get; set; } // e.g., "Confirmed", "Cancelled"
    }
}
