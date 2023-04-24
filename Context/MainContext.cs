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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Site>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.Description).IsRequired();
            entity.Property(e => e.City).IsRequired();
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.UpdatedAt).IsRequired();

            entity.HasMany(e => e.Employees)
                .WithOne(e => e.Site)
                .HasForeignKey(e => e.SiteId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Service>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.Name).IsRequired();
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.UpdatedAt).IsRequired();

            entity.HasMany(e => e.Employees)
                .WithOne(e => e.Service)
                .HasForeignKey(e => e.ServiceId)
                .OnDelete(DeleteBehavior.Cascade);
        });


        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.FirstName).IsRequired();
            entity.Property(e => e.LastName).IsRequired();
            entity.Property(e => e.Email).IsRequired();
            entity.Property(e => e.LandlinePhone).IsRequired();
            entity.Property(e => e.MobilePhone).IsRequired();
            //set pasword to nullable in database
            entity.Property(e => e.Password).IsRequired(false);
            entity.Property(e => e.IsAdmin).IsRequired();
            entity.Property(e => e.IsAdmin).HasDefaultValue(false);
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.UpdatedAt).IsRequired();
        });
    }

    public override int SaveChanges()
    {
        AddTimestamps();
        return base.SaveChanges();
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new())
    {
        AddTimestamps();
        return base.SaveChangesAsync();
    }

    private void AddTimestamps()
    {
        var entities = ChangeTracker.Entries()
            .Where(x => x.Entity is BaseEntity &&
                        (x.State == EntityState.Added || x.State == EntityState.Modified));

        foreach (var entity in entities)
        {
            var now = DateTime.UtcNow; // current datetime

            if (entity.State == EntityState.Added) ((BaseEntity)entity.Entity).CreatedAt = now;

            ((BaseEntity)entity.Entity).UpdatedAt = now;
        }
    }
}