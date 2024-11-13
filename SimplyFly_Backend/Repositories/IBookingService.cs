using SimplyFly_Backend.DTOs;
using SimplyFly_Backend.Models;

namespace SimplyFly_Backend.Repositories
{
    public interface IBookingService
    {
        Task<Booking> BookFlightAsync(BookingDto bookingDto);
        Task<IEnumerable<Booking>> GetBookingsByUserIdAsync(string userId);
        Task<Booking> GetBookingByIdAsync(int bookingId);
        Task<bool> CancelBookingAsync(int bookingId);
    }
}
