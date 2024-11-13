using SimplyFly_Backend.DTOs;

namespace SimplyFly_Backend.Models
{
    public class Flight
    {
        public int Id { get; set; }
        public string FlightNumber { get; set; }
        public string FlightName { get; set; }
        public string Origin { get; set; }
        public string Destination { get; set; }
        public DateTime DepartureTime { get; set; }
        public DateTime ArrivalTime { get; set; }
        public decimal Fare { get; set; }
        public int AvailableSeats { get; set; }
        public string BaggageInfo { get; set; }  // Baggage info per passenger

        // Navigation properties
        public ICollection<Booking>? Bookings { get; set; } = new List<Booking>();
    }
}
