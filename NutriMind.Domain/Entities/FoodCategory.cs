using System.Collections.Generic;

namespace NutriMind.Domain.Entities
{
    public class FoodCategory
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string IconUrl { get; set; }

        public virtual ICollection<Food> Foods { get; set; } = new List<Food>();
    }
}
