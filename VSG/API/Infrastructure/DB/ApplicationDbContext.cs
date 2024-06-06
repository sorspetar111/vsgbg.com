using Microsoft.EntityFrameworkCore;
using MyApi.Models; 

public class ApplicationDbContext : DbContext
{
    // public DbSet<PriceData> PriceData { get; set; }
    public DbSet<Price> Prices { get; set; }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Apply the configuration for PriceData entity
        // modelBuilder.ApplyConfiguration(new PriceDataConfiguration());
        modelBuilder.ApplyConfiguration(new PriceConfiguration());
    }
}
