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
        public DbSet<Trek> Treks { get; set; }

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

            // Configure Trek entity
            modelBuilder.Entity<Trek>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.SerialNumber).IsRequired();
                entity.Property(e => e.State).HasMaxLength(100);
                entity.Property(e => e.TrekName).HasMaxLength(200);
                entity.Property(e => e.TrekType).HasMaxLength(100);
                entity.Property(e => e.DifficultyLevel).HasMaxLength(50);
                entity.Property(e => e.Season).HasMaxLength(100);
                entity.Property(e => e.Duration).HasMaxLength(100);
                entity.Property(e => e.Distance).HasMaxLength(100);
                entity.Property(e => e.MaxAltitude).HasMaxLength(100);
                entity.Property(e => e.TrekDescription).HasColumnType("NVARCHAR(MAX)");
                entity.Property(e => e.Image).HasMaxLength(500);
            });
        }
    }
}
