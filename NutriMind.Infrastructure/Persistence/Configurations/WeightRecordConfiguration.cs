using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NutriMind.Domain.Entities;

namespace NutriMind.Infrastructure.Persistence.Configurations
{
    public class WeightRecordConfiguration : IEntityTypeConfiguration<WeightRecord>
    {
        public void Configure(EntityTypeBuilder<WeightRecord> builder)
        {
            builder.Property(w => w.WeightKg).HasPrecision(8, 2);
        }
    }
}
