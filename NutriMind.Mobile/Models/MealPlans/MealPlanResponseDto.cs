using NutriMind.Mobile.Helpers;

namespace NutriMind.Mobile.Models.MealPlans;

public class MealPlanResponseDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal TotalCaloriesPerDay { get; set; }
    public bool IsAIGenerated { get; set; }
    public List<PlannedMealDto> PlannedMeals { get; set; } = new();

    // Calculado en el momento de bindear, no viene del backend — mismo patrón que
    // DailySummaryDto.GoalAchieved. Usa hora Ecuador, no UTC crudo.
    public bool IsActive
    {
        get
        {
            var today = EcuadorTimeHelper.ToLocal(DateTime.UtcNow).Date;
            return StartDate.Date <= today && today <= EndDate.Date;
        }
    }
}

public class PlannedMealDto
{
    public DateTime Day { get; set; }
    public decimal QuantityG { get; set; }
    public int MealTypeId { get; set; }
    public Guid FoodId { get; set; }
}
