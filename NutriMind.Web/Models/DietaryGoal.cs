namespace NutriMind.Web.Models;

// Copia de NutriMind.Domain.Enums.DietaryGoal — mismo orden (serializa como int en el JSON de
// la API), Web no referencia Domain (proyecto desacoplado).
public enum DietaryGoal
{
    LoseWeight,
    MaintainWeight,
    GainMuscle
}
