using JWT.Entities;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace JWT.Services
{
    public class TokenService:ITokenService
    {
        private IConfiguration _configuration;

        public TokenService(IConfiguration configuration)
            => _configuration = configuration;

        public string GenerateJWT(IEnumerable<Claim> additionalClaims = null)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Sardor:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var expireInMinutes = Convert.ToInt32(_configuration["Sardor:ExpireInMinutes"] ?? "60");

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, DateTime.Now.ToString()),
                
            };

            if (additionalClaims?.Any() == true)
                claims.AddRange(additionalClaims);

            var token = new JwtSecurityToken(
                issuer: _configuration["Sardor:Issuer"],
                audience: _configuration["Sardor:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(expireInMinutes),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string GenerateJWT(User user)
        {
            var claims = new List<Claim>
            {
                new Claim("UserName", user.Username.ToString()),
                new Claim(ClaimTypes.Role, user.Role.ToString()),
            };

            return GenerateJWT(claims);
        }
    }
}
