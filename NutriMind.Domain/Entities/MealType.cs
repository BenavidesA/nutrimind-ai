using System.Collections.Generic;

namespace NutriMind.Domain.Entities
{
    public class MealType
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int DisplayOrder { get; set; }
        public string IconName { get; set; }

        public virtual ICollection<FoodLog> FoodLogs { get; set; } = new List<FoodLog>();
        public virtual ICollection<PlannedMeal> PlannedMeals { get; set; } = new List<PlannedMeal>();
    }
}
