using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Online_Car_Marketplace.Models;

namespace Online_Car_Marketplace.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // 👇 These represent tables in your DB
        public DbSet<Car> Cars { get; set; }


        public DbSet<User> Users { get; set; }

        public DbSet<Wishlist> Wishlist { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Wishlist>()
                .HasOne(w => w.User)
                .WithMany()
                .HasForeignKey(w => w.UserId);

            modelBuilder.Entity<Wishlist>()
                .HasOne(w => w.Car)
                .WithMany()
                .HasForeignKey(w => w.CarId);

            modelBuilder.Entity<Wishlist>()
                .HasIndex(w => new { w.UserId, w.CarId })
                .IsUnique();
        }
    }

    // Add more later, e.g.:
    // public DbSet<Booking> Bookings { get; set; }
}

