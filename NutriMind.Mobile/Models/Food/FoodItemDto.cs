using System;

namespace NutriMind.Mobile.Models.Food;

public class FoodItemDto
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; } = string.Empty;

    // Changed to double so they match the API EXACTLY and don't break the JSON
    public double Calories { get; set; }
    public double Carbs { get; set; }
    public double Protein { get; set; }
    public double Fat { get; set; }
    public double ServingSizeG { get; set; } = 100;
    public int MealTypeId { get; set; }
    public DateTime LogDate { get; set; }
    public string? Notes { get; set; }

    // Read-only property to keep the UI clean
    public string MacrosSummary => $"{ServingSizeG:0}g  •  {Calories:0} kcal  •  {Carbs:0}g Carbos  •  {Protein:0}g Prot.";
}