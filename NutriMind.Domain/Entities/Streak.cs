using NutriMind.Domain.Common;
using System;

namespace NutriMind.Domain.Entities
{
    public class Streak : BaseEntity
    {
        public int CurrentStreak { get; set; }
        public int LongestStreak { get; set; }
        public DateTime LastActivityDate { get; set; }

        public Guid UserId { get; set; }
        public virtual User User { get; set; }
    }
}
