using Microsoft.EntityFrameworkCore;
using TrekifyBackend.Models;

namespace TrekifyBackend.Data
{
    public class TrekifyDbContext : DbContext
    {
        public TrekifyDbContext(DbContextOptions<TrekifyDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        // ✅ REMOVED: Trek table - we'll read directly from Excel

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure User entity
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(255);
                entity.HasIndex(e => e.Email).IsUnique();
                entity.Property(e => e.Password).IsRequired().HasMaxLength(255);
                entity.Property(e => e.Avatar).HasMaxLength(500);
                entity.Property(e => e.Date).HasDefaultValueSql("GETUTCDATE()");
            });

            // ✅ REMOVED: Trek entity configuration - no longer using database table
        }
    }
}
