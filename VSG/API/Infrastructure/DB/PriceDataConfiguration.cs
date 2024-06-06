using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyApi.Models; 

public class PriceDataConfiguration : IEntityTypeConfiguration<PriceData>
{
    public void Configure(EntityTypeBuilder<PriceData> builder)
    {
        
        builder.Property(p => p.Symbol)
            .IsRequired()
            .HasMaxLength(50); 
        
        builder.Property(p => p.Price)
            .IsRequired();  
        
        builder.Property(p => p.Timestamp)
            .IsRequired();  
    }
}



using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyApi.Models;

public class PriceConfiguration : IEntityTypeConfiguration<Price>
{
    public void Configure(EntityTypeBuilder<Price> builder)
    {
        // Configure table name
        builder.ToTable("Prices");

        // Configure primary key
        builder.HasKey(p => p.Id);

        // Configure properties
        builder.Property(p => p.EventType)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(p => p.EventTime)
            .IsRequired();

        builder.Property(p => p.Symbol)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(p => p.AveragePriceInterval)
            .IsRequired()
            .HasMaxLength(10);

        builder.Property(p => p.AveragePrice)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(p => p.LastTradeTime)
            .IsRequired();
    }
}
