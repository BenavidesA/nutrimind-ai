#nullable enable
using System;
using System.Collections.Generic;

namespace NutriMind.Domain.Entities;

public class MealPlanDay
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid MealPlanId { get; set; }

    public int DayNumber { get; set; } // Ej: 1 para el primer día del plan

    public string? Notes { get; set; }

    // Propiedades de navegación
    public MealPlan? MealPlan { get; set; }
    public ICollection<MealPlanItem> Items { get; set; } = new List<MealPlanItem>();
}