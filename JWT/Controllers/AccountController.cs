using JWT.Data;
using JWT.DTOs;
using JWT.Entities;
using JWT.Filters;
using JWT.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace JWT.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ITokenService _jwtTokenService;
        private readonly IPasswordHasher _passwordHasher;

        public AccountController(
            AppDbContext context,
            ITokenService jwtTokenService,
            IPasswordHasher passwordHasher)
        {
            _context = context;
            _jwtTokenService = jwtTokenService;
            _passwordHasher = passwordHasher;
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginDTO loginDTO)
        {
            var user = await _context.Users.Include(x => x.Roles)
                .FirstOrDefaultAsync(user => user.Username == loginDTO.Username);

            if (user == null)
                return NotFound("Chiqmadi");

            if (!_passwordHasher.Verify(user.PasswordHash, loginDTO.Password, user.Salt))
                throw new Exception("Username or password is not valid");

            return Ok(
            new TokenDTO()
            {
                AccessToken = _jwtTokenService.GenerateJWT(user),
                RefreshToken = user.RefreshToken,
                ExpireDate = user.RefreshTokenExpireDate ?? DateTime.Now.AddMinutes(2),
            }
            );
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterDTO registerDTO)
        {
            var salt = Guid.NewGuid().ToString();
            var PasswordHash = _passwordHasher.Encrypt(registerDTO.Password, salt);

            var user = new User
            {
                Fullname = registerDTO.Fullname,
                Email = registerDTO.Email,
                Username = registerDTO.Username,
                Salt = salt,
                PasswordHash = PasswordHash,
                RefreshToken = Guid.NewGuid().ToString(),
                RefreshTokenExpireDate = DateTime.Now.AddMinutes(2)
            };

            List<Role> roles = new();

            foreach (var role in registerDTO.Roles)
            {
                var listItem = await _context.Roles.FirstOrDefaultAsync(x => x.Id == role);
                if (listItem == null) { /**/ }
                else
                {
                    roles.Add(listItem);
                }
            }

            user.Roles = roles;

            var entityEntry = await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            return Ok(
                new TokenDTO()
                {
                    AccessToken = _jwtTokenService.GenerateJWT(entityEntry.Entity),
                    RefreshToken = user.RefreshToken,
                    ExpireDate = user.RefreshTokenExpireDate ?? DateTime.Now.AddMinutes(2),
                }
               );
        }

        [HttpPost]
        public async Task<IActionResult> RefreshTokenAsync(RefreshTokenDTO refreshTokenDTO)
        {
            var token = await _jwtTokenService.RefreshToken(refreshTokenDTO);

            return Ok(token);
        }

        [PermissionFilter(permission: "CreateUser")]
        [LoggerFilter]
        [HttpPost]
        public async Task<IActionResult> TestAction([FromBody] string name)
        {
            var role = User.FindFirstValue(ClaimTypes.Role);

            return Ok(
                new
                {
                    CurrentUserRole = role,
                    Users = await _context.Users.ToListAsync()
                });
        }
    }
}
