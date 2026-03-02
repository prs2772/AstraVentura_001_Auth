using AstraVenturaAuth.Adapters.Drivens.Database.Models;
using Microsoft.EntityFrameworkCore;

namespace AstraVenturaAuth.Adapters.Drivens.Database;

public sealed class AuthDbContext : DbContext
{
    public AuthDbContext(DbContextOptions<AuthDbContext> options) : base(options) { }

    public DbSet<UserEntity> Users => Set<UserEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserEntity>(entity =>
        {
            entity.HasKey(u => u.Id);
            entity.HasIndex(u => u.Email).IsUnique();
            entity.Property(u => u.Email).HasMaxLength(320).IsRequired();
            entity.Property(u => u.FirstName).HasMaxLength(100).IsRequired();
            entity.Property(u => u.MiddleName).HasMaxLength(100);
            entity.Property(u => u.LastName).HasMaxLength(100).IsRequired();
            entity.Property(u => u.SecondLastName).HasMaxLength(100);
            entity.Property(u => u.PasswordHash).IsRequired();
        });
    }
}
