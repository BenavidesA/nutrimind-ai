using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NutriMind.Domain.Entities;

namespace NutriMind.Infrastructure.Persistence.Configurations
{
    public class FavoriteFoodConfiguration : IEntityTypeConfiguration<FavoriteFood>
    {
        public void Configure(EntityTypeBuilder<FavoriteFood> builder)
        {
            #nullable disable
            builder.ToTable("FavoriteFoods");

            builder.HasKey(ff => ff.Id);

            // Relationship with User
            builder.HasOne(ff => ff.User)
                .WithMany(u => u.FavoriteFoods)
                .HasForeignKey(ff => ff.UserId)
                .OnDelete(DeleteBehavior.Cascade); // If the user is deleted, their favorites are deleted too.

            // Relationship with Food
            builder.HasOne(ff => ff.Food)
                .WithMany() // We don't need a "favoritedBy" collection on Food.
                .HasForeignKey(ff => ff.FoodId)
                .OnDelete(DeleteBehavior.Cascade); // If the food is deleted, it's removed from favorites.

            // Composite UNIQUE constraint
            builder.HasIndex(ff => new { ff.UserId, ff.FoodId }).IsUnique();
            #nullable enable
        }
    }
}
