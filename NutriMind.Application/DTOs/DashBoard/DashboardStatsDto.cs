#nullable enable
using System.Collections.Generic;

namespace NutriMind.Application.DTOs.Dashboard;

public class DashboardStatsDto
{
    public decimal TotalCaloriesConsumed { get; set; }
    public decimal TotalProtein { get; set; }
    public decimal TotalCarbs { get; set; }
    public decimal TotalFat { get; set; }

    // Lista para gráficas diarias
    public List<DailyStatDto> DailyBreakdown { get; set; } = new();
}

public class DailyStatDto
{
    public string Date { get; set; } = string.Empty;
    public decimal Calories { get; set; }
}