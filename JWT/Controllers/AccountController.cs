using JWT.Data;
using JWT.DTOs;
using JWT.Entities;
using JWT.Services;
using Microsoft.AspNetCore.Authorization;
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
            var user = await _context.Users
                .FirstOrDefaultAsync(user => user.Username == loginDTO.Username);

            if (user == null)
                return NotFound("Chiqmadi");

            if (!_passwordHasher.Verify(user.PasswordHash, loginDTO.Password, user.Salt))
                throw new Exception("Username or password is not valid");

            return Ok(_jwtTokenService.GenerateJWT(user));
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
                Role = registerDTO.Role,
                Salt = salt,
                PasswordHash = PasswordHash
            };


            var entityEntry = await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            return Ok(_jwtTokenService.GenerateJWT(entityEntry.Entity));
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> GetUser()
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
