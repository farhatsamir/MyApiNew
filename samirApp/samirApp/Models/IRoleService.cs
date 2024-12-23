using samirApp.Data;

namespace samirApp.Models
{
    public interface IRoleService
    {
        Task<bool> UserHasRole(string email, string role);
    }

    public class RoleService : IRoleService
    {
        private readonly ApplicationDbContext _context;

        public RoleService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> UserHasRole(string email, string role)
        {
            return await _context.Roles!.AnyAsync(r => r.Email == email && r.Role == role);
        }
    }

}
