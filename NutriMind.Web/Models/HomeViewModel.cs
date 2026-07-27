using NutriMind.Web.Services.Dtos;

namespace NutriMind.Web.Models;

public class HomeViewModel
{
    public string FirstName { get; set; } = string.Empty;
    public int CurrentStreak { get; set; }
    public int TotalPoints { get; set; }
    public DashboardStatsDto? TodayStats { get; set; }
    public List<BadgeResponseDto> RecentBadges { get; set; } = new();
    public string? ErrorMessage { get; set; }
}
