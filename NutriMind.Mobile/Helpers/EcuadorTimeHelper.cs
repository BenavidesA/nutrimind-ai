namespace NutriMind.Mobile.Helpers;

// Copia del helper equivalente en NutriMind.Application/Common/EcuadorTimeHelper.cs — Mobile no
// referencia Application (proyecto desacoplado, solo consume la API por HTTP), así que no se
// puede compartir la clase directamente. Ecuador usa un offset fijo de UTC-5 todo el año (no
// observa horario de verano/DST), por lo que sumar/restar horas fijas es seguro.
public static class EcuadorTimeHelper
{
    private static readonly TimeSpan Offset = TimeSpan.FromHours(-5);

    public static DateTime ToLocal(DateTime utc) => utc.Add(Offset);

    // Auto-detecta el tipo de comida según la hora local de Ecuador al momento de registrar.
    // IDs reales de MealTypes (seed data, NutriMind.Infrastructure/Persistence/Configurations/
    // MealTypeConfiguration.cs): 1=Breakfast, 2=Lunch, 3=Dinner, 4=Snack.
    public static int GetMealTypeIdForNow()
    {
        var hour = ToLocal(DateTime.UtcNow).Hour;
        return hour switch
        {
            >= 5 and < 11 => 1,  // Desayuno
            >= 11 and < 16 => 2, // Almuerzo
            >= 16 and < 21 => 3, // Cena
            _ => 4                // Snack
        };
    }
}
