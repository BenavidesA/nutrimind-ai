using NutriMind.Domain.Common;
using System;

namespace NutriMind.Domain.Entities
{
    public class NutritionGoal : BaseEntity
    {
        public decimal TargetCalories { get; set; }
        public decimal TargetProtein { get; set; }
        public decimal TargetCarbs { get; set; }
        public decimal TargetFat { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsActive { get; set; }

        public Guid UserId { get; set; }
        public virtual User User { get; set; }
    }
}
