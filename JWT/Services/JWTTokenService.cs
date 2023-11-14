using JWT.Entities;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace JWT.Services
{
    public class JWTTokenService
    {
        private IConfiguration _configuration;

        public JWTTokenService(IConfiguration configuration)
            => _configuration = configuration;

        public string GenerateJWT(IEnumerable<Claim> additionalClaims = null)
        {
            var securityKey = GetSecurityKey(_configuration);
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var expireInMinutes = Convert.ToInt32(_configuration["Jwt:ExpireInMinutes"] ?? "60");

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

            if (additionalClaims?.Any() == true)
                claims.AddRange(additionalClaims);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: "*",
                claims: claims,
                expires: DateTime.Now.AddMinutes(expireInMinutes),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string GenerateJWT(User user, IEnumerable<Claim> additionalClaims = null)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Fullname.ToString()),
                new Claim("UserName", user.Username.ToString()),
                new Claim("Password", user.Password.ToString()),
                new Claim(ClaimTypes.Role, user.Role.ToString()),
            };

            if (additionalClaims?.Any() == true)
                claims.AddRange(additionalClaims);

            return GenerateJWT(claims);
        }

        private static SymmetricSecurityKey GetSecurityKey(IConfiguration configuration)
            => new(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]));
    }
}
