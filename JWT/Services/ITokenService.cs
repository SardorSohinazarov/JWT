using JWT.DTOs;
using JWT.Entities;

namespace JWT.Services
{
    public interface ITokenService
    {
        public string GenerateJWT(User user);
        public ValueTask<TokenDTO> RefreshToken(RefreshTokenDTO refreshTokenDTO);
    }
}
