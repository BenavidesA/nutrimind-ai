using System;

namespace NutriMind.Application.DTOs.Foods
{
    public class FoodLogResponseDto
    {
        public Guid Id { get; set; }
        public DateTime LogDate { get; set; }
        public decimal QuantityG { get; set; }
        public decimal Calories { get; set; }
        public decimal Protein { get; set; }
        public decimal Carbs { get; set; }
        public decimal Fat { get; set; }
        public string? Notes { get; set; }

        public Guid FoodId { get; set; }
        public required string FoodName { get; set; }

        public int MealTypeId { get; set; }
        public required string MealTypeName { get; set; }
    }
}
