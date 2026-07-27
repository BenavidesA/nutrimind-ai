namespace NutriMind.Mobile.Models.Profile;

// El orden de estos miembros debe coincidir EXACTO con NutriMind.Domain.Enums.ActivityLevel:
// el backend serializa enums como enteros (no hay JsonStringEnumConverter configurado),
// así que un orden distinto aquí haría que los valores se interpreten mal en silencio.
public enum ActivityLevel
{
    Sedentary,
    Light,
    Moderate,
    Active,
    VeryActive
}
