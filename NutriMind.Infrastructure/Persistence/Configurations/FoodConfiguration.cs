using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NutriMind.Domain.Entities;

namespace NutriMind.Infrastructure.Persistence.Configurations
{
    public class FoodConfiguration : IEntityTypeConfiguration<Food>
    {
        public void Configure(EntityTypeBuilder<Food> builder)
        {
            builder.Property(f => f.ServingSizeG).HasPrecision(8, 2);
            builder.Property(f => f.CaloriesPer100g).HasPrecision(8, 2);
            builder.Property(f => f.ProteinPer100g).HasPrecision(8, 2);
            builder.Property(f => f.CarbsPer100g).HasPrecision(8, 2);
            builder.Property(f => f.FatPer100g).HasPrecision(8, 2);
        }
    }
}
