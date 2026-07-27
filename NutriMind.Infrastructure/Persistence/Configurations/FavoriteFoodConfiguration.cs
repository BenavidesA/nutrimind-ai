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

            // Relación con User
            builder.HasOne(ff => ff.User)
                .WithMany(u => u.FavoriteFoods)
                .HasForeignKey(ff => ff.UserId)
                .OnDelete(DeleteBehavior.Cascade); // Si se borra el usuario, se borran sus favoritos.

            // Relación con Food
            builder.HasOne(ff => ff.Food)
                .WithMany() // No necesitamos una colección de "favoritedBy" en Food.
                .HasForeignKey(ff => ff.FoodId)
                .OnDelete(DeleteBehavior.Cascade); // Si se borra el alimento, se borra de los favoritos.

            // Restricción ÚNICA compuesta
            builder.HasIndex(ff => new { ff.UserId, ff.FoodId }).IsUnique();
            #nullable enable
        }
    }
}
