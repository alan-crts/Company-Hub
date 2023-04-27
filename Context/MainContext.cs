using CompanyHub.Models;
using Microsoft.EntityFrameworkCore;

namespace CompanyHub.Context;

public class MainContext : DbContext
{
    public MainContext(DbContextOptions<MainContext> options) : base(options)
    {
    }

    public DbSet<Employee> Employee { get; set; }
    public DbSet<Service> Service { get; set; }
    public DbSet<Site> Site { get; set; }

    public override int SaveChanges()
    {
        AddTimestamps();
        return base.SaveChanges();
    }

    public async Task<int> SaveChangesAsync()
    {
        AddTimestamps();
        return await base.SaveChangesAsync();
    }

    private void AddTimestamps()
    {
        var entities = ChangeTracker.Entries()
            .Where(x => x.Entity is BaseEntity && (x.State == EntityState.Added || x.State == EntityState.Modified));

        foreach (var entity in entities)
        {
            var now = DateTime.UtcNow; // current datetime

            if (entity.State == EntityState.Added) ((BaseEntity)entity.Entity).CreatedAt = now;

            ((BaseEntity)entity.Entity).UpdatedAt = now;
        }
    }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Employee>()
            .HasIndex(u => u.Email)
            .IsUnique();
    }
}