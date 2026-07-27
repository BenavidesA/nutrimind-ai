#nullable enable
using System;

namespace NutriMind.Application.DTOs.MealPlans;

public class UpdateMealPlanDto
{
    public string Name { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal TotalCaloriesPerDay { get; set; }
}
