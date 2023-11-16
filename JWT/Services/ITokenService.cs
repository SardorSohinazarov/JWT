using JWT.Entities;
using System.Security.Claims;

namespace JWT.Services
{
    public interface ITokenService
    {
        public string GenerateJWT(IEnumerable<Claim> additionalClaims = null);
        public string GenerateJWT(User user);
    }
}
