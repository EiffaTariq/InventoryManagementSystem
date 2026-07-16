using System.ComponentModel.DataAnnotations;

namespace IMS.Models.DTOs.Auth
{
    public class RegisterDto
    {

        [Required, StringLength(50)]
        public string FullName { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        [Required, MinLength(6)]
        public string Password { get; set; }

        [Required]
        public string Role { get; set; }
    }
}
