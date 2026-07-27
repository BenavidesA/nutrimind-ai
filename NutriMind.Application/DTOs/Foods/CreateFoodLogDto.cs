using System;

namespace NutriMind.Application.DTOs.Foods
{
    public class CreateFoodLogDto
    {
        public Guid FoodId { get; set; }
        public int MealTypeId { get; set; }
        public DateTime LogDate { get; set; }
        public decimal QuantityG { get; set; }
        public string? Notes { get; set; }
    }
}
