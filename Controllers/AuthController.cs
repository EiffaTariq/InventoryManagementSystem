using IMS.Models.DTOs.Auth;
using IMS.Models;
using IMS.Repositories.Interfaces;
using IMS.Services.Interfaces;
using IMS.Services;
using Microsoft.AspNetCore.Mvc;

namespace IMS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IRepository<User> _userRepository;
        private readonly ITokenService _tokenService;

        public AuthController(IRepository<User> userRepository, ITokenService tokenService)
        {
            _userRepository = userRepository;
            _tokenService = tokenService;
        }

        [HttpPost("register")]
        public async Task<ActionResult<AuthResponseDto>> Register(RegisterDto dto)
        {
            var existing = await _userRepository.FindAsync(u => u.Email == dto.Email);
            if (existing != null)
                return BadRequest("Email already registered.");

            var user = new User
            {
                FullName = dto.FullName,
                Email = dto.Email,
                PasswordHash = PasswordHasher.Hash(dto.Password),
                Role = dto.Role
            };

            await _userRepository.AddAsync(user);
            await _userRepository.SaveChangesAsync();

            var token = _tokenService.GenerateToken(user);

            return Ok(new AuthResponseDto
            {
                Token = token,
                Email = user.Email,
                Role = user.Role
            });
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthResponseDto>> Login(LoginDto dto)
        {
            var user = await _userRepository.FindAsync(u => u.Email == dto.Email);
            if (user == null || !PasswordHasher.Verify(dto.Password, user.PasswordHash))
                return Unauthorized("Invalid email or password.");

            var token = _tokenService.GenerateToken(user);

            return Ok(new AuthResponseDto
            {
                Token = token,
                Email = user.Email,
                Role = user.Role
            });
        }
    }
}
