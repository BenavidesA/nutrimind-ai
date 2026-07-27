using NutriMind.Domain.Common;
using NutriMind.Domain.Enums;
using System;

namespace NutriMind.Domain.Entities
{
    public class UserProfile : BaseEntity
    {
        public int? Age { get; set; }
        public string Gender { get; set; }
        public decimal? HeightCm { get; set; }
        public decimal? WeightKg { get; set; }
        public ActivityLevel ActivityLevel { get; set; }
        public DietaryGoal DietaryGoal { get; set; }
        public string ProfileImageUrl { get; set; }
        public string University { get; set; }

        public Guid UserId { get; set; }
        public virtual User User { get; set; }
    }
}
