using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NutriMind.Domain.Entities;

namespace NutriMind.Infrastructure.Persistence.Configurations
{
    public class RestaurantCacheConfiguration : IEntityTypeConfiguration<RestaurantCache>
    {
        public void Configure(EntityTypeBuilder<RestaurantCache> builder)
        {
            builder.Property(r => r.Latitude).HasPrecision(10, 7);
            builder.Property(r => r.Longitude).HasPrecision(10, 7);
        }
    }
}
