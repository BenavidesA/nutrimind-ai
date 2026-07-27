namespace NutriMind.Mobile.Models.Profile;

public class UpdateProfileDto
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public int? Age { get; set; }
    public string? Gender { get; set; }
    public decimal? HeightCm { get; set; }
    public decimal? WeightKg { get; set; }
    public ActivityLevel ActivityLevel { get; set; }
    public DietaryGoal DietaryGoal { get; set; }
    public string? University { get; set; }
}
