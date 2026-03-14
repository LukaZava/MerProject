using Microsoft.EntityFrameworkCore;

namespace MERPROJ.Models
{
    public class MerDbContext : DbContext
    {
        public MerDbContext(DbContextOptions<MerDbContext> options) : base(options)
        {
           
        }

        public DbSet<Customer> Customers => Set<Customer>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Customer>(entity =>
            {
                entity.HasKey(c => c.Id);

                entity.Property(c => c.FirstName)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(c => c.LastName)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(c => c.Email)
                    .IsRequired()
                    .HasMaxLength(150);

                entity.HasIndex(c => c.Email)
                    .IsUnique();

                entity.Property(c => c.Phone)
                    .HasMaxLength(30);

                entity.Property(c => c.City)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(c => c.Country)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(c => c.IsActive)
                    .IsRequired();

                entity.Property(c => c.CreatedAt)
                    .IsRequired();

                entity.Property(c => c.LastModifiedAt)
                    .IsRequired(false);

                entity.HasIndex(c => c.IsActive);
                entity.HasIndex(c => c.City);
                entity.HasIndex(c => c.Country);
            });
        }
    }
}
