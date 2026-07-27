namespace NutriMind.Mobile.Models.Profile;

// El orden de estos miembros debe coincidir EXACTO con NutriMind.Domain.Enums.DietaryGoal
// (ver nota en ActivityLevel.cs sobre serialización de enums como enteros).
public enum DietaryGoal
{
    LoseWeight,
    MaintainWeight,
    GainMuscle
}
