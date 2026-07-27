using NutriMind.Domain.Common;
using System;
using System.Collections.Generic;

namespace NutriMind.Domain.Entities
{
    public class MealPlan : BaseEntity
    {
        public string Name { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal TotalCaloriesPerDay { get; set; }
        public bool IsAIGenerated { get; set; }

        public Guid UserId { get; set; }
        public virtual User User { get; set; }

        public virtual ICollection<PlannedMeal> PlannedMeals { get; set; } = new List<PlannedMeal>();
    }
}
