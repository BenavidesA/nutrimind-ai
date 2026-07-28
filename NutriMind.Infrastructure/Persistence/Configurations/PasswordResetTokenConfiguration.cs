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

            // Relationship with User: a user can have several reset attempts
            builder.HasOne(t => t.User)
                .WithMany() // We don't need a PasswordResetTokens collection on User
                .HasForeignKey(t => t.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            #nullable enable
        }
    }
}
