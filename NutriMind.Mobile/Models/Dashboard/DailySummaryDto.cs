namespace NutriMind.Mobile.Models.Dashboard;

public sealed class DailySummaryDto
{
    public DateOnly Date { get; init; }
    public decimal TotalCalories { get; init; }
    public decimal TotalProtein { get; init; }
    public decimal TotalCarbs { get; init; }
    public decimal TotalFat { get; init; }
    public decimal CaloriesGoal { get; init; }

    public bool GoalAchieved => TotalCalories >= CaloriesGoal * 0.90m && TotalCalories <= CaloriesGoal * 1.10m;

    public string DayLabel => Date.DayOfWeek switch
    {
        DayOfWeek.Monday => "Lun",
        DayOfWeek.Tuesday => "Mar",
        DayOfWeek.Wednesday => "Mié",
        DayOfWeek.Thursday => "Jue",
        DayOfWeek.Friday => "Vie",
        DayOfWeek.Saturday => "Sáb",
        DayOfWeek.Sunday => "Dom",
        _ => "?"
    };
}