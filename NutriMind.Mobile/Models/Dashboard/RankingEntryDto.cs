namespace NutriMind.Mobile.Models.Dashboard;

public class RankingEntryDto
{
    public string UserId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public int TotalPoints { get; set; }
    public int Rank { get; set; }
    public bool IsCurrentUser { get; set; }
}
