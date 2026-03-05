using Microsoft.EntityFrameworkCore;
using SultanCups.Models;

namespace SultanCups.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Admain> admins { get; set; }
    }
}