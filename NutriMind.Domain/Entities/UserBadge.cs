using NutriMind.Domain.Common;
using System;

namespace NutriMind.Domain.Entities
{
    public class UserBadge : BaseEntity
    {
        public DateTime AwardedAt { get; set; } = DateTime.UtcNow;

        public Guid UserId { get; set; }
        public virtual User User { get; set; } = null!;

        public int BadgeId { get; set; }
        public virtual Badge Badge { get; set; } = null!;
    }
}