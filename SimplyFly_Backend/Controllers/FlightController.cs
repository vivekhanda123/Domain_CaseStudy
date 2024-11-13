using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SimplyFly_Backend.DTOs;
using SimplyFly_Backend.Models;
using SimplyFly_Backend.Repositories;
using System.Threading.Tasks;
using System;

namespace SimplyFly_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FlightController : ControllerBase
    {
        private readonly IFlightService _flightService;

        public FlightController(IFlightService flightService)
        {
            _flightService = flightService;
        }

        // Endpoint to search for flights based on criteria
        [HttpPost("search")]
        public async Task<IActionResult> SearchFlights([FromBody] FlightSearchDto searchDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); // Return detailed validation errors
            }

            var flights = await _flightService.SearchFlightsAsync(searchDto);
            if (flights == null || !flights.Any())
            {
                return NotFound("No flights found matching the search criteria.");
            }

            return Ok(flights);
        }

        // Endpoint to add a new flight (Admin Only)
       
        [HttpPost("add")]
        [Authorize(Roles = "FlightOwner")]
        public async Task<IActionResult> AddFlight([FromBody] FlightDto flightDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); // Return detailed validation errors
            }

            try
            {
                var addedFlight = await _flightService.AddFlightAsync(flightDto);
                return CreatedAtAction(nameof(GetFlightById), new { flightId = addedFlight.Id }, addedFlight);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error adding flight: {ex.Message}");
            }
        }

        // Endpoint to get flight details by ID
        [HttpGet("{flightId}")]
        public async Task<IActionResult> GetFlightById(int flightId)
        {
            var flight = await _flightService.GetFlightByIdAsync(flightId);
            if (flight == null)
            {
                return NotFound("Flight not found.");
            }

            return Ok(flight);
        }

        // Endpoint to book a flight
        [HttpPost("book")]
        public async Task<IActionResult> BookFlight([FromBody] BookingDto bookingDto)
        {
            if (bookingDto == null)
            {
                return BadRequest("Booking data is required.");
            }

            try
            {
                var booking = await _flightService.BookFlightAsync(bookingDto);
                return CreatedAtAction(nameof(BookFlight), new { bookingId = booking.Id }, booking);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [Authorize(Roles = "FlightOwner")]
        // Endpoint to cancel a booking
        [HttpDelete("cancel/{bookingId}")]
        public async Task<IActionResult> CancelBooking(int bookingId)
        {
            var result = await _flightService.CancelBookingAsync(bookingId);
            if (!result)
            {
                return NotFound("Booking not found.");
            }
            return NoContent(); // Successfully canceled the booking
        }
    }
}
