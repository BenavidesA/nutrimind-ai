using System;
using System.Collections.Generic;

namespace NutriMind.Domain.Entities
{
    public class Challenge
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int PointsAwarded { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsActive { get; set; }

        public virtual ICollection<UserChallenge> UserChallenges { get; set; } = new List<UserChallenge>();
    }
}
