#nullable enable
using System;

namespace NutriMind.Domain.Entities;

public class MealPlanItem
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid MealPlanDayId { get; set; }

    public Guid FoodItemId { get; set; } // Relación con la entidad FoodItem que creamos en Fase 4

    public int MealTypeId { get; set; } // Ej: 1=Desayuno, 2=Almuerzo (Relación con tu Enum/Entidad MealType)

    public decimal Quantity { get; set; } // Ej: 100

    public string Unit { get; set; } = "g"; // Ej: "g", "ml", "scoop"

    // Propiedad de navegación
    public MealPlanDay? MealPlanDay { get; set; }
}