using Microsoft.AspNetCore.Identity;
using SimplyFly_Backend.DTOs;
using SimplyFly_Backend.Models;

namespace SimplyFly_Backend.Repositories
{
    public interface IUserService
    {
        // Registers a new user
        Task<IdentityResult> RegisterUserAsync(UserRegistrationDto registrationDto);

        // Authenticates a user and returns a JWT token
        Task<string> AuthenticateUserAsync(LoginDto loginDto);

        // Retrieves a user by their ID
        Task<User> GetUserByIdAsync(string userId); // Change to string for user ID

        // Retrieves all users
        Task<IEnumerable<User>> GetAllUsersAsync();

        // Deletes a user by their ID
        Task<bool> DeleteUserAsync(string userId); // Change to string for user ID
    }
}
