using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SimplyFly_Backend.Data;
using SimplyFly_Backend.DTOs;
using SimplyFly_Backend.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SimplyFly_Backend.Repositories
{
    public class UserService : IUserService
    {
        private readonly MyDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly IConfiguration _configuration;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserService(MyDbContext context, UserManager<User> userManager, IConfiguration configuration, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _configuration = configuration;
            _roleManager = roleManager;
        }

        // Register a new user
        public async Task<IdentityResult> RegisterUserAsync(UserRegistrationDto registrationDto)
        {
            var user = new User
            {
                Name = registrationDto.Name,
                Gender = registrationDto.Gender,
                ContactNumber = registrationDto.ContactNumber,
                Address = registrationDto.Address,
                Email = registrationDto.Email,
                UserName = registrationDto.Email,
                Role = registrationDto.Role ?? "User"
            };

            var result = await _userManager.CreateAsync(user, registrationDto.Password);

            if (result.Succeeded)
            {
                if (!await _roleManager.RoleExistsAsync(user.Role))
                {
                    var roleResult = await _roleManager.CreateAsync(new IdentityRole(user.Role));
                    if (!roleResult.Succeeded) return IdentityResult.Failed(roleResult.Errors.ToArray());
                }

                await _userManager.AddToRoleAsync(user, user.Role);
            }

            return result;
        }

        // Authenticate user and generate JWT token
        public async Task<string> AuthenticateUserAsync(LoginDto loginDto)
        {
            var user = await _userManager.FindByEmailAsync(loginDto.Email);
            if (user == null || !await _userManager.CheckPasswordAsync(user, loginDto.Password))
            {
                throw new UnauthorizedAccessException("Invalid email or password.");
            }

            return GenerateJwtToken(user);
        }

        // Generate JWT token
        private string GenerateJwtToken(User user)
        {
            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName),
                //new Claim(ClaimTypes.Email, user.Email),
                //new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                //new Claim(ClaimTypes.Role, user.Role),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:SecretKey"]));

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                expires: DateTime.Now.AddDays(1),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        // Get user by ID
        public async Task<User> GetUserByIdAsync(string userId)
        {
            return await _userManager.FindByIdAsync(userId) ?? throw new KeyNotFoundException("User not found");
        }

        // Retrieve all users
        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return await _userManager.Users.ToListAsync();
        }

        // Delete a user by ID
        public async Task<bool> DeleteUserAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return false;

            var result = await _userManager.DeleteAsync(user);
            return result.Succeeded;
        }
    }
}
