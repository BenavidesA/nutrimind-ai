#nullable enable
using System;

namespace NutriMind.Domain.Entities;

public class MealPlanItem
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid MealPlanDayId { get; set; }

    public Guid FoodItemId { get; set; } // Relationship with the FoodItem entity we created in Phase 4

    public int MealTypeId { get; set; } // E.g.: 1=Breakfast, 2=Lunch (relationship with the MealType Enum/Entity)

    public decimal Quantity { get; set; } // E.g.: 100

    public string Unit { get; set; } = "g"; // E.g.: "g", "ml", "scoop"

    // Navigation property
    public MealPlanDay? MealPlanDay { get; set; }
}