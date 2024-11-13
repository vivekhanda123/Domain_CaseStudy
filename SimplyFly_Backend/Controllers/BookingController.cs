using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SimplyFly_Backend.DTOs;
using SimplyFly_Backend.Repositories;

namespace SimplyFly_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingController : ControllerBase
    {
        private readonly IBookingService _bookingService;

        public BookingController(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }

        // Endpoint to create a new booking
        [HttpPost("book")]
        public async Task<IActionResult> BookFlight([FromBody] BookingDto bookingDto)
        {
            // Ensure that the model is valid before proceeding
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); // Return 400 if the input is invalid
            }

            try
            {
                var booking = await _bookingService.BookFlightAsync(bookingDto);
                // Return 201 Created and location of the new booking
                return CreatedAtAction(nameof(GetBookingById), new { bookingId = booking.Id }, booking);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message }); // Return error message as JSON
            }
        }

        // Endpoint to get all bookings for a user
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetBookingsByUserId(string userId)
        {
            var bookings = await _bookingService.GetBookingsByUserIdAsync(userId);
            if (bookings == null || !bookings.Any()) // Check if bookings list is empty
            {
                return NotFound(new { message = "No bookings found for this user." });
            }
            return Ok(bookings); // Return the user's bookings
        }

        // Endpoint to get details of a specific booking by its ID
        [HttpGet("{bookingId}")]
        public async Task<IActionResult> GetBookingById(int bookingId)
        {
            var booking = await _bookingService.GetBookingByIdAsync(bookingId);
            if (booking == null)
            {
                return NotFound(new { message = "Booking not found." });
            }
            return Ok(booking); // Return the booking details
        }

        // Endpoint to cancel a booking
        [HttpDelete("{bookingId}")]
        public async Task<IActionResult> CancelBooking(int bookingId)
        {
            var result = await _bookingService.CancelBookingAsync(bookingId);
            if (!result)
            {
                return NotFound(new { message = "Booking not found for cancellation." });
            }
            return NoContent(); // Return 204 No Content to indicate successful cancellation
        }
    }
}
