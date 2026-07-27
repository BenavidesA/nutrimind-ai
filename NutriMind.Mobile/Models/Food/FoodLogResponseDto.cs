using System;

namespace NutriMind.Mobile.Models.Food;

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
    public string FoodName { get; set; } = string.Empty;
    public int MealTypeId { get; set; }
    public string MealTypeName { get; set; } = string.Empty;
}