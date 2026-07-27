using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NutriMind.Domain.Entities;

namespace NutriMind.Infrastructure.Persistence.Configurations
{
    public class NutritionGoalConfiguration : IEntityTypeConfiguration<NutritionGoal>
    {
        public void Configure(EntityTypeBuilder<NutritionGoal> builder)
        {
            builder.Property(g => g.TargetCalories).HasPrecision(8, 2);
            builder.Property(g => g.TargetProtein).HasPrecision(8, 2);
            builder.Property(g => g.TargetCarbs).HasPrecision(8, 2);
            builder.Property(g => g.TargetFat).HasPrecision(8, 2);
        }
    }
}
