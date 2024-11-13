using System;

namespace SimplyFly_Backend.DTOs
{
    public class FlightDto
    {
        public string Origin { get; set; }
        public string Destination { get; set; }
        public DateTime DepartureTime { get; set; }
        public DateTime ArrivalTime { get; set; }
        public decimal Fare { get; set; }           // Price per seat
        public int AvailableSeats { get; set; }     // Total available seats for the flight
        public string FlightNumber { get; set; }    // Unique flight number identifier
        public string BaggageInfo { get; set; }
        public string FlightName { get; set; }
    }
}
