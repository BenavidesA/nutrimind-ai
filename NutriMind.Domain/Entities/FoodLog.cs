using NutriMind.Domain.Common;
using System;

namespace NutriMind.Domain.Entities
{
    public class FoodLog : BaseEntity
    {
        public DateTime LogDate { get; set; }
        public decimal QuantityG { get; set; }
        public decimal Calories { get; set; }
        public decimal Protein { get; set; }
        public decimal Carbs { get; set; }
        public decimal Fat { get; set; }
        public string Notes { get; set; }

        public Guid UserId { get; set; }
        public virtual User User { get; set; }

        public Guid FoodId { get; set; }
        public virtual Food Food { get; set; }

        public int MealTypeId { get; set; }
        public virtual MealType MealType { get; set; }
    }
}
