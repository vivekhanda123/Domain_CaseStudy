using Microsoft.EntityFrameworkCore;
using SimplyFly_Backend.Data;
using SimplyFly_Backend.DTOs;
using SimplyFly_Backend.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SimplyFly_Backend.Repositories
{
    public class BookingService : IBookingService
    {
        private readonly MyDbContext _context;

        public BookingService(MyDbContext context)
        {
            _context = context;
        }

        // Private method to get flight by ID and handle not found case
        private async Task<Flight> GetFlightByIdAsync(int flightId)
        {
            var flight = await _context.Flights.FindAsync(flightId);
            if (flight == null)
            {
                throw new KeyNotFoundException($"Flight with ID {flightId} not found.");
            }
            return flight;
        }

        // Book flight and handle validation
        public async Task<Booking> BookFlightAsync(BookingDto bookingDto)
        {
            var flight = await GetFlightByIdAsync(bookingDto.FlightId);

            // Check if enough seats are available
            if (flight.AvailableSeats < bookingDto.NumberOfSeats)
            {
                throw new InvalidOperationException("Not enough available seats.");
            }

            // Calculate the total amount (optional logic can be added here)
            var totalAmount = flight.Fare * bookingDto.NumberOfSeats;

            var booking = new Booking
            {
                UserId = bookingDto.UserId,
                FlightId = bookingDto.FlightId,
                NumberOfSeats = bookingDto.NumberOfSeats,
                TotalAmount = totalAmount
            };

            // Start a transaction to ensure atomicity
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    // Update available seats and save the booking
                    flight.AvailableSeats -= bookingDto.NumberOfSeats;
                    _context.Bookings.Add(booking);
                    _context.Flights.Update(flight);

                    // Commit transaction
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    return booking;
                }
                catch (Exception)
                {
                    // Rollback transaction in case of an error
                    await transaction.RollbackAsync();
                    throw;
                }
            }
        }

        // Get all bookings for a specific user
        public async Task<IEnumerable<Booking>> GetBookingsByUserIdAsync(string userId)
        {
            return await _context.Bookings
                .Where(b => b.UserId == userId)
                .Include(b => b.Flight)
                .ToListAsync();
        }


        // Get booking by ID
        public async Task<Booking> GetBookingByIdAsync(int bookingId)
        {
            return await _context.Bookings
                .Include(b => b.Flight)
                .FirstOrDefaultAsync(b => b.Id == bookingId);
        }

        // Cancel booking and handle flight seat restoration
        public async Task<bool> CancelBookingAsync(int bookingId)
        {
            var booking = await _context.Bookings.FindAsync(bookingId);
            if (booking == null)
            {
                return false; // Booking not found
            }

            var flight = await _context.Flights.FindAsync(booking.FlightId);
            if (flight != null)
            {
                flight.AvailableSeats += booking.NumberOfSeats;
                _context.Flights.Update(flight);
            }

            // Remove booking and commit transaction
            _context.Bookings.Remove(booking);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
