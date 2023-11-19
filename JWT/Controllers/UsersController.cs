using JWT.Data;
using JWT.DTOs;
using JWT.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JWT.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UsersController(AppDbContext context)
            => _context = context;

        [HttpGet]
        public async Task<IActionResult> GetUsersAsync()
        {
            return Ok(await _context.Users.AsNoTracking().Include(x => x.Roles).ToListAsync());
        }

        [HttpPut]
        public async Task<IActionResult> UpdateAsync(int id, UpdateUserDTO userDTO)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(x => x.Id == id);

            if (user == null) { /**/ }

            user.Fullname = userDTO.Fullname;
            user.Username = userDTO.Username;
            user.Email = userDTO.Email;

            List<Role> roles = new();

            foreach (var role in userDTO.Roles)
            {
                var listItem = await _context.Roles.FirstOrDefaultAsync(x => x.Id == role);
                if (listItem == null) { /**/ }
                else
                {
                    roles.Add(listItem);
                }
            }

            user.Roles = roles;

            var entityEntry = _context.Update(user);
            await _context.SaveChangesAsync();

            return Ok(entityEntry.Entity);
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(x => x.Id == id);

            if (user == null) { /**/ }

            var entityEntry = _context.Remove(user);
            await _context.SaveChangesAsync();

            return Ok(entityEntry.Entity);
        }
    }
}
