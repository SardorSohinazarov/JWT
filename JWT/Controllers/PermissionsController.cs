using JWT.Data;
using JWT.DTOs;
using JWT.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JWT.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class PermissionsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public PermissionsController(AppDbContext context)
            => _context = context;

        [HttpGet]
        public async Task<IActionResult> GetPermissionsAsync()
        {
            return Ok(await _context.Permissions.AsNoTracking().ToListAsync());
        }

        [HttpPost]
        public async Task<IActionResult> Add(CreatePermissionDTO permissionDTO)
        {
            var permission = new Permission();
            permission.Name = permissionDTO.Name;

            var newPermission = await _context.Permissions.AddAsync(permission);
            await _context.SaveChangesAsync();

            return Ok(newPermission.Entity);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateAsync(int id, UpdatePermissionDTO PermissionDTO)
        {
            var permission = await _context.Permissions
                .FirstOrDefaultAsync(x => x.Id == id);

            if (permission == null) { /**/ }

            permission.Name = PermissionDTO.Name;

            var entityEntry = _context.Update(permission);
            await _context.SaveChangesAsync();

            return Ok(entityEntry.Entity);
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            var permission = await _context.Permissions
                .FirstOrDefaultAsync(x => x.Id == id);

            if (permission == null) { /**/ }

            var entityEntry = _context.Remove(permission);
            await _context.SaveChangesAsync();

            return Ok(entityEntry.Entity);
        }
    }
}
