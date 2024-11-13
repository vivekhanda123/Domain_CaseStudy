using System.ComponentModel.DataAnnotations;

namespace SimplyFly_Backend.DTOs
{
    public class UserRegistrationDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public string Name { get; set; }

        public string Gender { get; set; }
        public string ContactNumber { get; set; }
        public string Address { get; set; }

        [Required]
        public string Role { get; set; }
    }
}
