using NutriMind.Domain.Common;
using System;

namespace NutriMind.Domain.Entities
{
    public class PlannedMeal : BaseEntity
    {
        public DateTime Day { get; set; }
        public decimal QuantityG { get; set; }

        public Guid MealPlanId { get; set; }
        public virtual MealPlan MealPlan { get; set; }

        public int MealTypeId { get; set; }
        public virtual MealType MealType { get; set; }

        public Guid FoodId { get; set; }
        public virtual Food Food { get; set; }
    }
}
