using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace IdentityApp.Models
{
    public class IdentityContext : IdentityDbContext<AppUser, AppRole, string>
    {
        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    optionsBuilder.UseSqlite("Data Source=databse.dat");
        //}

        public IdentityContext(DbContextOptions<IdentityContext> options) : base(options)
        {
            
        }
    }
}
