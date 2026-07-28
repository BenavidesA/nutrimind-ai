namespace NutriMind.Web.Models;

// Copy of NutriMind.Domain.Enums.DietaryGoal — same order (serializes as int in the API's
// JSON), Web doesn't reference Domain (decoupled project).
public enum DietaryGoal
{
    LoseWeight,
    MaintainWeight,
    GainMuscle
}
