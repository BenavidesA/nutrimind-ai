using System.Collections.Generic;

namespace NutriMind.Domain.Entities
{
    public class Badge
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;        // <-- Add this
        public string Description { get; set; } = string.Empty; // <-- Add this
        public string ImageUrl { get; set; } = string.Empty;    // <-- Add this
        public int PointsReward { get; set; } = 0;
        public virtual ICollection<UserBadge> UserBadges { get; set; } = new List<UserBadge>();
    }
}
