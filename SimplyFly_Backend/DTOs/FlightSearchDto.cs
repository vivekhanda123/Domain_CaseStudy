using System;
using System.ComponentModel.DataAnnotations;

namespace SimplyFly_Backend.DTOs
{
    public class FlightSearchDto
    {
        [Required(ErrorMessage = "Origin is required.")]
        [StringLength(100, ErrorMessage = "Origin must be a maximum of 100 characters.")]
        public string Origin { get; set; }

        [Required(ErrorMessage = "Destination is required.")]
        [StringLength(100, ErrorMessage = "Destination must be a maximum of 100 characters.")]
        public string Destination { get; set; }

        [Required(ErrorMessage = "Date of journey is required.")]
        [DataType(DataType.Date, ErrorMessage = "Invalid date format.")]
        public DateTime DateOfJourney { get; set; }

        // Optional: If you plan to support return flights or more filters, you could add them like this:
        // public DateTime? ReturnDate { get; set; }

        // Optional: If you plan to add the number of passengers in the future
        // public int? PassengerCount { get; set; }
    }
}
