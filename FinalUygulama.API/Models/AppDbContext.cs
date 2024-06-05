using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FinalUygulama.API.Models
{
    public class AppDbContext : IdentityDbContext<AppUser, AppRole, string>
    {

        public DbSet<Kiralama> Kiralamalar { get; set; }
        public DbSet<Araba> Arabalar { get; set; }

        public AppDbContext(DbContextOptions options) : base(options)
        {
        }
    }
}
