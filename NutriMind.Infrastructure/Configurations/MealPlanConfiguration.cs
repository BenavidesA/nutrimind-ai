#nullable enable
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NutriMind.Domain.Entities;

namespace NutriMind.Infrastructure.Configurations;

public class MealPlanConfiguration : IEntityTypeConfiguration<MealPlan>
{
    public void Configure(EntityTypeBuilder<MealPlan> builder)
    {
        builder.ToTable("MealPlans");

        builder.HasKey(mp => mp.Id);

        builder.Property(mp => mp.Name)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(mp => mp.TotalCaloriesPerDay)
            .HasPrecision(18, 2);

        // Relationship with User (a user has many plans)
        builder.HasOne(mp => mp.User)
            .WithMany()
            .HasForeignKey(mp => mp.UserId)
            .OnDelete(DeleteBehavior.Cascade); // If the user is deleted, their plans are deleted too
    }
}