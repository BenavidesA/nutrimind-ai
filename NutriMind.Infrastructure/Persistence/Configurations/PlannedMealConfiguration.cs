using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NutriMind.Domain.Entities;

namespace NutriMind.Infrastructure.Persistence.Configurations
{
    public class PlannedMealConfiguration : IEntityTypeConfiguration<PlannedMeal>
    {
        public void Configure(EntityTypeBuilder<PlannedMeal> builder)
        {
            builder.Property(p => p.QuantityG).HasPrecision(8, 2);
        }
    }
}
