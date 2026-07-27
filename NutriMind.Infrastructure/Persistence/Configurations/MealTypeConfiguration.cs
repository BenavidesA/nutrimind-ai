using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NutriMind.Domain.Entities;

namespace NutriMind.Infrastructure.Persistence.Configurations
{
    public class MealTypeConfiguration : IEntityTypeConfiguration<MealType>
    {
        public void Configure(EntityTypeBuilder<MealType> builder)
        {
            #nullable disable
            builder.ToTable("MealTypes");

            builder.HasKey(mt => mt.Id);

            builder.Property(mt => mt.Name)
                .IsRequired()
                .HasMaxLength(50);

            // Pre-cargar datos iniciales (seed data)
            builder.HasData(
                new MealType { Id = 1, Name = "Breakfast", DisplayOrder = 1, IconName = "breakfast_icon" },
                new MealType { Id = 2, Name = "Lunch", DisplayOrder = 2, IconName = "lunch_icon" },
                new MealType { Id = 3, Name = "Dinner", DisplayOrder = 3, IconName = "dinner_icon" },
                new MealType { Id = 4, Name = "Snack", DisplayOrder = 4, IconName = "snack_icon" }
            );
            #nullable enable
        }
    }
}
