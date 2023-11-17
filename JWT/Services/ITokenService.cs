using JWT.DTOs;
using JWT.Entities;
using System.Linq.Expressions;
using System.Security.Claims;

namespace JWT.Services
{
    public interface ITokenService
    {
        public string GenerateJWT(User user);
        public ValueTask<TokenDTO> RefreshToken(RefreshTokenDTO refreshTokenDTO);
    }
}
