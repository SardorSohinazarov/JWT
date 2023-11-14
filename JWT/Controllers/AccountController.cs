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
        private readonly JWTTokenService _jwtTokenService;

        public AccountController(
            AppDbContext context, 
            JWTTokenService jwtTokenService)
        {
            _context = context;
            _jwtTokenService = jwtTokenService;
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginDTO loginDTO)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(user => user.Password == loginDTO.Password && user.Username == loginDTO.Username);

            if (user == null)
            {
                return NotFound("Chiqmadi");
            }

            return Ok(_jwtTokenService.GenerateJWT(user));
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterDTO registerDTO)
        {
            var user = new User
            {
                Fullname = registerDTO.Fullname,
                Email = registerDTO.Email,
                Username = registerDTO.Username,
                Password = registerDTO.Password,
                Role = registerDTO.Role,
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
