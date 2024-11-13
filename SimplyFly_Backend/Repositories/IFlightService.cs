using SimplyFly_Backend.DTOs;
using SimplyFly_Backend.Models;

namespace SimplyFly_Backend.Repositories
{
    public interface IFlightService
    {
        Task<IEnumerable<Flight>> SearchFlightsAsync(FlightSearchDto searchDto);
        Task<Flight> GetFlightByIdAsync(int flightId);
        Task<Booking> BookFlightAsync(BookingDto bookingDto);
        Task<bool> CancelBookingAsync(int bookingId);

        // Admin-specific methods for managing flights
        Task<Flight> AddFlightAsync(FlightDto flightDto);      // Add a new flight
        Task<bool> UpdateFlightAsync(int flightId, FlightDto flightDto); // Update flight details
        Task<bool> DeleteFlightAsync(int flightId);            // Delete a flight
    }
}
