namespace NutriMind.Web.Models;

// Copia de NutriMind.Domain.Enums.ActivityLevel — mismo orden (serializa como int en el JSON de
// la API), Web no referencia Domain (proyecto desacoplado).
public enum ActivityLevel
{
    Sedentary,
    Light,
    Moderate,
    Active,
    VeryActive
}
