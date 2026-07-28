namespace NutriMind.Web.Models;

// Copy of NutriMind.Domain.Enums.ActivityLevel — same order (serializes as int in the API's
// JSON), Web doesn't reference Domain (decoupled project).
public enum ActivityLevel
{
    Sedentary,
    Light,
    Moderate,
    Active,
    VeryActive
}
