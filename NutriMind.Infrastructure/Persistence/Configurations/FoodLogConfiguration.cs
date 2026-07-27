using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NutriMind.Domain.Entities;

namespace NutriMind.Infrastructure.Persistence.Configurations
{
    public class FoodLogConfiguration : IEntityTypeConfiguration<FoodLog>
    {
        public void Configure(EntityTypeBuilder<FoodLog> builder)
        {
            builder.HasIndex(fl => new { fl.UserId, fl.LogDate });

            builder.Property(fl => fl.QuantityG).HasPrecision(8, 2);
            builder.Property(fl => fl.Calories).HasPrecision(8, 2);
            builder.Property(fl => fl.Protein).HasPrecision(8, 2);
            builder.Property(fl => fl.Carbs).HasPrecision(8, 2);
            builder.Property(fl => fl.Fat).HasPrecision(8, 2);
        }
    }
}
