using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NutriMind.Domain.Entities;

namespace NutriMind.Infrastructure.Persistence.Configurations
{
    public class BadgeConfiguration : IEntityTypeConfiguration<Badge>
    {
        public void Configure(EntityTypeBuilder<Badge> builder)
        {
            builder.ToTable("Badges");

            builder.HasKey(b => b.Id);

            builder.Property(b => b.Name)
                .IsRequired()
                .HasMaxLength(100);

            // Pre-cargar las medallas que GamificationService.CheckAndAwardBadgesAsync busca por nombre exacto.
            builder.HasData(
                new Badge { Id = 1, Name = "Racha de 3 Días", Description = "Registraste comida 3 días seguidos.", ImageUrl = string.Empty, PointsReward = 0 },
                new Badge { Id = 2, Name = "Racha de 7 Días", Description = "Registraste comida 7 días seguidos.", ImageUrl = string.Empty, PointsReward = 0 }
            );
        }
    }
}
