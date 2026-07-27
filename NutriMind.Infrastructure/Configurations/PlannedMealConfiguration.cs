#nullable enable
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NutriMind.Domain.Entities;

namespace NutriMind.Infrastructure.Configurations;

public class PlannedMealConfiguration : IEntityTypeConfiguration<PlannedMeal>
{
    public void Configure(EntityTypeBuilder<PlannedMeal> builder)
    {
        builder.ToTable("PlannedMeals");

        builder.HasKey(pm => pm.Id);

        builder.Property(pm => pm.QuantityG)
            .HasPrecision(18, 2);

        // Relación con MealPlan (Un plan tiene muchas comidas)
        builder.HasOne(pm => pm.MealPlan)
            .WithMany(mp => mp.PlannedMeals)
            .HasForeignKey(pm => pm.MealPlanId)
            .OnDelete(DeleteBehavior.Cascade); // Si se borra el plan, se borran sus comidas planificadas

        // Relación con Food (No borrar la comida si se borra el plan)
        builder.HasOne(pm => pm.Food)
            .WithMany()
            .HasForeignKey(pm => pm.FoodId)
            .OnDelete(DeleteBehavior.Restrict);

        // Relación con MealType
        builder.HasOne(pm => pm.MealType)
            .WithMany()
            .HasForeignKey(pm => pm.MealTypeId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}