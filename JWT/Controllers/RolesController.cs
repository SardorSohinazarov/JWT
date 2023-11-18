using JWT.Data;
using JWT.DTOs;
using JWT.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JWT.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class RolesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public RolesController(AppDbContext context)
            => _context = context;

        [HttpGet]
        public async Task<IActionResult> GetRolesAsync()
        {
            return Ok(await _context.Roles.AsNoTracking().Include(x => x.Permissions).ToListAsync());
        }

        [HttpPost]
        public async Task<IActionResult> Add(CreateRoleDTO roleDTO)
        {
            var role = new Role();
            role.Name = roleDTO.Name;

            List<Permission> permissions = new();

            foreach (var permission in roleDTO.Permissions)
            {
                var storagePermission = await _context.Permissions
                    .FirstOrDefaultAsync(x => x.Id == permission);

                if (storagePermission == null)
                { /**/ }
                else
                    permissions.Add(storagePermission);
            }

            role.Permissions = permissions;

            var newRole = await _context.Roles.AddAsync(role);
            await _context.SaveChangesAsync();

            return Ok(newRole.Entity);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateAsync(int id, UpdateRoleDTO roleDTO)
        {
            var role = await _context.Roles
                .FirstOrDefaultAsync(x => x.Id == id);

            if (role == null) { /**/ }

            role.Name = roleDTO.Name;

            List<Permission> permissions = new();

            foreach (var permission in roleDTO.Permissions)
            {
                var storagePermission = await _context.Permissions
                    .FirstOrDefaultAsync(x => x.Id == permission);

                if (storagePermission == null)
                { /**/ }
                else
                    permissions.Add(storagePermission);
            }

            role.Permissions = permissions;

            var entityEntry = _context.Update(role);
            await _context.SaveChangesAsync();

            return Ok(entityEntry.Entity);
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            var role = await _context.Roles
                .FirstOrDefaultAsync(x => x.Id == id);

            if (role == null) { /**/ }

            var entityEntry = _context.Remove(role);
            await _context.SaveChangesAsync();

            return Ok(entityEntry.Entity);
        }
    }
}
