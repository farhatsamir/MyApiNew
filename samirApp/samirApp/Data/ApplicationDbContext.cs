

using samirApp.Models;
using samirApp.Models.Entities;

namespace samirApp.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<IdentityUser>(options)
    {
        public DbSet<Category>? Categories { get; set; }
        public DbSet<Product>? Products { get; set; }
        public DbSet<RoleModel>? Roles {  get; set; }

    }



}
