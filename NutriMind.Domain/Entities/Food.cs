using NutriMind.Domain.Common;
using System;
using System.Collections.Generic;

namespace NutriMind.Domain.Entities
{
    public class Food : BaseEntity
    {
        public string Name { get; set; }
        public string Brand { get; set; }
        public string Barcode { get; set; }
        public decimal ServingSizeG { get; set; }
        public string ServingUnit { get; set; }
        public decimal CaloriesPer100g { get; set; }
        public decimal ProteinPer100g { get; set; }
        public decimal CarbsPer100g { get; set; }
        public decimal FatPer100g { get; set; }
        public decimal FiberPer100g { get; set; }
        public decimal SugarPer100g { get; set; }
        public decimal SodiumPer100g { get; set; }
        public string ImageUrl { get; set; }
        public string ExternalId { get; set; }
        public string Source { get; set; }
        public bool IsVerified { get; set; }

        public int FoodCategoryId { get; set; }
        public virtual FoodCategory FoodCategory { get; set; }
        public virtual ICollection<FavoriteFood> FavoriteFoods { get; set; } = new List<FavoriteFood>();
        public virtual ICollection<FoodLog> FoodLogs { get; set; } = new List<FoodLog>();
        public virtual ICollection<PlannedMeal> PlannedMeals { get; set; } = new List<PlannedMeal>();
    }
}
