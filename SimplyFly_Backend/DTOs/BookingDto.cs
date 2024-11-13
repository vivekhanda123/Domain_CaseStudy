using System.ComponentModel.DataAnnotations;

namespace SimplyFly_Backend.DTOs
{
    public class BookingDto
    {
        [Required]
        public string UserId { get; set; } 

        [Required]
        public int FlightId { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Number of seats must be at least 1.")]
        public int NumberOfSeats { get; set; }
    }
}
