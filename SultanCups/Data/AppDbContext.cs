using Microsoft.EntityFrameworkCore;
using SultanCups.Models;

namespace SultanCups.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Employee> employees { get; set; } = null!;
        public DbSet<Admain> admins { get; set; } = null!;
        public DbSet<Salary> salaries { get; set; } = null!;
        public DbSet<CashBox> cash_boxes { get; set; } = null!;
        public DbSet<Product> products { get; set; } = null!;
        public DbSet<Production> production { get; set; } = null!;
        public DbSet<Supplier> suppliers { get; set; } = null!;
    }
}