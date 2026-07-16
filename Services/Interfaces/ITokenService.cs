using IMS.Models;

namespace IMS.Services.Interfaces
{
    public interface ITokenService
    {
        string GenerateToken(User user);
    }
}
