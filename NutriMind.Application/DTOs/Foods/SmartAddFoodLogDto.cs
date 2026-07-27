using System;
namespace NutriMind.Application.DTOs.Foods
{
    public class SmartAddFoodLogDto
    {
        public string Name { get; set; } = string.Empty;
        public decimal QuantityG { get; set; }
        public int MealTypeId { get; set; } = 1;
        public DateTime LogDate { get; set; } = DateTime.UtcNow;
        public string? Notes { get; set; }
    }
}