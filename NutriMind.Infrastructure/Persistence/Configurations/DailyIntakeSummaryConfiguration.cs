using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NutriMind.Domain.Entities;

namespace NutriMind.Infrastructure.Persistence.Configurations
{
    public class DailyIntakeSummaryConfiguration : IEntityTypeConfiguration<DailyIntakeSummary>
    {
        public void Configure(EntityTypeBuilder<DailyIntakeSummary> builder)
        {
            builder.Property(s => s.TotalCalories).HasPrecision(8, 2);
            builder.Property(s => s.TotalProtein).HasPrecision(8, 2);
            builder.Property(s => s.TotalCarbs).HasPrecision(8, 2);
            builder.Property(s => s.TotalFat).HasPrecision(8, 2);
        }
    }
}
