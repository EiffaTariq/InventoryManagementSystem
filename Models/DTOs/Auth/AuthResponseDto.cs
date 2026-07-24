using IMS.Enums;

namespace IMS.Models.DTOs.Auth
{
    public class AuthResponseDto
    {
        public string Token { get; set; }
        public string Email { get; set; }
        public UserRole Role { get; set; }
    }
}
