#nullable enable
using System;
using System.Collections.Generic;

namespace NutriMind.Domain.Entities;

public class MealPlanDay
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid MealPlanId { get; set; }

    public int DayNumber { get; set; } // E.g.: 1 for the first day of the plan

    public string? Notes { get; set; }

    // Navigation properties
    public MealPlan? MealPlan { get; set; }
    public ICollection<MealPlanItem> Items { get; set; } = new List<MealPlanItem>();
}