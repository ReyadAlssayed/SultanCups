using Microsoft.EntityFrameworkCore;
using SultanCups.Models;

namespace SultanCups.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        public DbSet<Employee> employees { get; set; } = null!;
        public DbSet<Admain> admins { get; set; } = null!;
        public DbSet<Salary> salaries { get; set; } = null!;
        public DbSet<CashBox> cash_boxes { get; set; } = null!;
        public DbSet<Product> products { get; set; } = null!;
        public DbSet<Production> production { get; set; } = null!;
        public DbSet<Supplier> suppliers { get; set; } = null!;
        public DbSet<RawMaterial> raw_materials { get; set; } = null!;
        public DbSet<Marketer> marketers { get; set; } = null!;
        public DbSet<Order> orders { get; set; } = null!;
        public DbSet<Purchase> purchases { get; set; } = null!;
        public DbSet<Customer> customers { get; set; } = null!;
        public DbSet<OtherPurchase> other_purchases { get; set; } = null!;




        // 🔥 الناقص
        public DbSet<FinancialEvent> financial_events { get; set; } = null!;
        public DbSet<AuditLog> audit_log { get; set; } = null!;
        public DbSet<CashBoxBalance> cash_box_balances { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 🔥 ربط الـ View
            modelBuilder.Entity<CashBoxBalance>()
                .HasNoKey()
                .ToView("cash_box_balances");
        }
    }
}