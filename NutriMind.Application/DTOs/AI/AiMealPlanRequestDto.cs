#nullable enable
using System.Collections.Generic;

namespace NutriMind.Application.DTOs.AI;

public class AiMealPlanRequestDto
{
    public decimal TargetCalories { get; set; }
    public int Days { get; set; } = 1;
    public List<string> Allergies { get; set; } = new();
    public string DietType { get; set; } = "Cualquiera";
}