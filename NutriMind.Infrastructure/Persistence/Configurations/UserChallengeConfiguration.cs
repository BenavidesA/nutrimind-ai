using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NutriMind.Domain.Entities;

namespace NutriMind.Infrastructure.Persistence.Configurations
{
    public class UserChallengeConfiguration : IEntityTypeConfiguration<UserChallenge>
    {
        public void Configure(EntityTypeBuilder<UserChallenge> builder)
        {
            builder.Property(uc => uc.Progress).HasPrecision(8, 2);
        }
    }
}
