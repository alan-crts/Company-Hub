using CompanyHub.Models;
using Microsoft.EntityFrameworkCore;

namespace CompanyHub.Context;

public class MainContext : DbContext
{
    public MainContext(DbContextOptions<MainContext> options) : base(options)
    {
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();

        modelBuilder.Entity<User>()
            .Property(u=>u.Id)
            .HasDefaultValueSql("uuid_generate_v4()");
    }
}