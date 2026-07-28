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

        // Relationship with MealPlan (a plan has many meals)
        builder.HasOne(pm => pm.MealPlan)
            .WithMany(mp => mp.PlannedMeals)
            .HasForeignKey(pm => pm.MealPlanId)
            .OnDelete(DeleteBehavior.Cascade); // If the plan is deleted, its planned meals are deleted too

        // Relationship with Food (don't delete the food if the plan is deleted)
        builder.HasOne(pm => pm.Food)
            .WithMany()
            .HasForeignKey(pm => pm.FoodId)
            .OnDelete(DeleteBehavior.Restrict);

        // Relationship with MealType
        builder.HasOne(pm => pm.MealType)
            .WithMany()
            .HasForeignKey(pm => pm.MealTypeId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}