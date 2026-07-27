using NutriMind.Domain.Common;
using System;

namespace NutriMind.Domain.Entities
{
    public class UserChallenge : BaseEntity
    {
        public DateTime StartedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public decimal Progress { get; set; }

        public Guid UserId { get; set; }
        public virtual User User { get; set; }

        public int ChallengeId { get; set; }
        public virtual Challenge Challenge { get; set; }
    }
}
