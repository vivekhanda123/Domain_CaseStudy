using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SimplyFly_Backend.DTOs;
using SimplyFly_Backend.Repositories;
using System.Net;

namespace SimplyFly_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        // Register a new user
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserRegistrationDto registrationDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); // Return validation errors if the model state is invalid
            }

            try
            {
                var result = await _userService.RegisterUserAsync(registrationDto);
                if (result.Succeeded)
                {
                    return Ok(new { status = "Success", message = "User registered successfully." });
                }

                // Collect error messages from IdentityResult and return them in the response
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return BadRequest(new { status = "Error", message = "User registration failed.", errors });
            }
            catch (InvalidOperationException ex)
            {
                // Handle registration-specific exceptions
                return StatusCode((int)HttpStatusCode.BadRequest, new { status = "Error", message = ex.Message });
            }
            catch (Exception ex)
            {
                // General exception handler for unexpected errors
                return StatusCode(StatusCodes.Status500InternalServerError, new { status = "Error", message = "An error occurred while registering the user.", details = ex.Message });
            }
        }

        // Authenticate user and return a JWT token
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); // Return validation errors if the model state is invalid
            }

            try
            {
                var token = await _userService.AuthenticateUserAsync(loginDto);
                return Ok(new { status = "Success", token });
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized(new { status = "Error", message = "Invalid login credentials." });
            }
            catch (Exception ex)
            {
                // General exception handler for unexpected errors
                return StatusCode(StatusCodes.Status500InternalServerError, new { status = "Error", message = "An error occurred during login.", details = ex.Message });
            }
        }
        [HttpGet("{userId}")]
        public async Task<IActionResult> GetUserById(string userId)
        {
            try
            {
                var user = await _userService.GetUserByIdAsync(userId);
                return Ok(user);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { status = "Error", message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { status = "Error", message = "An error occurred while retrieving the user.", details = ex.Message });
            }
        }

        // Retrieve all users
        [HttpGet("all")]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                var users = await _userService.GetAllUsersAsync();
                return Ok(users);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { status = "Error", message = "An error occurred while retrieving all users.", details = ex.Message });
            }
        }

        // Delete user by ID
        [HttpDelete("{userId}")]
        public async Task<IActionResult> DeleteUser(string userId)
        {
            try
            {
                var success = await _userService.DeleteUserAsync(userId);
                if (!success)
                {
                    return NotFound(new { status = "Error", message = "User not found or could not be deleted." });
                }

                return Ok(new { status = "Success", message = "User deleted successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { status = "Error", message = "An error occurred while deleting the user.", details = ex.Message });
            }
        }
    }
}
