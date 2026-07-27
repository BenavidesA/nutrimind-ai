using System;

namespace NutriMind.Mobile.Models.Food;

public class QuickAddFoodLogDto
{
    public string Name { get; set; } = string.Empty;
    public decimal Calories { get; set; }
    public decimal Carbs { get; set; }
    public decimal Protein { get; set; }
    public decimal Fat { get; set; }
    public int MealTypeId { get; set; } = 1;
    public DateTime LogDate { get; set; } = DateTime.UtcNow;
    public string? Notes { get; set; }
}