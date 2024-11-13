using Microsoft.EntityFrameworkCore;
using SimplyFly_Backend.Data;
using SimplyFly_Backend.DTOs;
using SimplyFly_Backend.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace SimplyFly_Backend.Repositories
{
    public class FlightService : IFlightService
    {
        private readonly MyDbContext _context;

        public FlightService(MyDbContext context)
        {
            _context = context;
        }

        // Search for flights based on origin, destination, and date of journey
        public async Task<IEnumerable<Flight>> SearchFlightsAsync(FlightSearchDto searchDto)
        {
            var flights = await _context.Flights
                .Where(f => f.Origin == searchDto.Origin
                            && f.Destination == searchDto.Destination
                            && f.DepartureTime.Date == searchDto.DateOfJourney.Date)
                .ToListAsync();

            return flights;
        }

        // Get a flight by its ID
        public async Task<Flight> GetFlightByIdAsync(int flightId)
        {
            return await _context.Flights.FirstOrDefaultAsync(f => f.Id == flightId);
        }

        // Book a flight for a user
        public async Task<Booking> BookFlightAsync(BookingDto bookingDto)
        {
            var flight = await GetFlightByIdAsync(bookingDto.FlightId);
            if (flight == null)
            {
                throw new KeyNotFoundException("Flight not found.");
            }

            // Check if enough seats are available
            if (flight.AvailableSeats < bookingDto.NumberOfSeats)
            {
                throw new InvalidOperationException("Not enough available seats.");
            }

            // Calculate the total amount
            var totalAmount = flight.Fare * bookingDto.NumberOfSeats;

            // Create a new booking
            var booking = new Booking
            {
                UserId = bookingDto.UserId,
                FlightId = bookingDto.FlightId,
                NumberOfSeats = bookingDto.NumberOfSeats,
                TotalAmount = totalAmount
            };

            // Update available seats
            flight.AvailableSeats -= bookingDto.NumberOfSeats;

            // Add booking to the database
            _context.Bookings.Add(booking);
            _context.Flights.Update(flight);

            await _context.SaveChangesAsync();
            return booking;
        }

        // Cancel an existing booking and restore the available seats
        public async Task<bool> CancelBookingAsync(int bookingId)  // Enter user id to cancel the ticket
        {
            var booking = await _context.Bookings.FindAsync(bookingId);
            if (booking == null)
            {
                return false; // Booking not found
            }

            // Get the related flight
            var flight = await _context.Flights.FindAsync(booking.FlightId);
            if (flight != null)
            {
                // Restore the available seats
                flight.AvailableSeats += booking.NumberOfSeats;
                _context.Flights.Update(flight);
            }

            // Delete the booking
            _context.Bookings.Remove(booking);
            await _context.SaveChangesAsync();
            return true; // Indicate cancellation was successful
        }

        // Add a new flight (Admin-only)
        public async Task<Flight> AddFlightAsync(FlightDto flightDto)
        {
            try
            {
                var flight = new Flight
                {
                    Origin = flightDto.Origin,
                    Destination = flightDto.Destination,
                    DepartureTime = flightDto.DepartureTime,
                    ArrivalTime = flightDto.ArrivalTime,
                    Fare = flightDto.Fare,
                    AvailableSeats = flightDto.AvailableSeats,
                    FlightNumber = flightDto.FlightNumber,
                    FlightName = flightDto.FlightName,
                    BaggageInfo = flightDto.BaggageInfo ?? "Standard baggage allowance" // Set a default value
                    
                };

                _context.Flights.Add(flight);
                await _context.SaveChangesAsync();
                return flight;
            }
            catch (Exception ex)
            {
                // Log the exception to the console or a file
                Console.WriteLine("Error adding flight: " + ex.Message);
                if (ex.InnerException != null)
                {
                    Console.WriteLine("Inner exception: " + ex.InnerException.Message);
                }
                throw; // Re-throw the exception after logging it
            }
        }


        // Update an existing flight's details (Admin-only)
        public async Task<bool> UpdateFlightAsync(int flightId, FlightDto flightDto)
        {
            var flight = await GetFlightByIdAsync(flightId);
            if (flight == null) return false;

            flight.Origin = flightDto.Origin;
            flight.Destination = flightDto.Destination;
            flight.DepartureTime = flightDto.DepartureTime;
            flight.ArrivalTime = flightDto.ArrivalTime;
            flight.Fare = flightDto.Fare;
            flight.AvailableSeats = flightDto.AvailableSeats;
            flight.FlightNumber = flightDto.FlightNumber;

            _context.Flights.Update(flight);
            await _context.SaveChangesAsync();
            return true;
        }

        // Delete a flight (Admin-only)
        public async Task<bool> DeleteFlightAsync(int flightId)
        {
            var flight = await _context.Flights.FindAsync(flightId);
            if (flight == null) return false;

            _context.Flights.Remove(flight);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
