#nullable enable
using System;
using System.Collections.Generic;

namespace NutriMind.Application.DTOs.MealPlans;

public class MealPlanResponseDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal TotalCaloriesPerDay { get; set; }
    public bool IsAIGenerated { get; set; }

    public List<PlannedMealResponseDto> PlannedMeals { get; set; } = new();
}

public class PlannedMealResponseDto
{
    public Guid Id { get; set; }
    public DateTime Day { get; set; }
    public decimal QuantityG { get; set; }
    public int MealTypeId { get; set; }
    public Guid FoodId { get; set; }
}