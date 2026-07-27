using NutriMind.Domain.Common;
using System;

namespace NutriMind.Domain.Entities
{
    public class AIRecommendation : BaseEntity
    {
        public string RecommendationType { get; set; } // e.g., "Meal", "Workout", "Article"
        public string Content { get; set; }
        public bool IsAccepted { get; set; }
        public DateTime? AcceptedAt { get; set; }

        public Guid UserId { get; set; }
        public virtual User User { get; set; }
    }
}
