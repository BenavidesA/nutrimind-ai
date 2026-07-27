namespace NutriMind.Web.Services.Dtos;

public class DashboardStatsDto
{
    public decimal TotalCaloriesConsumed { get; set; }
    public decimal TotalProtein { get; set; }
    public decimal TotalCarbs { get; set; }
    public decimal TotalFat { get; set; }

    public List<DailyStatDto> DailyBreakdown { get; set; } = new();
}

public class DailyStatDto
{
    public string Date { get; set; } = string.Empty;
    public decimal Calories { get; set; }
}
