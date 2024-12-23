using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using samirApp.Data;
using samirApp.Models;
using samirApp.Models.Entities;
using System.Threading.Tasks;

namespace samirApp.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class RolesController : ControllerBase
    {
        private readonly ApplicationDbContext dbContext;
        private readonly IRoleService _roleService;
        public RolesController(ApplicationDbContext dbContext, IRoleService _roleService)
        {
            this.dbContext = dbContext;
            this._roleService = _roleService;
        }

        [HttpPost("assign-role")]
        public async Task<IActionResult> AssignRole(AddRole addRole)
        {
            var email = User.Identity?.Name;
            if (email == null || !await _roleService.UserHasRole(email, "Admin"))
            {
                return Unauthorized("You do not have access to add roles.");
            }

            var add = new RoleModel()
            {
                Email = addRole.Email,
                Role = addRole.Role
            };

            dbContext.Roles!.Add(add);
            dbContext.SaveChanges();
            return Ok(add);
        }


        [HttpDelete("revok role")]
        public async Task<IActionResult> RevokRole(Guid id)
        {
            var email = User.Identity?.Name; 
            if (email == null || !await _roleService.UserHasRole(email, "Admin"))
            {
                return Unauthorized("You do not have access to delete role.");
            }
            var delete = await dbContext.Roles.FindAsync(id);
            if (delete == null)
            {
                return NotFound("Roles not found.");
            }
            dbContext.Roles.Remove(delete);
            await dbContext.SaveChangesAsync();
            return Ok(delete);

        }
    }

   
}
