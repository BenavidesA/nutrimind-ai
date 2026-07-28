namespace NutriMind.Mobile.Models.Profile;

// The order of these members must match NutriMind.Domain.Enums.ActivityLevel EXACTLY:
// the backend serializes enums as integers (no JsonStringEnumConverter configured),
// so a different order here would cause values to be silently misinterpreted.
public enum ActivityLevel
{
    Sedentary,
    Light,
    Moderate,
    Active,
    VeryActive
}
