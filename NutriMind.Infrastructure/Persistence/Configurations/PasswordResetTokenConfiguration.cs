using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NutriMind.Domain.Entities;

namespace NutriMind.Infrastructure.Persistence.Configurations
{
    public class PasswordResetTokenConfiguration : IEntityTypeConfiguration<PasswordResetToken>
    {
        public void Configure(EntityTypeBuilder<PasswordResetToken> builder)
        {
            #nullable disable
            builder.ToTable("PasswordResetTokens");

            builder.HasKey(t => t.Id);

            builder.Property(t => t.Token)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(t => t.ExpiresAt)
                .IsRequired();

            builder.Property(t => t.IsUsed)
                .HasDefaultValue(false);

            // Relación con User: Un usuario puede tener varios intentos de reseteo
            builder.HasOne(t => t.User)
                .WithMany() // No necesitamos una colección de PasswordResetTokens en User
                .HasForeignKey(t => t.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            #nullable enable
        }
    }
}
