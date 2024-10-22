using Microsoft.EntityFrameworkCore;
using Project_1.Core.Entities;

namespace Project_1.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Product> Products { get; set; }

        public DbSet<Category> Categories { get; set; }

        public DbSet<Payment> Payments { get; set; }

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }

        public DbSet<Customer> Customers { get; set; }

        public DbSet<Order> Orders { get; set; }

        public DbSet<OrderItems> orderitem { get; set; }

        public DbSet<Wishlist> Wishlist { get; set; }

        public DbSet<Tag> Tags { get; set; }

        public DbSet<Discount> Discounts { get; set; }   

        public DbSet<Cart> Carts { get; set; }

        public DbSet<CartItems> CartItems { get; set; }

        public DbSet<Employee> Employees { get; set; }
        public DbSet<Attendance> Attendance { get; set; }
        public DbSet<Payroll> Payrolls { get; set; }




        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Product>()
              .HasOne(p => p.Tag)
              .WithMany(c => c.Product)
              .HasForeignKey(p => p.ID_tag);


            modelBuilder.Entity<Product>()
              .HasOne(p => p.Category)
              .WithMany(c => c.Product)
              .HasForeignKey(p => p.ID_danhmuc);

            // Thiết lập quan hệ 1-nhiều giữa Category và Product
            modelBuilder.Entity<Category>()
                .HasMany(c => c.Product)  // Một Category có nhiều Product
                .WithOne(p => p.Category)  // Một Product thuộc một Category
                .HasForeignKey(p => p.ID_danhmuc);  // Thiết lập khóa ngoại

            modelBuilder.Entity<UserRole>()
             .HasKey(ur => new { ur.UserId, ur.RoleId });

            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.User)
                .WithMany(u => u.UserRoles)
                .HasForeignKey(ur => ur.UserId);

            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.Role)
                .WithMany(r => r.UserRoles)
                .HasForeignKey(ur => ur.RoleId);

            // Thiết lập khóa chính và mối quan hệ giữa Role và Permission qua RolePermission
            modelBuilder.Entity<RolePermission>()
                .HasKey(rp => new { rp.RoleId, rp.PermissionId });

            modelBuilder.Entity<RolePermission>()
                .HasOne(rp => rp.Role)
                .WithMany(r => r.RolePermissions)
                .HasForeignKey(rp => rp.RoleId);

            modelBuilder.Entity<RolePermission>()
                .HasOne(rp => rp.Permission)
                .WithMany(p => p.RolePermissions)
                .HasForeignKey(rp => rp.PermissionId);

                modelBuilder.Entity<Cart>()
                .HasMany(c => c.CartItems)
                .WithOne(ci => ci.Cart)
                .HasForeignKey(ci => ci.CartId)
                .OnDelete(DeleteBehavior.Cascade);



            modelBuilder.Entity<Employee>().HasKey(e => e.ID_Employee);
            modelBuilder.Entity<Attendance>().HasKey(a => a.ID_Employee);
            modelBuilder.Entity<Payroll>().HasKey(p => p.ID_Employee);

            modelBuilder.Entity<Employee>()
                .HasMany(e => e.Attendances)
                .WithOne(a => a.Employee)
                .HasForeignKey(a => a.ID_Employee);

            modelBuilder.Entity<Employee>()
                .HasMany(e => e.Payrolls)
                .WithOne(p => p.Employee)
                .HasForeignKey(p => p.ID_Employee);

        }
    }
}
