namespace NutriMind.Mobile.Models.Profile;

// The order of these members must match NutriMind.Domain.Enums.DietaryGoal EXACTLY
// (see note in ActivityLevel.cs about serializing enums as integers).
public enum DietaryGoal
{
    LoseWeight,
    MaintainWeight,
    GainMuscle
}
