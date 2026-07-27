using System;

namespace NutriMind.Mobile.Models.Food;

public class FoodItemDto
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; } = string.Empty;

    // Cambiados a double para que coincidan EXACTAMENTE con la API y no rompan el JSON
    public double Calories { get; set; }
    public double Carbs { get; set; }
    public double Protein { get; set; }
    public double Fat { get; set; }
    public double ServingSizeG { get; set; } = 100;
    public int MealTypeId { get; set; }
    public DateTime LogDate { get; set; }
    public string? Notes { get; set; }

    // Propiedad de solo lectura para mantener la UI limpia
    public string MacrosSummary => $"{ServingSizeG:0}g  •  {Calories:0} kcal  •  {Carbs:0}g Carbos  •  {Protein:0}g Prot.";
}